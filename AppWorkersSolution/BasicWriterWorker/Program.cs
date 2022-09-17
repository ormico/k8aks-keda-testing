using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

Console.WriteLine("Basic Writer Worker Starting...!");

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var config = builder.Build() ?? throw new Exception("Configuration is null");

string serviceBusConnectionString = config.GetConnectionString("serviceBus");
string queueName = config["queueName"];
int numberOfMessages = int.Parse(config["numberOfMessages"]);
int msgSendDelaySeconds = int.Parse(config["messageSendDelaySeconds"]);
Console.WriteLine($"numberOfMessages = {numberOfMessages}");
Console.WriteLine($"msgSendDelaySeconds = {msgSendDelaySeconds}");

var clientOptions = new ServiceBusClientOptions()
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
await using ServiceBusClient queueClient = new(serviceBusConnectionString, clientOptions);

CancellationTokenSource cts = new();

int cancelCount = 0;
// or use PosixSignalRegistration.Create(PosixSignal.SIGINT, fun context ->
Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs args) =>
{
    cancelCount++;
    if(cancelCount < 2)
    {
        Console.WriteLine("Press [CTRL]+C again to exit.");
        args.Cancel = true;
    }
    else
    {
        Console.Write("Cancelling...");
        args.Cancel = true;
        cts.Cancel();
        Console.WriteLine();
    }
};

Console.WriteLine("Press [CTRL]+C to Cancel.");

async Task SendMessagesAsync(int numMsgsToRepeat)
{
    string msgContent = "";

    try
    {
        await using ServiceBusSender serviceBusSender = queueClient.CreateSender(queueName);

        for (var i = 0; i < numMsgsToRepeat; i++)
        {
            msgContent = $"UTC:{DateTime.UtcNow.ToString()} - {i}";
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(msgContent));

            Console.WriteLine($"Sending: '{msgContent}'");

            //await using ServiceBusSender serviceBusSender = queueClient.CreateSender(queueName);
            await serviceBusSender.SendMessageAsync(message, cts.Token);

            if (msgSendDelaySeconds > 0)
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
