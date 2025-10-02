using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ClientAzureFunction.Models;
using ClientAzureFunction.Configuration;
using ClientAzureFunction.Helpers;

namespace ClientAzureFunction.Functions
{
    public class ServiceBusConsumerFunction
    {
        private readonly ILogger<ServiceBusConsumerFunction> _logger;
        private readonly AppSettings _appSettings;

        public ServiceBusConsumerFunction(ILogger<ServiceBusConsumerFunction> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        [Function("ProcessLdapGroupMessage")]
        public async Task Run([ServiceBusTrigger("%ServiceBusSettings:QueueName%", Connection = "ServiceBusSettings:ConnectionString")] string message)
        {
            try
            {
                _logger.LogInformation(string.Format(AppConstant.Messages.MessageReceived, message));

                if (_appSettings.LogReceivedMessages)
                {
                    _logger.LogInformation("📨 Raw message: {Message}", message);
                }

                // Deserialize the message to Group object
                var group = JsonSerializer.Deserialize<Group>(message);
                if (group == null)
                {
                    _logger.LogError(string.Format(AppConstant.Messages.ErrorDeserializingMessage, "Group object is null"));
                    return;
                }

                // Process the received group
                await ProcessReceivedGroup(group);

                _logger.LogInformation(AppConstant.Messages.MessageProcessed);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, string.Format(AppConstant.Messages.ErrorDeserializingMessage, ex.Message));
                throw; // This will move the message to dead letter queue
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(AppConstant.Messages.MessageProcessingFailed, ex.Message));
                throw; // This will retry the message based on Service Bus configuration
            }
        }

        private async Task ProcessReceivedGroup(Group group)
        {
            try
            {
                // Log the received group for debugging (but not console output in Azure Functions)
                _logger.LogInformation(string.Format(AppConstant.Messages.GroupProcessed, group.GroupName, group.Users.Count));

                // Display group information (this would be useful in local development)
                if (_appSettings.LogReceivedMessages)
                {
                    _logger.LogInformation("📊 Processing Group: {GroupName} with {UserCount} users", 
                        group.GroupName, group.Users.Count);
                    
                    if (group.Users.Count > 0)
                    {
                        var usersList = string.Join(", ", group.Users.Take(5).Select(u => u.DomainId));
                        if (group.Users.Count > 5)
                        {
                            usersList += $" (and {group.Users.Count - 5} more)";
                        }
                        _logger.LogInformation("👥 Users: {Users}", usersList);
                    }
                }

                // Add your custom business logic here
                // For example:
                // - Save to database
                // - Call external APIs
                // - Transform data
                // - Send notifications
                
                await Task.CompletedTask; // Placeholder for async operations
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing group {GroupName}: {Error}", group.GroupName, ex.Message);
                throw;
            }
        }
    }
}