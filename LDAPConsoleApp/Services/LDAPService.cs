using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using LDAPConsoleApp.Interfaces;
using LDAPConsoleApp.Helpers;
using LDAPConsoleApp.Configuration;
using Microsoft.Extensions.Options;

namespace LDAPConsoleApp.Services
{
    public class LDAPService : ILdapService, IDisposable
    {
        private readonly string _domainPath;
        private readonly LdapSettings _settings;
        private DirectoryEntry? _directoryEntry;

        public LDAPService(IOptions<LdapSettings> settings)
        {
            _settings = settings.Value;
            _domainPath = LdapHelper.BuildLdapPath(_settings.Domain);
        }

        public bool Connect()
        {
            try
            {
                _directoryEntry = new DirectoryEntry(_domainPath);
                var name = _directoryEntry.Name;
                
                DisplayHelper.DisplayConnectionSuccess(_directoryEntry.Path, _directoryEntry.Name, _directoryEntry.SchemaClassName);
                
                return true;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayConnectionFailure(ex.Message);
                return false;
            }
        }

        public bool ConnectToSpecificDomain(string domain)
        {
            try
            {
                DisposeExistingConnection();

                string domainPath = LdapHelper.BuildLdapPath(domain);
                _directoryEntry = new DirectoryEntry(domainPath);
                
                var name = _directoryEntry.Name;
                
                DisplayHelper.DisplayConnectionSuccess(_directoryEntry.Path, _directoryEntry.Name, _directoryEntry.SchemaClassName);
                
                return true;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplaySpecificDomainConnectionFailure(domain, ex.Message);
                return false;
            }
        }

