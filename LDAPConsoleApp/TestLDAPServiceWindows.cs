using System;
using Microsoft.Extensions.Options;
using LDAPConsoleApp.Interfaces;
using LDAPConsoleApp.Configuration;
using LDAPConsoleApp.Helpers;

namespace LDAPConsoleApp
{
    public class LDAPTest
    {
        private readonly ILdapService _ldapService;
        private readonly LdapSettings _settings;

        public LDAPTest(ILdapService ldapService, IOptions<LdapSettings> settings)
        {
            _ldapService = ldapService;
            _settings = settings.Value;
        }

        public void RunTest()
        {
            // Test individual groups from configuration
            foreach (var groupName in _settings.GroupNames)
            {
                Console.WriteLine($"\n=== Testing Individual Group: {groupName} ===");
                TestDomainGroup(_settings.SecondaryDomain, groupName);
            }

            // Test groups by prefix to discover all FCMConsole groups
            Console.WriteLine($"\n=== Testing Groups by Prefix: {_settings.GroupPrefix} ===");
            TestGroupsByPrefix(_settings.SecondaryDomain, _settings.GroupPrefix);
        }

        public void TestDomainGroup(string domain, string groupName)
        {
            try
            {
                var deLdapService = new LDAPService();
                
                if (deLdapService.ConnectToSpecificDomainQuiet(domain))
                {
                    var groupDetails = deLdapService.GetGroupDetails(groupName);
                    if (groupDetails != null)
                    {
                        var members = deLdapService.GetGroupMembers(groupName);
                        deLdapService.ShowGroupMembers(members);
                    }
                    else
                    {
                        DisplayHelper.DisplayGroupNotFoundInDomain(domain, groupName);
                    }
                    
                    deLdapService.Disconnect();
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

        public void TestGroupsByPrefix(string domain, string prefix)
        {
            try
            {
                var deLdapService = new LDAPService();
                
                if (deLdapService.ConnectToSpecificDomainQuiet(domain))
                {
                    Console.WriteLine($"Searching for groups with prefix '{prefix}' in domain '{domain}'...");
                    var groups = deLdapService.GetGroupsByPrefix(prefix, _settings.MaxGroupResults);
                    
                    if (groups.Count > 0)
                    {
                        Console.WriteLine($"\nFound {groups.Count} groups with prefix '{prefix}':");
                        deLdapService.ShowGroups(groups, _settings.MaxDisplayItems);
                        
                        // Show members for the first group as an example
                        if (groups.Count > 0)
                        {
                            var firstGroup = groups[0];
                            var firstGroupName = firstGroup.GetValueOrDefault(CommonConstant.LdapProperties.CommonName)?.ToString();
                            if (!string.IsNullOrEmpty(firstGroupName))
                            {
                                Console.WriteLine($"\n=== Example: Members of {firstGroupName} ===");
                                var members = deLdapService.GetGroupMembers(firstGroupName);
                                deLdapService.ShowGroupMembers(members);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No groups found with prefix '{prefix}' in domain '{domain}'");
                    }
                    
                    deLdapService.Disconnect();
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