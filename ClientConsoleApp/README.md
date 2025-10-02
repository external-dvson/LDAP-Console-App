# 🔌 Client Console App - Azure Service Bus Consumer

This is an independent console application that consumes LDAP group data from Azure Service Bus queues. It receives messages sent by the LDAP Console App and processes them for display or further processing.

## 📋 **Overview**

The ClientConsoleApp is designed to be deployed on Azure Cloud and consume data from Azure Service Bus queues. It provides a clean separation between LDAP data collection (LDAPConsoleApp) and data consumption/processing (ClientConsoleApp).

## ✨ **Features**

- ✅ **Azure Service Bus Consumer** - Connects to Service Bus queue to receive messages
- ✅ **JSON Message Deserialization** - Parses Group objects from queue messages
- ✅ **Real-time Processing** - Processes messages as they arrive
- ✅ **Error Handling** - Comprehensive error handling with dead letter queue support
- ✅ **Graceful Shutdown** - Handles Ctrl+C for clean application shutdown
- ✅ **Configurable Settings** - Configurable through appsettings.json
- ✅ **Cloud-Ready** - Designed for Azure deployment

## 🏗️ **Architecture**

### **SOLID Principles Applied:**
- **Single Responsibility**: Each class has one clear purpose (consumer, display, configuration)
- **Open/Closed**: Interface-based design allows easy extension
- **Liskov Substitution**: Interfaces can be safely substituted
- **Interface Segregation**: Focused, cohesive interfaces
- **Dependency Inversion**: Dependencies injected via constructor

## 📂 **Project Structure**

```
ClientConsoleApp/
├── Program.cs                           # Application entry point with DI setup
├── AppConstant.cs                       # Application constants and messages
├── Configuration/
│   └── Settings.cs                      # Configuration models
├── Interfaces/
│   └── IServiceBusConsumerService.cs    # Service Bus consumer contract
├── Models/
│   ├── Group.cs                         # Group data model
│   └── User.cs                          # User data model
├── Services/
│   └── ServiceBusConsumerService.cs     # Azure Service Bus consumer implementation
├── Helpers/
│   └── DisplayHelper.cs                 # Display and formatting utilities
└── appsettings.json                     # Application configuration
```

## ⚙️ **Configuration**

### **appsettings.json**
```json
{
  "ServiceBusSettings": {
    "ConnectionString": "Endpoint=sb://your-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key",
    "QueueName": "ldap-data-queue"
  },
  "AppSettings": {
    "MaxRetries": 3,
    "DelayBetweenRetries": 5000,
    "LogReceivedMessages": true,
    "ProcessingTimeout": 30000
  }
}
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

## 🔐 **Security Considerations**

### **For Azure Cloud Deployment:**

1. **Connection String Security**:
   - Use Azure Key Vault to store Service Bus connection strings
   - Avoid storing connection strings in plain text in production
   - Use Managed Identity when possible for authentication

2. **Certificate Management**:
   - Consider using certificate-based authentication for production
   - Implement proper certificate rotation policies

3. **Network Security**:
   - Use private endpoints for Service Bus when deployed in Azure
   - Implement proper network security groups and firewall rules

4. **Access Control**:
   - Use Azure RBAC for fine-grained access control
   - Follow principle of least privilege

### **Example with Azure Key Vault**:
```json
{
  "ServiceBusSettings": {
    "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/ServiceBusConnectionString/)",
    "QueueName": "ldap-data-queue"
  }
}
```

## 🔄 **Message Processing Flow**

1. **Connect**: Establish connection to Azure Service Bus
2. **Listen**: Wait for messages on the configured queue
3. **Receive**: Get Group messages from the queue
4. **Deserialize**: Parse JSON message content to Group objects
5. **Process**: Display group information and user details
6. **Acknowledge**: Complete successfully processed messages
7. **Error Handling**: Dead letter failed messages or retry on transient errors

## 🚀 **Quick Start**

### Prerequisites
- .NET 9.0 runtime
- Access to Azure Service Bus namespace
- Valid connection string and queue name

### Running the Application
```bash
# Navigate to project directory
cd ClientConsoleApp

# Build the project
dotnet build

# Run the application
dotnet run
```

### Running in Azure
1. Deploy as Azure Container Instance
2. Deploy as Azure App Service
3. Deploy as Azure Function (with modifications)

## 🛠️ **Dependencies**

- **.NET 9.0**
- **Microsoft.Extensions.Configuration** (8.0.0)
- **Microsoft.Extensions.DependencyInjection** (8.0.0)
- **Microsoft.Extensions.Options** (8.0.0)
- **Azure.Messaging.ServiceBus** (7.18.2)
- **System.Text.Json** (9.0.0)

## 📊 **Example Output**

```
══════════════════════════════════════════════════════════════════
🔌 CLIENT CONSOLE APP - Azure Service Bus Consumer
══════════════════════════════════════════════════════════════════
📅 Started: 2024-12-19 10:30:15
🖥️  Environment: MyMachine
👤 User: DOMAIN\Username
══════════════════════════════════════════════════════════════════

🚀 Client Console App Started
✅ Connected to Service Bus: ldap-data-queue
⏳ Waiting for messages from LDAP Console App...

📨 Message received from queue: ldap-data-queue

┌────────────────────────────────────────────────────────────────┐
│ 📋 GROUP: IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN                   │
├────────────────────────────────────────────────────────────────┤
│ 👥 Total Users: 5                                             │
├────────────────────────────────────────────────────────────────┤
│ 📋 Users:                                                      │
│    01. DOMAIN\user1                                            │
│    02. DOMAIN\user2                                            │
│    03. DOMAIN\user3                                            │
│    04. DOMAIN\user4                                            │
│    05. DOMAIN\user5                                            │
└────────────────────────────────────────────────────────────────┘

📊 Group 'IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN' processed with 5 users
✅ Message processed successfully
```

## 🤝 **Extending the Application**

### **Adding Custom Processing**:
1. Modify the `ProcessReceivedGroup` method in `Program.cs`
2. Add new services for database storage, API calls, etc.
3. Implement additional interfaces for extensibility

### **Adding New Configuration**:
1. Update the `Settings.cs` file with new configuration options
2. Update `appsettings.json` with new sections
3. Register new configuration in dependency injection

## 📝 **Development Notes**

- **Cross-Platform** - Works on Windows, Linux, and macOS
- **Cloud-Native** - Designed for Azure deployment
- **Modern Architecture** - Follows .NET best practices and SOLID principles
- **Independent** - No dependencies on LDAP Console App for deployment

## 📄 **License**

Internal Bosch tool - for authorized personnel only.

---

**Last Updated**: December 2024  
**Tested Environment**: .NET 9.0, Azure Service Bus  
**Architecture**: Clean Architecture with SOLID Principles