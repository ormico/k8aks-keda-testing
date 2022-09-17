using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

Console.WriteLine("Worker Starting...!");

IConfigurationRoot ReadConfiguration()
{
    var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true)
        .AddUserSecrets<Program>()
        .AddEnvironmentVariables();
    var configRoot = builder.Build();

    return configRoot;
}

var config = ReadConfiguration() ?? throw new Exception("Configuration is null");

//Console.WriteLine($"ConnectionString.serviceBus='{config.GetConnectionString("serviceBus")}'");
//Console.WriteLine($"alpha='{config["alpha"]}'");
//Console.WriteLine($"beta='{config["beta"]}'");
//Console.WriteLine($"does-not-exist='{config["does-not-exist"]}'");
//Console.WriteLine($"queuName='{config["queueName"]}'");

int numberOfMessages = int.Parse(config["numberOfMessages"]);
int msgSendDelaySeconds = int.Parse(config["messageSendDelaySeconds"]);
string serviceBusConnectionString = config.GetConnectionString("serviceBus");
string queueName = config["queueName"];

var clientOptions = new ServiceBusClientOptions()
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
await using ServiceBusClient queueClient = new ServiceBusClient(serviceBusConnectionString, clientOptions);

ServiceBusProcessor processor = queueClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());

async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
{
    // if canceled, don't make calls to CompleteMessageAsync(), CompleteAsync(), or AbandonAsync() etc.
    if(args.CancellationToken.IsCancellationRequested)
    {
        Console.WriteLine("Processing Cancelled");
        return;
    }

    Console.WriteLine($"Received message:#{args.Message.SequenceNumber} Payload:'{Encoding.UTF8.GetString(args.Message.Body)}'");
    
    // requires queueClient have RecieveMode.PeekLock
    await args.CompleteMessageAsync(args.Message);
}

processor.ProcessMessageAsync += ProcessMessagesAsync;

async Task ExceptionReceivedHandlerAsync(ProcessErrorEventArgs args)
{
    Console.WriteLine($"Message handling Error {args.Exception}. Entity Path: '{args.EntityPath}'");
}

processor.ProcessErrorAsync += ExceptionReceivedHandlerAsync;

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs args) =>
{
    Console.Write("Cancelling...");
    args.Cancel = true;
    cts.Cancel();
    Console.WriteLine();
};

Console.WriteLine("Press [CTRL]+C to Cancel.");

await processor.StartProcessingAsync();

async Task SendMessagesAsync(int numMsgsToRepeat)
{
    string msgContent = "";

    try
    {
        for (var i = 0; i < numMsgsToRepeat; i++)
        {
            msgContent = $"UTC:{DateTime.UtcNow.ToString()} - {i}";
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(msgContent));

            Console.WriteLine($"Sending: '{msgContent}'");

            await using ServiceBusSender serviceBusSender = queueClient.CreateSender(queueName);
            await serviceBusSender.SendMessageAsync(message, cts.Token);

            if(msgSendDelaySeconds > 0)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(msgSendDelaySeconds));
            }
        }
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Error Sending Message: '{msgContent}' Exception: {exception.Message}");
    }
}

await SendMessagesAsync(numberOfMessages);

Console.WriteLine("Worker exiting.");
