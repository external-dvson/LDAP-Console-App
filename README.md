# 🔍 LDAP Console Application & Client Console App

A modern, well-structured LDAP query tool designed for Active Directory operations with Windows Authentication and an independent Azure Service Bus consumer application.

## 📋 **Project Overview**

This solution contains two applications:
1. **LDAPConsoleApp** - Connects to Active Directory to query groups and send data to Azure Service Bus
2. **ClientConsoleApp** - Independent consumer that receives and processes LDAP data from Azure Service Bus

The codebase has been refactored following modern .NET practices and SOLID principles.

## ✨ **Features**

### **LDAP Console App:**
- ✅ **Windows Authentication** - Uses current user credentials
- ✅ **Cross-domain Support** - Configurable AD domains (APAC, DE, etc.)
- ✅ **Group Member Details** - Retrieves comprehensive user information
- ✅ **Azure Service Bus Integration** - Send LDAP data to Azure Service Bus queue

### **Client Console App:**
- ✅ **Azure Service Bus Consumer** - Receives LDAP data from queue
- ✅ **Cloud-Ready** - Designed for Azure deployment
- ✅ **Real-time Processing** - Processes messages as they arrive
- ✅ **Independent Deployment** - No LDAP dependencies

### **Common Features:**
- ✅ **Clean Architecture** - SOLID principles implementation
- ✅ **Configuration Management** - Options pattern with appsettings.json
- ✅ **Dependency Injection** - Modern .NET DI container
- ✅ **Error Handling** - Comprehensive exception management
- ✅ **Group/User Data Models** - Structured data models for group and user information

## 🏗️ **Architecture & Design Principles**

### **SOLID Principles Applied:**
- **Single Responsibility**: Each class has one clear purpose
- **Open/Closed**: Interface-based design allows easy extension
- **Liskov Substitution**: Interfaces can be safely substituted
- **Interface Segregation**: Focused, cohesive interfaces
- **Dependency Inversion**: Dependencies injected via constructor

