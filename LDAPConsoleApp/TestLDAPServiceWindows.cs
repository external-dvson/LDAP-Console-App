using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using LDAPConsoleApp.Interfaces;
using LDAPConsoleApp.Configuration;
using LDAPConsoleApp.Helpers;

namespace LDAPConsoleApp
{
    public class LDAPTest
    {
        private readonly ILdapService _ldapService;
        private readonly ILdapServiceBusOrchestrator _serviceBusOrchestrator;
        private readonly LdapSettings _settings;
        private static readonly object _ldapLock = new object();

        public LDAPTest(ILdapService ldapService, ILdapServiceBusOrchestrator serviceBusOrchestrator, IOptions<LdapSettings> settings)
        {
            _ldapService = ldapService;
            _serviceBusOrchestrator = serviceBusOrchestrator;
            _settings = settings.Value;
        }

        public void RunTest()
        {
            // Option 1: Async Parallel (current implementation)
            Console.WriteLine("🔄 Testing individual groups with async parallel...");
            RunTestAsync().GetAwaiter().GetResult();

            // Option 2: Send LDAP data to Service Bus
            Console.WriteLine("\n🚌 Sending LDAP data to Azure Service Bus...");
            RunServiceBusTest().GetAwaiter().GetResult();
        }

        public async Task RunServiceBusTest()
        {
            try
            {
                await _serviceBusOrchestrator.ProcessGroupsAndSendToServiceBusAsync();
                Console.WriteLine("✅ Service Bus operations completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Service Bus operation failed: {ex.Message}");
            }
        }

        public async Task RunTestAsync()
        {
            // Test individual groups from configuration in parallel
            var groupTasks = _settings.GroupNames.Select(async groupName =>
            {
                Console.WriteLine($"\n=== Testing Individual Group: {groupName} ===");
                await TestDomainGroupAsync(_settings.SecondaryDomain, groupName);
            });

            await Task.WhenAll(groupTasks);

            // Test groups by prefix to discover all FCMConsole groups
            Console.WriteLine($"\n=== Testing Groups by Prefix: {_settings.GroupPrefix} ===");
            await TestGroupsByPrefixAsync(_settings.SecondaryDomain, _settings.GroupPrefix);
        }

        // Alternative: Parallel.ForEach approach for comparison
        public void RunTestParallel()
        {
            Console.WriteLine("🔄 Testing individual groups with Parallel.ForEach...");
            
            // Test individual groups from configuration in parallel
            System.Threading.Tasks.Parallel.ForEach(_settings.GroupNames, groupName =>
            {
                Console.WriteLine($"\n=== Testing Individual Group: {groupName} ===");
                TestDomainGroupSync(_settings.SecondaryDomain, groupName);
            });

            // Test groups by prefix to discover all FCMConsole groups
            Console.WriteLine($"\n=== Testing Groups by Prefix: {_settings.GroupPrefix} ===");
            TestGroupsByPrefixSync(_settings.SecondaryDomain, _settings.GroupPrefix);
        }

