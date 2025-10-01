# 🔍 LDAP Group Query Tool

A simple .NET console application to query Active Directory groups and their members using Windows Authentication.

## 📋 **Project Overview**

This tool connects to Active Directory to query specific groups and retrieve detailed member information. It's specifically designed to work with Bosch's DE domain infrastructure.

## ✨ **Features**

- ✅ **Windows Authentication** - Uses current user credentials
- ✅ **Cross-domain Support** - Can connect to different AD domains (APAC, DE, etc.)
- ✅ **Group Member Details** - Retrieves username, display name, email, department
- ✅ **Clean Output** - Minimal, focused results
- ✅ **Error Handling** - Graceful fallback for connection issues

## 🎯 **Current Configuration**

- **Primary Domain**: APAC.bosch.com
- **Target Domain**: DE.bosch.com  
- **Target Group**: `IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN`
- **Authentication**: Windows Integrated (current user credentials)

## 🚀 **Quick Start**

### Prerequisites
- Windows environment with domain access
- .NET 9.0 runtime
- Access to Bosch AD infrastructure

### Running the Application
```bash
# Clone or navigate to project directory
cd LDAP-Test/LDAPConsoleApp

# Build the project
dotnet build

# Run the application
dotnet run
```

## 📂 **Project Structure**

```
LDAP-Test/
├── LDAPConsoleApp/
│   ├── Program.cs                    # Entry point
│   ├── LDAPService.cs               # Core LDAP operations
│   ├── TestLDAPServiceWindows.cs    # Test harness
│   ├── appsettings.json             # Configuration
│   └── LDAPConsoleApp.csproj        # Project file
└── README.md                        # This file
```

## 🔧 **Key Components**

### **LDAPService.cs**
- **ConnectToSpecificDomainQuiet()** - Silent domain connection
- **GetGroupDetails()** - Retrieve group information
- **GetGroupMembers()** - Get detailed member list
- **ShowGroupMembers()** - Display formatted results

### **TestLDAPServiceWindows.cs**
- **TestDEDomainGroup()** - Main test method for DE domain
- **RunTest()** - Entry point for testing

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

## 🔐 **Security & Authentication**

- Uses **Windows Authentication** (current user context)
- **No credentials stored** in code or config files
- Connects using **integrated security**
- Access depends on user's **AD permissions**

## 🌐 **Domain Support**

### **Supported Domains:**
- **APAC.bosch.com** - Asia Pacific region
- **DE.bosch.com** - Germany region
- Can be extended to other Bosch domains

### **Connection Methods:**
```csharp
// Silent connection (no console output)
ldapService.ConnectToSpecificDomainQuiet("DE.bosch.com")

// Verbose connection (with logs)
ldapService.ConnectToSpecificDomain("DE.bosch.com")
```

## ⚙️ **Configuration**

### **appsettings.json**
```json
{
  "Domain": "APAC.bosch.com"
}
```

### **Modifying Target Group**
Edit the group name in `TestLDAPServiceWindows.cs`:
```csharp
TestDEDomainGroup(ldapService, "YOUR_GROUP_NAME_HERE");
```

## 🛠️ **Dependencies**

- **.NET 9.0**
- **System.DirectoryServices** (9.0.9)
- **Microsoft.Extensions.Configuration** (8.0.0)

## 🔍 **Troubleshooting**

### **Common Issues:**

1. **"Group not found"**
   - Verify group name spelling
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
1. Modify `TestDEDomainGroup()` call with new group name
2. Add multiple group queries if needed

### **Supporting New Domains:**
1. Add domain to `TestLDAPServiceWindows.cs`
2. Create new test methods for additional domains

### **Adding More User Properties:**
1. Extend `GetGroupMembers()` method
2. Add properties to `ShowGroupMembers()` display

## 📝 **Development Notes**

- **Windows-only** - Uses DirectoryServices which requires Windows
- **Enterprise-focused** - Designed for Bosch AD infrastructure  
- **Minimal dependencies** - Keeps the tool lightweight
- **Clean architecture** - Separation of concerns between service and presentation

## 🤝 **Contributing**

To modify or extend this tool:
1. Follow existing code patterns
2. Maintain Windows Authentication approach
3. Keep output clean and focused
4. Test with multiple domains when possible

## 📄 **License**

Internal Bosch tool - for authorized personnel only.

---

**Last Updated**: October 2025  
**Tested Environment**: Windows 11, .NET 9.0, Bosch AD Infrastructure