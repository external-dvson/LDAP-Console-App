using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ClientAzureFunction.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Configure settings
        services.Configure<ServiceBusSettings>(context.Configuration.GetSection("ServiceBusSettings"));
        services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));
        
        // Add any additional services here
    })
    .Build();

host.Run();