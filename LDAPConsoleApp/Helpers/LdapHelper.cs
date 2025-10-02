using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace LDAPConsoleApp.Helpers
{
    public static class LdapHelper
    {
        public static string BuildLdapPath(string domain)
        {
            return domain.StartsWith(CommonConstant.LdapSchemes.LdapPrefix) 
                ? domain 
                : $"{CommonConstant.LdapSchemes.LdapPrefix}{domain}";
        }

        public static string ExtractCommonNameFromDistinguishedName(string distinguishedName)
        {
            if (string.IsNullOrEmpty(distinguishedName)) 
                return CommonConstant.DefaultValues.Unknown;
            
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
            
            return CommonConstant.DefaultValues.Unknown;
        }

        public static Dictionary<string, object?> CreatePropertyDictionary(SearchResult result, string[] propertiesToLoad)
        {
            var propertyDict = new Dictionary<string, object?>();
            
            foreach (string propertyName in propertiesToLoad)
            {
                if (result.Properties[propertyName].Count > 0)
                {
                    if (result.Properties[propertyName].Count == 1)
                    {
                        propertyDict[propertyName] = result.Properties[propertyName][0];
                    }
                    else
                    {
                        var values = new object[result.Properties[propertyName].Count];
                        for (int i = 0; i < result.Properties[propertyName].Count; i++)
                        {
                            values[i] = result.Properties[propertyName][i];
                        }
                        propertyDict[propertyName] = values;
                    }
                }
            }
            
            return propertyDict;
        }

        public static void SetupDirectorySearcher(DirectorySearcher searcher, string filter, string[] properties, int sizeLimit)
        {
            searcher.Filter = filter;
            searcher.SizeLimit = sizeLimit;
            
            foreach (var property in properties)
            {
                searcher.PropertiesToLoad.Add(property);
            }
        }

        public static List<string> ExtractMemberDistinguishedNames(object? memberData)
        {
            var memberDNs = new List<string>();
            
            if (memberData is object[] memberArray)
            {
                foreach (var memberDN in memberArray)
                {
                    memberDNs.Add(memberDN.ToString() ?? "");
                }
            }
            else if (memberData is string singleMember)
            {
                memberDNs.Add(singleMember);
            }
            
            return memberDNs;
        }
    }
}