        public async Task TestDomainGroupAsync(string domain, string groupName)
        {
            await Task.Run(() =>
            {
                lock (_ldapLock) // Synchronize LDAP operations
                {
                    try
                    {
                        if (_ldapService.ConnectToSpecificDomainQuiet(domain))
                        {
                            var groupDetails = _ldapService.GetGroupDetails(groupName);
                            if (groupDetails != null)
                            {
                                var members = _ldapService.GetGroupMembers(groupName);
                                _ldapService.ShowGroupMembers(members);
                            }
                            else
                            {
                                DisplayHelper.DisplayGroupNotFoundInDomain(domain, groupName);
                            }
                            
                            _ldapService.Disconnect();
                        }
                        else
                        {
                            DisplayHelper.DisplayCouldNotConnectToDomain(domain);
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayHelper.DisplayDomainTestError(domain, ex.Message);
                    }
                }
            });
        }

        public async Task TestGroupsByPrefixAsync(string domain, string prefix)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (_ldapService.ConnectToSpecificDomainQuiet(domain))
                    {
                        Console.WriteLine($"Searching for groups with prefix '{prefix}' in domain '{domain}'...");
                        var groups = _ldapService.GetGroupsByPrefix(prefix, _settings.MaxGroupResults);
                        
                        if (groups.Count > 0)
                        {
                            Console.WriteLine($"\nFound {groups.Count} groups with prefix '{prefix}':");
                            _ldapService.ShowGroups(groups, _settings.MaxDisplayItems);
                            
                            // Show members for the first group as an example
                            if (groups.Count > 0)
                            {
                                var firstGroup = groups[0];
                                var firstGroupName = firstGroup.GetValueOrDefault(CommonConstant.LdapProperties.CommonName)?.ToString();
                                if (!string.IsNullOrEmpty(firstGroupName))
                                {
                                    Console.WriteLine($"\n=== Example: Members of {firstGroupName} ===");
                                    var members = _ldapService.GetGroupMembers(firstGroupName);
                                    _ldapService.ShowGroupMembers(members);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No groups found with prefix '{prefix}' in domain '{domain}'");
                        }
                        
                        _ldapService.Disconnect();
                    }
                    else
                    {
                        DisplayHelper.DisplayCouldNotConnectToDomain(domain);
                    }
                }
                catch (Exception ex)
                {
                    DisplayHelper.DisplayDomainTestError(domain, ex.Message);
                }
            });
        }

        public void TestDomainGroupSync(string domain, string groupName)
        {
            lock (_ldapLock) // Synchronize LDAP operations for thread safety
            {
                try
                {
                    if (_ldapService.ConnectToSpecificDomainQuiet(domain))
                    {
                        var groupDetails = _ldapService.GetGroupDetails(groupName);
                        if (groupDetails != null)
                        {
                            var members = _ldapService.GetGroupMembers(groupName);
                            _ldapService.ShowGroupMembers(members);
                        }
                        else
                        {
                            DisplayHelper.DisplayGroupNotFoundInDomain(domain, groupName);
                        }
                        
                        _ldapService.Disconnect();
                    }
                    else
                    {
                        DisplayHelper.DisplayCouldNotConnectToDomain(domain);
                    }
                }
                catch (Exception ex)
                {
                    DisplayHelper.DisplayDomainTestError(domain, ex.Message);
                }
            }
        }

        public void TestGroupsByPrefixSync(string domain, string prefix)
        {
            lock (_ldapLock) // Synchronize LDAP operations for thread safety
            {
                try
                {
                    if (_ldapService.ConnectToSpecificDomainQuiet(domain))
                    {
                        Console.WriteLine($"Searching for groups with prefix '{prefix}' in domain '{domain}'...");
                        var groups = _ldapService.GetGroupsByPrefix(prefix, _settings.MaxGroupResults);
                        
                        if (groups.Count > 0)
                        {
                            Console.WriteLine($"\nFound {groups.Count} groups with prefix '{prefix}':");
                            _ldapService.ShowGroups(groups, _settings.MaxDisplayItems);
                            
                            // Show members for the first group as an example
                            if (groups.Count > 0)
                            {
                                var firstGroup = groups[0];
                                var firstGroupName = firstGroup.GetValueOrDefault(CommonConstant.LdapProperties.CommonName)?.ToString();
                                if (!string.IsNullOrEmpty(firstGroupName))
                                {
                                    Console.WriteLine($"\n=== Example: Members of {firstGroupName} ===");
                                    var members = _ldapService.GetGroupMembers(firstGroupName);
                                    _ldapService.ShowGroupMembers(members);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No groups found with prefix '{prefix}' in domain '{domain}'");
                        }
                        
                        _ldapService.Disconnect();
                    }
                    else
                    {
                        DisplayHelper.DisplayCouldNotConnectToDomain(domain);
                    }
                }
                catch (Exception ex)
                {
                    DisplayHelper.DisplayDomainTestError(domain, ex.Message);
                }
            }
        }
    }
}