using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace LDAPConsoleApp
{
    public class LDAPService
    {
        private readonly string _domainPath;
        private DirectoryEntry? _directoryEntry;

        public LDAPService(string domain = "APAC.bosch.com")
        {
            _domainPath = $"LDAP://{domain}";
        }

        public bool Connect()
        {
            try
            {
                // Sử dụng Windows Authentication (current user credentials)
                _directoryEntry = new DirectoryEntry(_domainPath);
                
                // Test connection bằng cách truy cập một property
                var name = _directoryEntry.Name;
                
                Console.WriteLine($"✅ Connected to: {_directoryEntry.Path}");
                Console.WriteLine($"   Name: {_directoryEntry.Name}");
                Console.WriteLine($"   Schema: {_directoryEntry.SchemaClassName}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection failed: {ex.Message}");
                return false;
            }
        }

        public bool ConnectToSpecificDomain(string domain)
        {
            try
            {
                // Dispose existing connection if any
                if (_directoryEntry != null)
                {
                    _directoryEntry.Dispose();
                    _directoryEntry = null;
                }

                // Create LDAP path for specific domain
                string domainPath = domain.StartsWith("LDAP://") ? domain : $"LDAP://{domain}";
                
                // Sử dụng Windows Authentication (current user credentials)
                _directoryEntry = new DirectoryEntry(domainPath);
                
                // Test connection bằng cách truy cập một property
                var name = _directoryEntry.Name;
                
                Console.WriteLine($"✅ Connected to: {_directoryEntry.Path}");
                Console.WriteLine($"   Name: {_directoryEntry.Name}");
                Console.WriteLine($"   Schema: {_directoryEntry.SchemaClassName}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection to {domain} failed: {ex.Message}");
                return false;
            }
        }

        public bool ConnectToSpecificDomainQuiet(string domain)
        {
            try
            {
                // Dispose existing connection if any
                if (_directoryEntry != null)
                {
                    _directoryEntry.Dispose();
                    _directoryEntry = null;
                }

                // Create LDAP path for specific domain
                string domainPath = domain.StartsWith("LDAP://") ? domain : $"LDAP://{domain}";
                
                // Sử dụng Windows Authentication (current user credentials)
                _directoryEntry = new DirectoryEntry(domainPath);
                
                // Test connection bằng cách truy cập một property (silent)
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
                Console.WriteLine("❌ Not connected to LDAP server");
                return groups;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                searcher.Filter = "(objectClass=group)";
                searcher.PropertiesToLoad.Add("cn");
                searcher.PropertiesToLoad.Add("distinguishedName");
                searcher.PropertiesToLoad.Add("description");
                searcher.PropertiesToLoad.Add("displayName");
                searcher.PropertiesToLoad.Add("member");
                searcher.SizeLimit = maxResults;

                var results = searcher.FindAll();
                
                foreach (SearchResult result in results)
                {
                    var group = new Dictionary<string, object?>();
                    
                    foreach (string propertyName in searcher.PropertiesToLoad)
                    {
                        if (result.Properties[propertyName].Count > 0)
                        {
                            if (result.Properties[propertyName].Count == 1)
                            {
                                group[propertyName] = result.Properties[propertyName][0];
                            }
                            else
                            {
                                // Multiple values - convert to array
                                var values = new object[result.Properties[propertyName].Count];
                                for (int i = 0; i < result.Properties[propertyName].Count; i++)
                                {
                                    values[i] = result.Properties[propertyName][i];
                                }
                                group[propertyName] = values;
                            }
                        }
                    }
                    
                    groups.Add(group);
                }
                
                Console.WriteLine($"✅ Found {groups.Count} groups");
                return groups;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error searching groups: {ex.Message}");
                return groups;
            }
        }

        public List<Dictionary<string, object?>> GetGroupsByName(string searchPattern, int maxResults = 50)
        {
            var groups = new List<Dictionary<string, object?>>();
            
            if (_directoryEntry == null)
            {
                Console.WriteLine("❌ Not connected to LDAP server");
                return groups;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                searcher.Filter = $"(&(objectClass=group)(cn={searchPattern}))";
                searcher.PropertiesToLoad.Add("cn");
                searcher.PropertiesToLoad.Add("distinguishedName");
                searcher.PropertiesToLoad.Add("description");
                searcher.PropertiesToLoad.Add("displayName");
                searcher.PropertiesToLoad.Add("member");
                searcher.SizeLimit = maxResults;

                var results = searcher.FindAll();
                
                foreach (SearchResult result in results)
                {
                    var group = new Dictionary<string, object?>();
                    
                    foreach (string propertyName in searcher.PropertiesToLoad)
                    {
                        if (result.Properties[propertyName].Count > 0)
                        {
                            if (result.Properties[propertyName].Count == 1)
                            {
                                group[propertyName] = result.Properties[propertyName][0];
                            }
                            else
                            {
                                var values = new object[result.Properties[propertyName].Count];
                                for (int i = 0; i < result.Properties[propertyName].Count; i++)
                                {
                                    values[i] = result.Properties[propertyName][i];
                                }
                                group[propertyName] = values;
                            }
                        }
                    }
                    
                    groups.Add(group);
                }
                
                Console.WriteLine($"✅ Found {groups.Count} groups matching '{searchPattern}'");
                return groups;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error searching groups by name: {ex.Message}");
                return groups;
            }
        }

        public List<Dictionary<string, object?>> SearchUsers(string searchPattern, int maxResults = 50)
        {
            var users = new List<Dictionary<string, object?>>();
            
            if (_directoryEntry == null)
            {
                Console.WriteLine("❌ Not connected to LDAP server");
                return users;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                searcher.Filter = $"(&(objectClass=user)(objectCategory=person)(|(cn={searchPattern})(sAMAccountName={searchPattern})(displayName={searchPattern})))";
                searcher.PropertiesToLoad.Add("cn");
                searcher.PropertiesToLoad.Add("sAMAccountName");
                searcher.PropertiesToLoad.Add("displayName");
                searcher.PropertiesToLoad.Add("distinguishedName");
                searcher.PropertiesToLoad.Add("mail");
                searcher.SizeLimit = maxResults;

                var results = searcher.FindAll();
                
                foreach (SearchResult result in results)
                {
                    var user = new Dictionary<string, object?>();
                    
                    foreach (string propertyName in searcher.PropertiesToLoad)
                    {
                        if (result.Properties[propertyName].Count > 0)
                        {
                            user[propertyName] = result.Properties[propertyName][0];
                        }
                    }
                    
                    users.Add(user);
                }
                
                Console.WriteLine($"✅ Found {users.Count} users matching '{searchPattern}'");
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error searching users: {ex.Message}");
                return users;
            }
        }

        public void ShowGroups(List<Dictionary<string, object?>> groups, int maxDisplay = 10)
        {
            if (!groups.Any())
            {
                Console.WriteLine("❌ No groups to display");
                return;
            }

            Console.WriteLine($"\n📋 Displaying {Math.Min(groups.Count, maxDisplay)} groups:");
            
            foreach (var group in groups.Take(maxDisplay))
            {
                var cn = group.GetValueOrDefault("cn", "N/A")?.ToString() ?? "N/A";
                var description = group.GetValueOrDefault("description", "")?.ToString() ?? "";
                
                Console.WriteLine($"  📂 {cn}");
                
                if (!string.IsNullOrEmpty(description))
                {
                    Console.WriteLine($"     📝 {description}");
                }
                
                // Show member count if available
                if (group.ContainsKey("member"))
                {
                    var members = group["member"];
                    if (members is object[] memberArray)
                    {
                        Console.WriteLine($"     👤 Members: {memberArray.Length}");
                    }
                    else if (members != null)
                    {
                        Console.WriteLine($"     👤 Members: 1");
                    }
                }
                
                Console.WriteLine();
            }
            
            if (groups.Count > maxDisplay)
            {
                Console.WriteLine($"  ... và {groups.Count - maxDisplay} groups khác");
            }
        }

        public Dictionary<string, object?>? GetGroupDetails(string groupName)
        {
            if (_directoryEntry == null)
            {
                Console.WriteLine("❌ Not connected to LDAP server");
                return null;
            }

            try
            {
                using var searcher = new DirectorySearcher(_directoryEntry);
                searcher.Filter = $"(&(objectClass=group)(cn={groupName}))";
                searcher.PropertiesToLoad.Add("cn");
                searcher.PropertiesToLoad.Add("distinguishedName");
                searcher.PropertiesToLoad.Add("description");
                searcher.PropertiesToLoad.Add("displayName");
                searcher.PropertiesToLoad.Add("member");
                searcher.PropertiesToLoad.Add("memberOf");
                searcher.PropertiesToLoad.Add("groupType");
                searcher.SizeLimit = 1;

                var result = searcher.FindOne();
                
                if (result != null)
                {
                    var group = new Dictionary<string, object?>();
                    
                    foreach (string propertyName in searcher.PropertiesToLoad)
                    {
                        if (result.Properties[propertyName].Count > 0)
                        {
                            if (result.Properties[propertyName].Count == 1)
                            {
                                group[propertyName] = result.Properties[propertyName][0];
                            }
                            else
                            {
                                var values = new object[result.Properties[propertyName].Count];
                                for (int i = 0; i < result.Properties[propertyName].Count; i++)
                                {
                                    values[i] = result.Properties[propertyName][i];
                                }
                                group[propertyName] = values;
                            }
                        }
                    }
                    
                    return group;
                }
                else
                {
                    Console.WriteLine($"❌ Group not found: {groupName}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting group details: {ex.Message}");
                return null;
            }
        }

        public List<Dictionary<string, object?>> GetGroupMembers(string groupName)
        {
            var members = new List<Dictionary<string, object?>>();
            
            var groupDetails = GetGroupDetails(groupName);
            if (groupDetails == null || !groupDetails.ContainsKey("member"))
            {
                Console.WriteLine($"❌ No members found in group: {groupName}");
                return members;
            }

            try
            {
                var memberDNs = new List<string>();
                
                if (groupDetails["member"] is object[] memberArray)
                {
                    foreach (var memberDN in memberArray)
                    {
                        memberDNs.Add(memberDN.ToString() ?? "");
                    }
                }
                else if (groupDetails["member"] is string singleMember)
                {
                    memberDNs.Add(singleMember);
                }

                foreach (var memberDN in memberDNs)
                {
                    if (string.IsNullOrEmpty(memberDN)) continue;

                    try
                    {
                        using var memberEntry = new DirectoryEntry($"LDAP://{memberDN}");
                        var member = new Dictionary<string, object?>();
                        
                        // Get user properties
                        if (memberEntry.Properties.Contains("cn"))
                            member["cn"] = memberEntry.Properties["cn"].Value;
                        if (memberEntry.Properties.Contains("sAMAccountName"))
                            member["sAMAccountName"] = memberEntry.Properties["sAMAccountName"].Value;
                        if (memberEntry.Properties.Contains("displayName"))
                            member["displayName"] = memberEntry.Properties["displayName"].Value;
                        if (memberEntry.Properties.Contains("mail"))
                            member["mail"] = memberEntry.Properties["mail"].Value;
                        if (memberEntry.Properties.Contains("title"))
                            member["title"] = memberEntry.Properties["title"].Value;
                        if (memberEntry.Properties.Contains("department"))
                            member["department"] = memberEntry.Properties["department"].Value;
                        
                        member["distinguishedName"] = memberDN;
                        members.Add(member);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Could not get details for member: {memberDN} - {ex.Message}");
                        // Add basic info even if we can't get full details
                        members.Add(new Dictionary<string, object?> 
                        { 
                            ["distinguishedName"] = memberDN,
                            ["cn"] = ExtractCNFromDN(memberDN)
                        });
                    }
                }

                return members;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting group members: {ex.Message}");
                return members;
            }
        }

        private string ExtractCNFromDN(string distinguishedName)
        {
            if (string.IsNullOrEmpty(distinguishedName)) return "Unknown";
            
            var cnIndex = distinguishedName.IndexOf("CN=", StringComparison.OrdinalIgnoreCase);
            if (cnIndex >= 0)
            {
                var start = cnIndex + 3;
                var end = distinguishedName.IndexOf(',', start);
                if (end > start)
                {
                    return distinguishedName.Substring(start, end - start);
                }
                else
                {
                    return distinguishedName.Substring(start);
                }
            }
            
            return "Unknown";
        }

        public void ShowGroupDetails(Dictionary<string, object?> group)
        {
            if (group == null)
            {
                Console.WriteLine("❌ No group details to display");
                return;
            }

            var cn = group.GetValueOrDefault("cn", "N/A")?.ToString() ?? "N/A";
            var dn = group.GetValueOrDefault("distinguishedName", "")?.ToString() ?? "";
            var description = group.GetValueOrDefault("description", "")?.ToString() ?? "";
            var displayName = group.GetValueOrDefault("displayName", "")?.ToString() ?? "";
            
            Console.WriteLine($"\n📂 Group Details: {cn}");
            Console.WriteLine($"   🔗 Distinguished Name: {dn}");
            
            if (!string.IsNullOrEmpty(displayName) && displayName != cn)
            {
                Console.WriteLine($"   📛 Display Name: {displayName}");
            }
            
            if (!string.IsNullOrEmpty(description))
            {
                Console.WriteLine($"   📝 Description: {description}");
            }

            if (group.ContainsKey("member"))
            {
                var members = group["member"];
                if (members is object[] memberArray)
                {
                    Console.WriteLine($"   👥 Total Members: {memberArray.Length}");
                }
                else if (members != null)
                {
                    Console.WriteLine($"   👥 Total Members: 1");
                }
            }
            else
            {
                Console.WriteLine($"   👥 Total Members: 0");
            }
        }

        public void ShowGroupMembers(List<Dictionary<string, object?>> members)
        {
            if (!members.Any())
            {
                Console.WriteLine("❌ No members to display");
                return;
            }

            Console.WriteLine($"\n👥 Group Members ({members.Count}):");
            
            foreach (var member in members)
            {
                var cn = member.GetValueOrDefault("cn", "N/A")?.ToString() ?? "N/A";
                var sAMAccountName = member.GetValueOrDefault("sAMAccountName", "")?.ToString() ?? "";
                var displayName = member.GetValueOrDefault("displayName", "")?.ToString() ?? "";
                var mail = member.GetValueOrDefault("mail", "")?.ToString() ?? "";
                var title = member.GetValueOrDefault("title", "")?.ToString() ?? "";
                var department = member.GetValueOrDefault("department", "")?.ToString() ?? "";
                
                Console.WriteLine($"  👤 {cn}");
                
                if (!string.IsNullOrEmpty(sAMAccountName))
                {
                    Console.WriteLine($"     🆔 Username: {sAMAccountName}");
                }
                
                if (!string.IsNullOrEmpty(displayName) && displayName != cn)
                {
                    Console.WriteLine($"     📛 Display Name: {displayName}");
                }
                
                if (!string.IsNullOrEmpty(mail))
                {
                    Console.WriteLine($"     📧 Email: {mail}");
                }
                
                if (!string.IsNullOrEmpty(title))
                {
                    Console.WriteLine($"     💼 Title: {title}");
                }
                
                if (!string.IsNullOrEmpty(department))
                {
                    Console.WriteLine($"     🏢 Department: {department}");
                }
                
                Console.WriteLine();
            }
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
    }
}