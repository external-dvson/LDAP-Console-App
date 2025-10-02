using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ClientConsoleApp.Configuration;
using ClientConsoleApp.Interfaces;
using ClientConsoleApp.Services;
using ClientConsoleApp.Helpers;
using ClientConsoleApp.Models;

namespace ClientConsoleApp
{
    internal class Program
    {
        private static ServiceProvider? _serviceProvider;
        private static CancellationTokenSource? _cancellationTokenSource;

        static async Task Main(string[] args)
        {
            try
            {
                // Setup cancellation token for graceful shutdown
                _cancellationTokenSource = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    _cancellationTokenSource?.Cancel();
                };

                // Configure services
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                _serviceProvider = serviceCollection.BuildServiceProvider();

                // Display application header
                DisplayHelper.DisplayApplicationHeader();

                // Run the client
                await RunClientAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError("Application Error", ex.Message);
            }
            finally
            {
                Console.WriteLine(AppConstant.Messages.ApplicationStopped);
                Console.WriteLine(AppConstant.Messages.PressKeyToExit);
                Console.ReadKey();

                _serviceProvider?.Dispose();
                _cancellationTokenSource?.Dispose();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(AppConstant.Configuration.AppSettingsFile, optional: false, reloadOnChange: true)
                .Build();

            services.Configure<ServiceBusSettings>(configuration.GetSection(AppConstant.Configuration.ServiceBusSettingsSection));
            services.Configure<AppSettings>(configuration.GetSection(AppConstant.Configuration.AppSettingsSection));
            services.AddScoped<IServiceBusConsumerService, ServiceBusConsumerService>();
        }

        private static async Task RunClientAsync(CancellationToken cancellationToken)
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("Service provider is not initialized");
            }

            Console.WriteLine(AppConstant.Messages.ApplicationStarted);

            var serviceBusConsumer = _serviceProvider.GetRequiredService<IServiceBusConsumerService>();
            var appSettings = _serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

            try
            {
                // Connect to Service Bus
                var connected = await serviceBusConsumer.ConnectAsync();
                if (!connected)
                {
                    DisplayHelper.DisplayError("Connection Failed", "Could not connect to Azure Service Bus");
                    return;
                }

                // Start receiving messages
                await serviceBusConsumer.StartReceivingAsync(ProcessReceivedGroup);

                // Keep the application running until cancellation is requested
                try
                {
                    await Task.Delay(-1, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine();
                    Console.WriteLine("🛑 Shutdown requested...");
                }
            }
            finally
            {
                await serviceBusConsumer.DisconnectAsync();
                serviceBusConsumer.Dispose();
            }
        }

        private static async Task ProcessReceivedGroup(Group group)
        {
            try
            {
                // Display the received group data
                DisplayHelper.DisplayReceivedGroup(group);

                // Here you can add additional processing logic
                // For example: save to database, call APIs, generate reports, etc.
                
                await Task.CompletedTask; // Placeholder for async operations
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError("Processing Error", $"Failed to process group '{group.GroupName}': {ex.Message}");
            }
        }
    }
}