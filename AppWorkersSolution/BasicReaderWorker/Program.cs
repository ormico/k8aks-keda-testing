using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

Console.WriteLine("Basic Reader Worker Starting...!");

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var config = builder.Build() ?? throw new Exception("Configuration is null");

string serviceBusConnectionString = config.GetConnectionString("serviceBus");
string queueName = config["queueName"];
int readLifespanSeconds = int.Parse(config["readLifespanSeconds"]??"-1");
Console.WriteLine($"readLifespanSeconds = {readLifespanSeconds}");

var clientOptions = new ServiceBusClientOptions()
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
await using ServiceBusClient queueClient = new ServiceBusClient(serviceBusConnectionString, clientOptions);

ServiceBusProcessor processor = queueClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());

async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
{
    // if canceled, don't make calls to CompleteMessageAsync(), CompleteAsync(), or AbandonAsync() etc.
    if (args.CancellationToken.IsCancellationRequested)
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

CancellationTokenSource cts = readLifespanSeconds <= 0 ? new() : new(TimeSpan.FromSeconds(readLifespanSeconds));

Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs args) =>
{
    Console.Write("Cancelling...");
    args.Cancel = true;
    cts.Cancel();
    Console.WriteLine();
};

Console.WriteLine("Press [CTRL]+C to Cancel.");
await processor.StartProcessingAsync(cts.Token);

cts.Token.WaitHandle.WaitOne();

//while(!cts.Token.IsCancellationRequested)
//{
//    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.25));
//}

Console.WriteLine("Worker exiting.");