        public bool ConnectToSpecificDomainQuiet(string domain)
        {
            try
            {
                DisposeExistingConnection();

                string domainPath = LdapHelper.BuildLdapPath(domain);
                _directoryEntry = new DirectoryEntry(domainPath);
                
                var name = _directoryEntry.Name;
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Dictionary<string, object?>> GetAllGroups(int maxResults = 100)
        {
            var groups = new List<Dictionary<string, object?>>();
            
            if (_directoryEntry == null)
            {
                DisplayHelper.DisplayNotConnectedError();
                return groups;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                var properties = new[] { 
                    CommonConstant.LdapProperties.CommonName,
                    CommonConstant.LdapProperties.DistinguishedName,
                    CommonConstant.LdapProperties.Description,
                    CommonConstant.LdapProperties.DisplayName,
                    CommonConstant.LdapProperties.Member
                };

                LdapHelper.SetupDirectorySearcher(searcher, CommonConstant.LdapFilters.GroupObjectClass, properties, maxResults);

                var results = searcher.FindAll();
                
                foreach (SearchResult result in results)
                {
                    var group = LdapHelper.CreatePropertyDictionary(result, properties);
                    groups.Add(group);
                }
                
                DisplayHelper.DisplayGroupsFound(groups.Count);
                return groups;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError("searching groups", ex.Message);
                return groups;
            }
        }

        public List<Dictionary<string, object?>> GetGroupsByName(string searchPattern, int maxResults = 50)
        {
            var groups = new List<Dictionary<string, object?>>();
            
            if (_directoryEntry == null)
            {
                DisplayHelper.DisplayNotConnectedError();
                return groups;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                var properties = new[] { 
                    CommonConstant.LdapProperties.CommonName,
                    CommonConstant.LdapProperties.DistinguishedName,
                    CommonConstant.LdapProperties.Description,
                    CommonConstant.LdapProperties.DisplayName,
                    CommonConstant.LdapProperties.Member
                };

                var filter = string.Format(CommonConstant.LdapFilters.GroupByName, searchPattern);
                LdapHelper.SetupDirectorySearcher(searcher, filter, properties, maxResults);

                var results = searcher.FindAll();
                
                foreach (SearchResult result in results)
                {
                    var group = LdapHelper.CreatePropertyDictionary(result, properties);
                    groups.Add(group);
                }
                
                DisplayHelper.DisplayGroupsFoundWithPattern(groups.Count, searchPattern);
                return groups;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError("searching groups by name", ex.Message);
                return groups;
            }
        }

        public List<Dictionary<string, object?>> GetGroupsByPrefix(string prefix, int maxResults = 50)
        {
            var groups = new List<Dictionary<string, object?>>();
            
            if (_directoryEntry == null)
            {
                DisplayHelper.DisplayNotConnectedError();
                return groups;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                var properties = new[] { 
                    CommonConstant.LdapProperties.CommonName,
                    CommonConstant.LdapProperties.DistinguishedName,
                    CommonConstant.LdapProperties.Description,
                    CommonConstant.LdapProperties.DisplayName,
                    CommonConstant.LdapProperties.Member
                };

                var filter = string.Format(CommonConstant.LdapFilters.GroupByPrefix, prefix);
                LdapHelper.SetupDirectorySearcher(searcher, filter, properties, maxResults);

                var results = searcher.FindAll();
                
                foreach (SearchResult result in results)
                {
                    var group = LdapHelper.CreatePropertyDictionary(result, properties);
                    groups.Add(group);
                }
                
                DisplayHelper.DisplayGroupsFoundWithPattern(groups.Count, $"{prefix}*");
                return groups;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError("searching groups by prefix", ex.Message);
                return groups;
            }
        }

        public List<Dictionary<string, object?>> SearchUsers(string searchPattern, int maxResults = 50)
        {
            var users = new List<Dictionary<string, object?>>();
            
            if (_directoryEntry == null)
            {
                DisplayHelper.DisplayNotConnectedError();
                return users;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                var properties = new[] { 
                    CommonConstant.LdapProperties.CommonName,
                    CommonConstant.LdapProperties.SamAccountName,
                    CommonConstant.LdapProperties.DisplayName,
                    CommonConstant.LdapProperties.DistinguishedName,
                    CommonConstant.LdapProperties.Mail
                };

                var filter = string.Format(CommonConstant.LdapFilters.UserSearch, searchPattern);
                LdapHelper.SetupDirectorySearcher(searcher, filter, properties, maxResults);

                var results = searcher.FindAll();
                
                foreach (SearchResult result in results)
                {
                    var user = new Dictionary<string, object?>();
                    
                    foreach (string propertyName in properties)
                    {
                        if (result.Properties[propertyName].Count > 0)
                        {
                            user[propertyName] = result.Properties[propertyName][0];
                        }
                    }
                    
                    users.Add(user);
                }
                
                DisplayHelper.DisplayUsersFoundWithPattern(users.Count, searchPattern);
                return users;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError("searching users", ex.Message);
                return users;
            }
        }

        public void ShowGroups(List<Dictionary<string, object?>> groups, int maxDisplay = 10)
        {
            DisplayHelper.DisplayGroups(groups, maxDisplay);
        }

        public Dictionary<string, object?>? GetGroupDetails(string groupName)
        {
            if (_directoryEntry == null)
            {
                DisplayHelper.DisplayNotConnectedError();
                return null;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                var properties = new[] { 
                    CommonConstant.LdapProperties.CommonName,
                    CommonConstant.LdapProperties.DistinguishedName,
                    CommonConstant.LdapProperties.Description,
                    CommonConstant.LdapProperties.DisplayName,
                    CommonConstant.LdapProperties.Member,
                    CommonConstant.LdapProperties.MemberOf,
                    CommonConstant.LdapProperties.GroupType
                };

                var filter = string.Format(CommonConstant.LdapFilters.GroupByName, groupName);
                LdapHelper.SetupDirectorySearcher(searcher, filter, properties, 1);

                var result = searcher.FindOne();
                
                if (result != null)
                {
                    return LdapHelper.CreatePropertyDictionary(result, properties);
                }
                else
                {
                    Console.WriteLine(string.Format(CommonConstant.Messages.GroupNotFound, groupName));
                    return null;
                }
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError("getting group details", ex.Message);
                return null;
            }
        }

        public List<Dictionary<string, object?>> GetGroupMembers(string groupName)
        {
            var members = new List<Dictionary<string, object?>>();
            
            var groupDetails = GetGroupDetails(groupName);
            if (groupDetails == null || !groupDetails.ContainsKey(CommonConstant.LdapProperties.Member))
            {
                Console.WriteLine(string.Format(CommonConstant.Messages.NoMembersFoundInGroup, groupName));
                return members;
            }

            try
            {
                var memberDNs = LdapHelper.ExtractMemberDistinguishedNames(groupDetails[CommonConstant.LdapProperties.Member]);

                foreach (var memberDN in memberDNs)
                {
                    if (string.IsNullOrEmpty(memberDN)) continue;

                    try
                    {
                        var member = GetMemberDetails(memberDN);
                        members.Add(member);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format(CommonConstant.Messages.CouldNotGetMemberDetails, memberDN, ex.Message));
                        members.Add(CreateBasicMemberInfo(memberDN));
                    }
                }

                return members;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError("getting group members", ex.Message);
                return members;
            }
        }

        public void ShowGroupDetails(Dictionary<string, object?> group)
        {
            DisplayHelper.DisplayGroupDetails(group);
        }

        public void ShowGroupMembers(List<Dictionary<string, object?>> members)
        {
            DisplayHelper.DisplayGroupMembers(members);
        }

        public void Disconnect()
        {
            _directoryEntry?.Dispose();
            _directoryEntry = null;
        }

        public void Dispose()
        {
            Disconnect();
        }

        private void DisposeExistingConnection()
        {
            if (_directoryEntry != null)
            {
                _directoryEntry.Dispose();
                _directoryEntry = null;
            }
        }

        private Dictionary<string, object?> GetMemberDetails(string memberDN)
        {
            using var memberEntry = new DirectoryEntry($"{CommonConstant.LdapSchemes.LdapPrefix}{memberDN}");
            var member = new Dictionary<string, object?>();
            
            var properties = new[] {
                CommonConstant.LdapProperties.CommonName,
                CommonConstant.LdapProperties.SamAccountName,
                CommonConstant.LdapProperties.DisplayName,
                CommonConstant.LdapProperties.Mail,
                CommonConstant.LdapProperties.Title,
                CommonConstant.LdapProperties.Department
            };

            foreach (var property in properties)
            {
                if (memberEntry.Properties.Contains(property))
                    member[property] = memberEntry.Properties[property].Value;
            }
            
            member[CommonConstant.LdapProperties.DistinguishedName] = memberDN;
            return member;
        }

        private Dictionary<string, object?> CreateBasicMemberInfo(string memberDN)
        {
            return new Dictionary<string, object?> 
            { 
                [CommonConstant.LdapProperties.DistinguishedName] = memberDN,
                [CommonConstant.LdapProperties.CommonName] = LdapHelper.ExtractCommonNameFromDistinguishedName(memberDN)
            };
        }
    }
}