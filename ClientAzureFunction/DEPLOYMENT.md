# 🚀 Azure Function Deployment Guide

## Prerequisites

- Azure subscription with appropriate permissions
- Azure Service Bus namespace and queue already configured
- Azure CLI or Azure PowerShell installed
- .NET 8.0 SDK installed

## Deployment Steps

### 1. Create Azure Function App

Using Azure CLI:
```bash
# Create resource group (if not exists)
az group create --name rg-ldap-consumer --location "East US"

# Create storage account (required for Azure Functions)
az storage account create \
  --name salldapconsumer \
  --resource-group rg-ldap-consumer \
  --location "East US" \
  --sku Standard_LRS

# Create Function App
az functionapp create \
  --resource-group rg-ldap-consumer \
  --consumption-plan-location "East US" \
  --runtime dotnet-isolated \
  --functions-version 4 \
  --name func-ldap-consumer \
  --storage-account salldapconsumer
```

### 2. Configure Application Settings

```bash
az functionapp config appsettings set \
  --name func-ldap-consumer \
  --resource-group rg-ldap-consumer \
  --settings \
    "ServiceBusSettings__ConnectionString=Endpoint=sb://your-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key" \
    "ServiceBusSettings__QueueName=ldap-data-queue" \
    "AppSettings__MaxRetries=3" \
    "AppSettings__DelayBetweenRetries=5000" \
    "AppSettings__LogReceivedMessages=true" \
    "AppSettings__ProcessingTimeout=30000"
```

### 3. Deploy the Function

```bash
# Navigate to the Azure Function project
cd ClientAzureFunction

# Deploy to Azure
func azure functionapp publish func-ldap-consumer
```

### 4. Verify Deployment

Check the deployment in the Azure portal:
- Navigate to your Function App
- Go to "Functions" section
- Verify "ProcessLdapGroupMessage" function is listed
- Check "Monitor" tab for execution logs

## Environment-Specific Configuration

### Development
Use `local.settings.json` for local development with Azure Storage Emulator.

### Production
Configure App Settings directly in Azure portal or via Azure CLI as shown above.

## Monitoring and Troubleshooting

### Application Insights
Enable Application Insights for detailed monitoring:
```bash
az functionapp config appsettings set \
  --name func-ldap-consumer \
  --resource-group rg-ldap-consumer \
  --settings "APPINSIGHTS_INSTRUMENTATIONKEY=your-instrumentation-key"
```

### Log Analysis
- Use Azure portal "Monitor" section
- Check Application Insights for detailed traces
- Monitor Service Bus queue metrics for message flow

## Security Best Practices

1. **Use Managed Identity** instead of connection strings when possible
2. **Key Vault Integration** for sensitive configuration
3. **Network Restrictions** to limit access to the Function App
4. **RBAC** for appropriate access control

## Scaling Configuration

The Function App will auto-scale based on queue load. You can configure:
- Maximum scale-out limit
- Pre-warmed instances
- Always On setting (for Premium plans)

For more information, see the [Azure Functions documentation](https://docs.microsoft.com/en-us/azure/azure-functions/).