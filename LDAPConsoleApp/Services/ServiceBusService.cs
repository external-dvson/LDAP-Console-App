using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using LDAPConsoleApp.Configuration;
using LDAPConsoleApp.Interfaces;
using LDAPConsoleApp.Models;
using LDAPConsoleApp.Helpers;

namespace LDAPConsoleApp.Services
{
    public class ServiceBusService : IServiceBusService, IDisposable
    {
        private readonly ServiceBusSettings _settings;
        private ServiceBusClient? _client;
        private ServiceBusSender? _sender;
        private bool _disposed = false;

        public ServiceBusService(IOptions<ServiceBusSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.ConnectionString) || string.IsNullOrEmpty(_settings.QueueName))
                {
                    DisplayHelper.DisplayError("Service Bus Connection", "Connection string or queue name is not configured");
                    return false;
                }

                _client = new ServiceBusClient(_settings.ConnectionString);
                _sender = _client.CreateSender(_settings.QueueName);

                Console.WriteLine(string.Format(CommonConstant.Messages.ServiceBusConnectionSuccess, _settings.QueueName));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(CommonConstant.Messages.ServiceBusConnectionFailed, ex.Message));
                return false;
            }
        }

        public async Task<bool> SendGroupMessageAsync(Group group)
        {
            if (_sender == null)
            {
                DisplayHelper.DisplayError("Service Bus", "Not connected to Service Bus");
                return false;
            }

            try
            {
                var messageData = new
                {
                    MessageType = "Group",
                    Timestamp = DateTime.UtcNow,
                    Data = group
                };

                var jsonMessage = JsonSerializer.Serialize(messageData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                var message = new ServiceBusMessage(jsonMessage)
                {
                    ContentType = "application/json",
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = $"Group-{group.GroupName}"
                };

                await _sender.SendMessageAsync(message);

                Console.WriteLine(string.Format(CommonConstant.Messages.MessageSentToQueue, _settings.QueueName, $"Group '{group.GroupName}' with {group.Users.Count} users"));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(CommonConstant.Messages.ErrorSendingMessage, ex.Message));
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_sender != null)
            {
                await _sender.DisposeAsync();
                _sender = null;
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