using System;
using System.Collections.Generic;
using System.Linq;

namespace LDAPConsoleApp.Helpers
{
    public static class DisplayHelper
    {
        public static void DisplayConnectionSuccess(string path, string name, string schema)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.ConnectedTo, path));
            Console.WriteLine(string.Format(CommonConstant.Messages.ConnectionName, name));
            Console.WriteLine(string.Format(CommonConstant.Messages.ConnectionSchema, schema));
        }

        public static void DisplayConnectionFailure(string message)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.ConnectionFailed, message));
        }

        public static void DisplaySpecificDomainConnectionFailure(string domain, string message)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.ConnectionToSpecificDomainFailed, domain, message));
        }

        public static void DisplayNotConnectedError()
        {
            Console.WriteLine(CommonConstant.Messages.NotConnectedToLdapServer);
        }

        public static void DisplayGroupsFound(int count)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.FoundGroups, count));
        }

        public static void DisplayGroupsFoundWithPattern(int count, string pattern)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.FoundGroupsMatching, count, pattern));
        }

        public static void DisplayUsersFoundWithPattern(int count, string pattern)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.FoundUsersMatching, count, pattern));
        }

        public static void DisplayError(string operation, string message)
        {
            var errorMessage = operation switch
            {
                "searching groups" => CommonConstant.Messages.ErrorSearchingGroups,
                "searching groups by name" => CommonConstant.Messages.ErrorSearchingGroupsByName,
                "searching groups by prefix" => CommonConstant.Messages.ErrorSearchingGroupsByPrefix,
                "searching users" => CommonConstant.Messages.ErrorSearchingUsers,
                "getting group details" => CommonConstant.Messages.ErrorGettingGroupDetails,
                "getting group members" => CommonConstant.Messages.ErrorGettingGroupMembers,
                _ => "❌ Error {0}: {1}"
            };
            
            Console.WriteLine(string.Format(errorMessage, operation, message));
        }

        public static void DisplayGroups(List<Dictionary<string, object?>> groups, int maxDisplay)
        {
            if (!groups.Any())
            {
                Console.WriteLine(CommonConstant.Messages.NoGroupsToDisplay);
                return;
            }

            Console.WriteLine(string.Format(CommonConstant.Messages.DisplayingGroups, Math.Min(groups.Count, maxDisplay)));
            
            foreach (var group in groups.Take(maxDisplay))
            {
                var cn = GetValueOrDefault(group, CommonConstant.LdapProperties.CommonName);
                var description = GetValueOrDefault(group, CommonConstant.LdapProperties.Description);
                
                Console.WriteLine($"  {CommonConstant.Icons.Group} {cn}");
                
                if (!string.IsNullOrEmpty(description))
                {
                    Console.WriteLine($"     {CommonConstant.Icons.Description} {description}");
                }
                
                DisplayMemberCount(group);
                Console.WriteLine();
            }
            
            if (groups.Count > maxDisplay)
            {
                Console.WriteLine(string.Format(CommonConstant.Messages.AndMoreGroups, groups.Count - maxDisplay));
            }
        }

        public static void DisplayGroupDetails(Dictionary<string, object?> group)
        {
            if (group == null)
            {
                Console.WriteLine(CommonConstant.Messages.NoGroupDetailsToDisplay);
                return;
            }

            var cn = GetValueOrDefault(group, CommonConstant.LdapProperties.CommonName);
            var dn = GetValueOrDefault(group, CommonConstant.LdapProperties.DistinguishedName);
            var description = GetValueOrDefault(group, CommonConstant.LdapProperties.Description);
            var displayName = GetValueOrDefault(group, CommonConstant.LdapProperties.DisplayName);
            
            Console.WriteLine($"\n{CommonConstant.Icons.Group} Group Details: {cn}");
            Console.WriteLine($"   {CommonConstant.Icons.Link} Distinguished Name: {dn}");
            
            if (!string.IsNullOrEmpty(displayName) && displayName != cn)
            {
                Console.WriteLine($"   {CommonConstant.Icons.Name} Display Name: {displayName}");
            }
            
            if (!string.IsNullOrEmpty(description))
            {
                Console.WriteLine($"   {CommonConstant.Icons.Description} Description: {description}");
            }

            DisplayMemberCount(group);
        }

        public static void DisplayGroupMembers(List<Dictionary<string, object?>> members)
        {
            if (!members.Any())
            {
                Console.WriteLine(CommonConstant.Messages.NoMembersToDisplay);
                return;
            }

            Console.WriteLine(string.Format(CommonConstant.Messages.GroupMembers, members.Count));
            
            foreach (var member in members)
            {
                var cn = GetValueOrDefault(member, CommonConstant.LdapProperties.CommonName);
                var sAMAccountName = GetValueOrDefault(member, CommonConstant.LdapProperties.SamAccountName);
                var displayName = GetValueOrDefault(member, CommonConstant.LdapProperties.DisplayName);
                var mail = GetValueOrDefault(member, CommonConstant.LdapProperties.Mail);
                var title = GetValueOrDefault(member, CommonConstant.LdapProperties.Title);
                var department = GetValueOrDefault(member, CommonConstant.LdapProperties.Department);
                
                Console.WriteLine($"  {CommonConstant.Icons.User} {cn}");
                
                if (!string.IsNullOrEmpty(sAMAccountName))
                {
                    Console.WriteLine($"     {CommonConstant.Icons.Id} Username: {sAMAccountName}");
                }
                
                if (!string.IsNullOrEmpty(displayName) && displayName != cn)
                {
                    Console.WriteLine($"     {CommonConstant.Icons.Name} Display Name: {displayName}");
                }
                
                if (!string.IsNullOrEmpty(mail))
                {
                    Console.WriteLine($"     {CommonConstant.Icons.Email} Email: {mail}");
                }
                
                if (!string.IsNullOrEmpty(title))
                {
                    Console.WriteLine($"     {CommonConstant.Icons.Title} Title: {title}");
                }
                
                if (!string.IsNullOrEmpty(department))
                {
                    Console.WriteLine($"     {CommonConstant.Icons.Department} Department: {department}");
                }
                
                Console.WriteLine();
            }
        }

        public static void DisplayApplicationHeader(string domain, string userDomain, string userName)
        {
            Console.WriteLine(CommonConstant.Messages.ApplicationTitle);
            Console.WriteLine(string.Format(CommonConstant.Messages.DomainLabel, domain));
            Console.WriteLine(string.Format(CommonConstant.Messages.CurrentUserLabel, userDomain, userName));
            Console.WriteLine();
        }

        public static void DisplayGroupNotFoundInDomain(string domain, string groupName)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.GroupNotFoundInDomain, domain, groupName));
        }

        public static void DisplayCouldNotConnectToDomain(string domain)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.CouldNotConnectToDomain, domain));
        }

        public static void DisplayDomainTestError(string domain, string message)
        {
            Console.WriteLine(string.Format(CommonConstant.Messages.ErrorTestingDomain, domain, message));
        }

        private static string GetValueOrDefault(Dictionary<string, object?> dict, string key)
        {
            return dict.GetValueOrDefault(key, CommonConstant.DefaultValues.NotAvailable)?.ToString() 
                ?? CommonConstant.DefaultValues.NotAvailable;
        }

        private static void DisplayMemberCount(Dictionary<string, object?> group)
        {
            if (group.ContainsKey(CommonConstant.LdapProperties.Member))
            {
                var members = group[CommonConstant.LdapProperties.Member];
                if (members is object[] memberArray)
                {
                    Console.WriteLine($"     {CommonConstant.Icons.Members} Total Members: {memberArray.Length}");
                }
                else if (members != null)
                {
                    Console.WriteLine($"     {CommonConstant.Icons.Members} Total Members: 1");
                }
            }
            else
            {
                Console.WriteLine($"     {CommonConstant.Icons.Members} Total Members: 0");
            }
        }
    }
}