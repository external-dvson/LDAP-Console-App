# 🔍 LDAP Console Application

A modern, well-structured LDAP query tool designed for Active Directory operations with Windows Authentication.

## 📋 **Project Overview**

This application connects to Active Directory to query groups and retrieve detailed member information. The codebase has been refactored following modern .NET practices and SOLID principles.

## ✨ **Features**

- ✅ **Windows Authentication** - Uses current user credentials
- ✅ **Cross-domain Support** - Configurable AD domains (APAC, DE, etc.)
- ✅ **Group Member Details** - Retrieves comprehensive user information
- ✅ **Clean Architecture** - SOLID principles implementation
- ✅ **Configuration Management** - Options pattern with appsettings.json
- ✅ **Dependency Injection** - Modern .NET DI container
- ✅ **Error Handling** - Comprehensive exception management

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
LDAPConsoleApp/
├── Program.cs                    # Application entry point with DI setup
├── CommonConstant.cs             # Centralized constants and messages
├── Configuration/
│   └── LdapSettings.cs          # Configuration model
├── Interfaces/
│   └── ILdapService.cs          # LDAP service contract
├── Helpers/
│   ├── LdapHelper.cs            # LDAP utility functions
│   └── DisplayHelper.cs         # UI display utilities
├── LDAPService.cs               # Core LDAP operations implementation
├── TestLDAPServiceWindows.cs    # Test execution logic
└── appsettings.json             # Application configuration
```

## ⚙️ **Configuration**

### **appsettings.json**
```json
{
  "LdapSettings": {
    "Domain": "APAC.bosch.com",
    "SecondaryDomain": "DE.bosch.com",
    "DefaultGroupName": "IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN",
    "MaxResults": 50,
    "MaxGroupResults": 100,
    "MaxDisplayItems": 10
  }
}
```

### **Configuration Options:**
- **Domain**: Primary LDAP domain
- **SecondaryDomain**: Secondary domain for testing
- **DefaultGroupName**: Target group for queries
- **MaxResults**: Maximum search results limit
- **MaxGroupResults**: Maximum group results limit
- **MaxDisplayItems**: Maximum items to display

## 🚀 **Quick Start**

### Prerequisites
- Windows environment with domain access
- .NET 9.0 runtime
- Access to configured AD infrastructure

### Running the Application
```bash
# Navigate to project directory
cd LDAPConsoleApp

# Build the project
dotnet build

# Run the application
dotnet run
```

## 🛠️ **Dependencies**

- **.NET 9.0**
- **System.DirectoryServices** (9.0.9)
- **Microsoft.Extensions.Configuration** (8.0.0)
- **Microsoft.Extensions.DependencyInjection** (8.0.0)
- **Microsoft.Extensions.Options** (8.0.0)

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
- **ILdapService**: Main LDAP operations interface
- **LDAPService**: LDAP operations implementation
- **LDAPTest**: Test execution and orchestration

### **Helper Classes:**
- **LdapHelper**: LDAP utility functions and data processing
- **DisplayHelper**: Console output formatting and display logic
- **CommonConstant**: Centralized constants, messages, and configuration keys

### **Configuration:**
- **LdapSettings**: Strongly-typed configuration model
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
1. Modify `DefaultGroupName` in `appsettings.json`
2. Or inject different group names via configuration

### **Supporting New Domains:**
1. Update `LdapSettings` configuration
2. Add domain-specific logic if needed

### **Adding More User Properties:**
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