### **Additional Principles:**
- **DRY (Don't Repeat Yourself)**: Common functionality extracted to helpers
- **KISS (Keep It Simple, Stupid)**: Clean, readable code structure

## 📂 **Project Structure**

```
LDAP-Console-App/
├── LDAPConsoleApp/                  # LDAP Console Application
│   ├── Program.cs                   # Application entry point with DI setup
│   ├── CommonConstant.cs            # Centralized constants and messages
│   ├── Configuration/
│   │   └── LdapSettings.cs         # Configuration model (LDAP + Service Bus)
│   ├── Interfaces/
│   │   ├── ILdapService.cs         # LDAP service contract
│   │   ├── IServiceBusService.cs   # Service Bus service contract
│   │   └── ILdapServiceBusOrchestrator.cs  # Orchestrator contract
│   ├── Models/
│   │   ├── Group.cs                # Group data model (GroupName + Users)
│   │   └── User.cs                 # User data model (DomainId)
│   ├── Services/
│   │   ├── ServiceBusService.cs    # Azure Service Bus implementation
│   │   └── LdapServiceBusOrchestrator.cs  # LDAP to Service Bus orchestration
│   ├── Helpers/
│   │   ├── LdapHelper.cs           # LDAP utility functions
│   │   └── DisplayHelper.cs        # UI display utilities
│   ├── LDAPService.cs              # Core LDAP operations implementation
│   ├── TestLDAPServiceWindows.cs   # Test execution logic
│   └── appsettings.json            # Application configuration
├── ClientConsoleApp/               # Client Console Application (NEW)
│   ├── Program.cs                  # Consumer app entry point
│   ├── AppConstant.cs              # Application constants
│   ├── Configuration/
│   │   └── Settings.cs             # Service Bus and app settings
│   ├── Interfaces/
│   │   └── IServiceBusConsumerService.cs  # Consumer service contract
│   ├── Models/
│   │   ├── Group.cs                # Group data model (same as LDAP app)
│   │   └── User.cs                 # User data model (same as LDAP app)
│   ├── Services/
│   │   └── ServiceBusConsumerService.cs   # Service Bus consumer implementation
│   ├── Helpers/
│   │   └── DisplayHelper.cs        # Display utilities for received data
│   ├── appsettings.json            # Consumer app configuration
│   └── README.md                   # Client app documentation
└── LDAP-Test.sln                   # Solution file (contains both projects)
```

## ⚙️ **Configuration**

### **LDAP Console App - appsettings.json**
```json
{
  "LdapSettings": {
    "Domain": "APAC.bosch.com",
    "SecondaryDomain": "DE.bosch.com",
    "GroupNames": [
      "IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN",
      "IdM2BCD_FCMCONSOLE_USER_ADMIN",
      "IdM2BCD_FCMCONSOLE_READ_ONLY"
    ],
    "GroupPrefix": "IdM2BCD_FCMCONSOLE_",
    "MaxResults": 50,
    "MaxGroupResults": 100,
    "MaxDisplayItems": 10
  },
  "ServiceBusSettings": {
    "ConnectionString": "Endpoint=sb://your-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key",
    "QueueName": "ldap-data-queue"
  }
}
```

### **Client Console App - appsettings.json**
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
**LDAP Settings:**
- **Domain**: Primary LDAP domain
- **SecondaryDomain**: Secondary domain for testing
- **GroupNames**: Array of specific group names to query
- **GroupPrefix**: Prefix to search for all related groups (e.g., FCMConsole modules)
- **MaxResults**: Maximum search results limit
- **MaxGroupResults**: Maximum group results limit
- **MaxDisplayItems**: Maximum items to display

**Service Bus Settings:**
- **ConnectionString**: Azure Service Bus connection string
- **QueueName**: Name of the queue to send LDAP data to

### **Enhanced Multi-Group Support:**
The application now supports querying multiple groups in two ways:
1. **Specific Groups**: Configure exact group names in `GroupNames` array
2. **Prefix-based Discovery**: Use `GroupPrefix` to find all groups with a common prefix (ideal for discovering all FCMConsole module groups)

## 🚀 **Quick Start**

### Prerequisites
- .NET 9.0 runtime
- Access to Azure Service Bus namespace (for both applications)
- Windows environment with domain access (for LDAP Console App only)

### Running the LDAP Console App (Data Producer)
```bash
# Navigate to LDAP project directory
cd LDAPConsoleApp

# Build the project
dotnet build

# Run the application
dotnet run
```

### Running the Client Console App (Data Consumer)
```bash
# Navigate to Client project directory
cd ClientConsoleApp

# Build the project
dotnet build

# Run the application
dotnet run
```

### Building Both Applications
```bash
# Build entire solution
dotnet build

# Or build specific projects
dotnet build LDAPConsoleApp/LDAPConsoleApp.csproj
dotnet build ClientConsoleApp/ClientConsoleApp.csproj
```

## 🛠️ **Dependencies**

- **.NET 9.0**
- **System.DirectoryServices** (9.0.9)
- **Microsoft.Extensions.Configuration** (8.0.0)
- **Microsoft.Extensions.DependencyInjection** (8.0.0)
- **Microsoft.Extensions.Options** (8.0.0)
- **Azure.Messaging.ServiceBus** (7.18.2)

## 🔄 **Azure Service Bus Integration**

### **Data Models:**
- **Group**: Contains `GroupName` and collection of `User` objects (1-to-many relationship)
- **User**: Contains `DomainId` field for user identification

### **Service Bus Workflow:**
1. **LDAP Query**: Retrieve groups and their members from configured AD domains
2. **Data Transformation**: Convert LDAP data to Group/User model objects
3. **Message Creation**: Serialize groups with their users to JSON format
4. **Queue Delivery**: Send structured messages to Azure Service Bus queue

### **Message Format:**
```json
{
  "messageType": "Group",
  "timestamp": "2024-12-19T10:00:00Z",
  "data": {
    "groupName": "IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN",
    "users": [
      {
        "domainId": "user1"
      },
      {
        "domainId": "user2"
      }
    ]
  }
}
```

### **Service Configuration:**
Configure your Azure Service Bus settings in `appsettings.json`:
- Update `ConnectionString` with your Service Bus namespace connection string
- Set `QueueName` to your target queue name

## 🔐 **Security & Authentication**

- Uses **Windows Authentication** (current user context)
- **No credentials stored** in code or config files
- Connects using **integrated security**
- Access depends on user's **AD permissions**

## 🌐 **Domain Support**

### **Supported Domains:**
- **APAC.bosch.com** - Asia Pacific region
- **DE.bosch.com** - Germany region
- Easily configurable for other domains

### **Connection Methods:**
```csharp
// Silent connection (no console output)
ldapService.ConnectToSpecificDomainQuiet("DE.bosch.com")

// Verbose connection (with logs)
ldapService.ConnectToSpecificDomain("DE.bosch.com")
```

## 🔧 **Key Components**

### **Core Services:**
- **ILdapService**: Main LDAP operations interface with enhanced group query methods
  - `GetGroupsByName()`: Query specific group by exact name
  - `GetGroupsByPrefix()`: Discover all groups with common prefix
  - `GetGroupDetails()` & `GetGroupMembers()`: Detailed group information
- **LDAPService**: LDAP operations implementation
- **LDAPTest**: Test execution and orchestration with multi-group support

### **Service Bus Services:**
- **IServiceBusService**: Azure Service Bus operations interface
  - `ConnectAsync()`: Establish connection to Service Bus
  - `SendGroupMessageAsync()`: Send group data as JSON message to queue
  - `DisconnectAsync()`: Clean up Service Bus resources
- **ServiceBusService**: Service Bus operations implementation
- **ILdapServiceBusOrchestrator**: Orchestration interface for LDAP to Service Bus workflow
- **LdapServiceBusOrchestrator**: Coordinates LDAP queries and Service Bus messaging

### **Enhanced Group Query Methods:**
```csharp
// Query specific groups by name
var groups = ldapService.GetGroupsByName("IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN");

// Discover all groups with prefix (ideal for FCMConsole modules)
var allGroups = ldapService.GetGroupsByPrefix("IdM2BCD_FCMCONSOLE_");
```

### **Helper Classes:**
- **LdapHelper**: LDAP utility functions and data processing
- **DisplayHelper**: Console output formatting and display logic
- **CommonConstant**: Centralized constants, messages, and configuration keys

### **Configuration:**
- **LdapSettings**: Strongly-typed configuration model with multi-group support
- **ServiceBusSettings**: Configuration for Azure Service Bus connection and queue
- **Options Pattern**: Dependency injection of configuration

## 📊 **Sample Output**

```
👥 Group Members (2):
  👤 DAS6HC
     🆔 Username: DAS6HC
     📛 Display Name: EXTERNAL Dang Viet Son (Amaris, SX/EIT-MM-TM-OS)
     📧 Email: external.Son.DangViet@vn.bosch.com
     🏢 Department: SX/EIT-MM-TM-OS

  👤 GYG8HC
     🆔 Username: GYG8HC
     📛 Display Name: EXTERNAL Nguyen Ngoc Manh (CMC, BGSV/BDO)
     📧 Email: external.NgocManh.Nguyen@vn.bosch.com
     🏢 Department: BGSV/BDO
```

## 🔍 **Troubleshooting**

### **Common Issues:**

1. **"Group not found"**
   - Verify group name in configuration
   - Check if group exists in target domain
   - Ensure user has read permissions

2. **"Could not connect to domain"**
   - Verify network connectivity to domain
   - Check user has domain access
   - Try different domain controller

3. **"Access denied"**
   - User lacks AD read permissions
   - Group may be in restricted OU
   - Contact AD administrator

### **Testing Connectivity:**
```bash
# Test network connectivity
ping DE.bosch.com

# Test LDAP port
telnet DE.bosch.com 389
```

## 📈 **Extending the Tool**

### **Adding New Groups:**
1. **Specific Groups**: Add to `GroupNames` array in `appsettings.json`
   ```json
   "GroupNames": [
     "IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN",
     "IdM2BCD_FCMCONSOLE_USER_ADMIN",
     "IdM2BCD_FCMCONSOLE_YOUR_NEW_MODULE"
   ]
   ```
2. **Prefix-based Discovery**: Update `GroupPrefix` to discover related groups automatically
   ```json
   "GroupPrefix": "IdM2BCD_FCMCONSOLE_"
   ```

### **Supporting New Domains:**
1. Update `LdapSettings` configuration
2. Add domain-specific logic if needed

### **Adding More User Properties:**
1. Extend `LdapHelper.GetMemberDetails()` method
2. Update `DisplayHelper.DisplayGroupMembers()` display logic

### **Multi-Module Support:**
The application now supports enterprise scenarios with multiple FCMConsole modules:
- **Transport Module**: `IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN`
- **User Management**: `IdM2BCD_FCMCONSOLE_USER_ADMIN`  
- **Read-Only Access**: `IdM2BCD_FCMCONSOLE_READ_ONLY`
- **Automatic Discovery**: Use prefix search to find all related groups
1. Extend `LdapHelper.GetMemberDetails()` method
2. Update `DisplayHelper.DisplayGroupMembers()` display logic

## 🎯 **Current Configuration**

- **Primary Domain**: APAC.bosch.com
- **Target Domain**: DE.bosch.com  
- **Target Group**: `IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN`
- **Authentication**: Windows Integrated (current user credentials)

## 📝 **Development Notes**

- **Windows-only** - Uses DirectoryServices which requires Windows
- **Enterprise-focused** - Designed for Bosch AD infrastructure  
- **Modern Architecture** - Follows .NET best practices and SOLID principles
- **Dependency Injection** - Uses Microsoft.Extensions.DependencyInjection
- **Options Pattern** - Configuration management following .NET standards

## 🤝 **Contributing**

To modify or extend this tool:
1. Follow SOLID principles and existing patterns
2. Maintain Windows Authentication approach
3. Use dependency injection for new services
4. Add constants to `CommonConstant.cs`
5. Test with multiple domains when possible

## 📄 **License**

Internal Bosch tool - for authorized personnel only.

---

**Last Updated**: December 2024  
**Tested Environment**: Windows 11, .NET 9.0, Bosch AD Infrastructure  
**Architecture**: Clean Architecture with SOLID Principles