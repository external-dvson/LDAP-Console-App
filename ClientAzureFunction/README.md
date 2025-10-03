# 🔥 Client Azure Function - Azure Service Bus Consumer (.NET 8 Isolated)

## 📋 **Overview**

The ClientAzureFunction is an Azure Function (.NET 8 Isolated) variant of the ClientConsoleApp designed to consume LDAP data from Azure Service Bus queues using Service Bus triggers. It provides serverless, event-driven processing of LDAP group data.

## ✨ **Features**

- ✅ **Azure Function with Service Bus Trigger** - Automatically triggered when messages arrive in the queue
- ✅ **JSON Message Deserialization** - Parses Group objects from queue messages
- ✅ **Event-driven Processing** - Serverless execution model with automatic scaling
- ✅ **Error Handling** - Comprehensive error handling with dead letter queue support
- ✅ **Configurable Settings** - Configurable through Azure App Settings or local.settings.json
- ✅ **Cloud-Native** - Designed for Azure deployment with optimized resource usage
- ✅ **.NET 8 Isolated** - Uses the latest Azure Functions runtime with .NET 8

## 🛠️ **Dependencies**

- **.NET 8.0**
- **Microsoft.Azure.Functions.Worker** (1.21.0)
- **Microsoft.Azure.Functions.Worker.Sdk** (1.16.4)
- **Microsoft.Azure.Functions.Worker.Extensions.ServiceBus** (5.16.0)
- **Microsoft.Extensions.Hosting** (8.0.0)
- **Microsoft.Extensions.Configuration** (8.0.0)
- **Microsoft.Extensions.DependencyInjection** (8.0.0)
- **Azure.Messaging.ServiceBus** (7.18.2)
- **System.Text.Json** (9.0.0)

## 📂 **Project Structure**

```
ClientAzureFunction/
├── Program.cs                           # Azure Function startup and DI setup
├── AppConstant.cs                       # Application constants and messages
├── host.json                            # Azure Functions host configuration
├── local.settings.json                  # Local development settings (not deployed)
├── Configuration/
│   └── Settings.cs                      # Configuration models
├── Functions/
│   └── ServiceBusConsumerFunction.cs    # Service Bus triggered function
├── Models/
│   ├── Group.cs                         # Group data model
│   └── User.cs                          # User data model
└── Helpers/
    └── DisplayHelper.cs                 # Display and formatting utilities
```

## ⚙️ **Configuration**

### **local.settings.json** (Local Development)
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ServiceBusSettings__ConnectionString": "Endpoint=sb://your-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key",
    "ServiceBusSettings__QueueName": "ldap-data-queue",
    "AppSettings__MaxRetries": "3",
    "AppSettings__DelayBetweenRetries": "5000",
    "AppSettings__LogReceivedMessages": "true",
    "AppSettings__ProcessingTimeout": "30000"
  }
}
```

### **Azure App Settings** (Production Deployment)
```
ServiceBusSettings__ConnectionString = Endpoint=sb://your-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key
ServiceBusSettings__QueueName = ldap-data-queue
AppSettings__MaxRetries = 3
AppSettings__DelayBetweenRetries = 5000
AppSettings__LogReceivedMessages = true
AppSettings__ProcessingTimeout = 30000
```

### **Configuration Options:**

**Service Bus Settings:**
- **ConnectionString**: Azure Service Bus connection string
- **QueueName**: Name of the queue to consume messages from

**App Settings:**
- **MaxRetries**: Maximum number of retry attempts for failed messages
- **DelayBetweenRetries**: Delay between retry attempts (milliseconds)
- **LogReceivedMessages**: Enable/disable message logging
- **ProcessingTimeout**: Timeout for message processing (milliseconds)

## 🚀 **Quick Start**

### Prerequisites
- .NET 8.0 SDK
- Azure Functions Core Tools (for local development)
- Access to Azure Service Bus namespace
- Azure Storage account (for local development)

### Local Development
```bash
# Navigate to Azure Function project directory
cd ClientAzureFunction

# Install Azure Functions Core Tools (if not installed)
npm install -g azure-functions-core-tools@4 --unsafe-perm true

# Build the project
dotnet build

# Run the Azure Function locally
func start
```

### Deployment to Azure
```bash
# Build the project
dotnet build

# Deploy to Azure Function App
func azure functionapp publish <your-function-app-name>
```

## 🏗️ **Architecture**

### **Azure Functions Isolated Model:**
- **Isolated Process**: Function runs in a separate .NET worker process
- **Service Bus Trigger**: Automatically triggered when messages arrive in the queue
- **Dependency Injection**: Built-in DI container for configuration and services
- **Logging**: Integrated with Azure Application Insights and console logging

### **SOLID Principles Applied:**
- **Single Responsibility**: Each class has one clear purpose (function, configuration, models)
- **Open/Closed**: Interface-based design allows easy extension
- **Liskov Substitution**: Interfaces can be safely substituted
- **Interface Segregation**: Focused, cohesive interfaces
- **Dependency Inversion**: Dependencies injected via constructor

## 🔄 **Message Processing Flow**

1. **Message Arrival**: Service Bus trigger automatically invokes the function when a message arrives
2. **Deserialization**: JSON message is deserialized to Group object
3. **Processing**: Business logic processes the group data
4. **Logging**: Detailed logging for monitoring and debugging
5. **Error Handling**: Failures trigger retries or dead letter queue processing

## 🤝 **Extending the Application**

### **Adding Custom Processing**:
1. Modify the `ProcessReceivedGroup` method in `ServiceBusConsumerFunction.cs`
2. Add new services for database storage, API calls, etc.
3. Register new services in `Program.cs`

### **Adding New Configuration**:
1. Update the `Settings.cs` file with new configuration options
2. Update `local.settings.json` and Azure App Settings
3. Register new configuration in dependency injection

## 🆚 **Comparison with Console App**

| Feature | Console App | Azure Function |
|---------|-------------|----------------|
| **Execution Model** | Continuous running | Event-driven, serverless |
| **Scaling** | Manual scaling | Automatic scaling |
| **Cost** | Fixed compute cost | Pay-per-execution |
| **Deployment** | VM or container | Azure Function App |
| **Monitoring** | Custom logging | Built-in Application Insights |
| **Message Processing** | Polling-based | Trigger-based |

## 📝 **Development Notes**

- **Serverless**: No need to manage infrastructure or long-running processes
- **Event-driven**: Functions only execute when messages are available
- **Cost-effective**: Pay only for actual execution time
- **Auto-scaling**: Automatically scales based on queue load
- **Monitoring**: Built-in integration with Azure Monitor and Application Insights

## 📄 **License**

Internal Bosch tool - for authorized personnel only.

---

**Last Updated**: December 2024  
**Runtime**: Azure Functions v4, .NET 8 Isolated  
**Architecture**: Serverless, Event-driven with SOLID Principles