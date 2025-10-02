using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using LDAPConsoleApp.Configuration;
using LDAPConsoleApp.Interfaces;
using LDAPConsoleApp.Models;

namespace LDAPConsoleApp.Services
{
    public class LdapServiceBusOrchestrator : ILdapServiceBusOrchestrator
    {
        private readonly ILdapService _ldapService;
        private readonly IServiceBusService _serviceBusService;
        private readonly LdapSettings _settings;

        public LdapServiceBusOrchestrator(
            ILdapService ldapService,
            IServiceBusService serviceBusService,
            IOptions<LdapSettings> settings)
        {
            _ldapService = ldapService;
            _serviceBusService = serviceBusService;
            _settings = settings.Value;
        }

        public async Task ProcessGroupsAndSendToServiceBusAsync()
        {
            Console.WriteLine(CommonConstant.Messages.ProcessingGroupsForServiceBus);

            // Connect to Service Bus
            if (!await _serviceBusService.ConnectAsync())
            {
                Console.WriteLine("❌ Failed to connect to Service Bus. Aborting operation.");
                return;
            }

            try
            {
                // Connect to LDAP domain
                if (!_ldapService.ConnectToSpecificDomainQuiet(_settings.SecondaryDomain))
                {
                    Console.WriteLine($"❌ Failed to connect to LDAP domain: {_settings.SecondaryDomain}");
                    return;
                }

                // Process each group from configuration
                foreach (var groupName in _settings.GroupNames)
                {
                    await ProcessGroupAndSendToServiceBusAsync(groupName);
                }

                // Also process groups by prefix to discover additional groups
                await ProcessGroupsByPrefixAndSendToServiceBusAsync(_settings.GroupPrefix);
            }
            finally
            {
                await _serviceBusService.DisconnectAsync();
                _ldapService.Disconnect();
            }
        }

        private async Task ProcessGroupAndSendToServiceBusAsync(string groupName)
        {
            try
            {
                // Get group details
                var groupDetails = _ldapService.GetGroupDetails(groupName);
                if (groupDetails == null)
                {
                    Console.WriteLine($"⚠️ Group not found: {groupName}");
                    return;
                }

                // Get group members
                var members = _ldapService.GetGroupMembers(groupName);
                
                // Create Group model
                var group = new Group
                {
                    GroupName = groupName,
                    Users = ConvertMembersToUsers(members)
                };

                Console.WriteLine(string.Format(CommonConstant.Messages.SendingGroupToServiceBus, group.GroupName, group.Users.Count));

                // Send to Service Bus
                await _serviceBusService.SendGroupMessageAsync(group);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing group '{groupName}': {ex.Message}");
            }
        }

        private async Task ProcessGroupsByPrefixAndSendToServiceBusAsync(string prefix)
        {
            try
            {
                var groups = _ldapService.GetGroupsByPrefix(prefix, _settings.MaxGroupResults);
                
                foreach (var groupData in groups)
                {
                    var groupName = GetGroupNameFromData(groupData);
                    if (!string.IsNullOrEmpty(groupName) && !_settings.GroupNames.Contains(groupName))
                    {
                        await ProcessGroupAndSendToServiceBusAsync(groupName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing groups by prefix '{prefix}': {ex.Message}");
            }
        }

        private List<User> ConvertMembersToUsers(List<Dictionary<string, object?>> members)
        {
            var users = new List<User>();

            foreach (var member in members)
            {
                var domainId = GetDomainIdFromMember(member);
                if (!string.IsNullOrEmpty(domainId))
                {
                    users.Add(new User { DomainId = domainId });
                }
            }

            return users;
        }

        private string GetDomainIdFromMember(Dictionary<string, object?> member)
        {
            // Try to get domain ID from various LDAP properties
            if (member.TryGetValue(CommonConstant.LdapProperties.SamAccountName, out var samAccountName) && samAccountName != null)
            {
                return samAccountName.ToString() ?? string.Empty;
            }

            if (member.TryGetValue(CommonConstant.LdapProperties.CommonName, out var commonName) && commonName != null)
            {
                return commonName.ToString() ?? string.Empty;
            }

            if (member.TryGetValue(CommonConstant.LdapProperties.DistinguishedName, out var distinguishedName) && distinguishedName != null)
            {
                // Extract the CN part from the distinguished name
                var dn = distinguishedName.ToString() ?? string.Empty;
                var cnStart = dn.IndexOf("CN=", StringComparison.OrdinalIgnoreCase);
                if (cnStart >= 0)
                {
                    var cnEnd = dn.IndexOf(',', cnStart);
                    if (cnEnd > cnStart)
                    {
                        return dn.Substring(cnStart + 3, cnEnd - cnStart - 3);
                    }
                }
            }

            return string.Empty;
        }

        private string GetGroupNameFromData(Dictionary<string, object?> groupData)
        {
            if (groupData.TryGetValue(CommonConstant.LdapProperties.CommonName, out var commonName) && commonName != null)
            {
                return commonName.ToString() ?? string.Empty;
            }

            return string.Empty;
        }
    }
}