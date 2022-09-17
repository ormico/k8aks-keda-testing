// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;

IConfigurationRoot ReadConfiguration()
{
    //var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
    var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true)
        .AddUserSecrets<Program>()
        .AddEnvironmentVariables();
    var configurationRoot = builder.Build();

    return configurationRoot;
}

Console.WriteLine("Hello, World!");

var config = ReadConfiguration();

Console.WriteLine($"ConnectionString.serviceBus='{config.GetConnectionString("serviceBus")}'");
Console.WriteLine($"alpha='{config["alpha"]}'");
Console.WriteLine($"beta='{config["beta"]}'");
Console.WriteLine($"does-not-exist='{config["does-not-exist"]}'");
