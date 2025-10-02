using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using ClientConsoleApp.Configuration;
using ClientConsoleApp.Interfaces;
using ClientConsoleApp.Models;

namespace ClientConsoleApp.Services
{
    public class ServiceBusConsumerService : IServiceBusConsumerService, IDisposable
    {
        private readonly ServiceBusSettings _settings;
        private ServiceBusClient? _client;
        private ServiceBusProcessor? _processor;
        private bool _disposed = false;

        public ServiceBusConsumerService(IOptions<ServiceBusSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.ConnectionString) || string.IsNullOrEmpty(_settings.QueueName))
                {
                    Console.WriteLine("❌ Service Bus Connection string or queue name is not configured");
                    return false;
                }

                _client = new ServiceBusClient(_settings.ConnectionString);
                _processor = _client.CreateProcessor(_settings.QueueName, new ServiceBusProcessorOptions());

                Console.WriteLine(string.Format(AppConstant.Messages.ConnectedToServiceBus, _settings.QueueName));
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(AppConstant.Messages.ConnectionFailed, ex.Message));
                return false;
            }
        }

        public async Task StartReceivingAsync(Func<Group, Task> onMessageReceived)
        {
            if (_processor == null)
            {
                throw new InvalidOperationException("Service Bus processor is not initialized. Call ConnectAsync first.");
            }

            _processor.ProcessMessageAsync += async (args) =>
            {
                try
                {
                    string messageBody = args.Message.Body.ToString();
                    Console.WriteLine(string.Format(AppConstant.Messages.MessageReceived, _settings.QueueName));

                    var group = JsonSerializer.Deserialize<Group>(messageBody);
                    if (group != null)
                    {
                        await onMessageReceived(group);
                        Console.WriteLine(AppConstant.Messages.MessageProcessed);
                    }

                    // Complete the message
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine(string.Format(AppConstant.Messages.ErrorDeserializingMessage, ex.Message));
                    // Dead letter the message if deserialization fails
                    await args.DeadLetterMessageAsync(args.Message, "DeserializationError", ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format(AppConstant.Messages.MessageProcessingFailed, ex.Message));
                    // Abandon the message to retry
                    await args.AbandonMessageAsync(args.Message);
                }
            };

            _processor.ProcessErrorAsync += (args) =>
            {
                Console.WriteLine(string.Format(AppConstant.Messages.UnexpectedError, args.Exception.Message));
                return Task.CompletedTask;
            };

            await _processor.StartProcessingAsync();
            Console.WriteLine(AppConstant.Messages.WaitingForMessages);
        }

        public async Task DisconnectAsync()
        {
            if (_processor != null)
            {
                await _processor.StopProcessingAsync();
                await _processor.DisposeAsync();
                _processor = null;
            }

            if (_client != null)
            {
                await _client.DisposeAsync();
                _client = null;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                DisconnectAsync().GetAwaiter().GetResult();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}