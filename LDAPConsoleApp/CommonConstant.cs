namespace LDAPConsoleApp
{
    public static class CommonConstant
    {
        public static class Messages
        {
            public const string ConnectedTo = "✅ Connected to: {0}";
            public const string ConnectionName = "   Name: {0}";
            public const string ConnectionSchema = "   Schema: {0}";
            public const string ConnectionFailed = "❌ Connection failed: {0}";
            public const string ConnectionToSpecificDomainFailed = "❌ Connection to {0} failed: {1}";
            public const string NotConnectedToLdapServer = "❌ Not connected to LDAP server";
            public const string FoundGroups = "✅ Found {0} groups";
            public const string FoundGroupsMatching = "✅ Found {0} groups matching '{1}'";
            public const string FoundUsersMatching = "✅ Found {0} users matching '{1}'";
            public const string ErrorSearchingGroups = "❌ Error searching groups: {0}";
            public const string ErrorSearchingGroupsByName = "❌ Error searching groups by name: {0}";
            public const string ErrorSearchingUsers = "❌ Error searching users: {0}";
            public const string NoGroupsToDisplay = "❌ No groups to display";
            public const string DisplayingGroups = "\n📋 Displaying {0} groups:";
            public const string GroupNotFound = "❌ Group not found: {0}";
            public const string ErrorGettingGroupDetails = "❌ Error getting group details: {0}";
            public const string NoMembersFoundInGroup = "❌ No members found in group: {0}";
            public const string CouldNotGetMemberDetails = "⚠️ Could not get details for member: {0} - {1}";
            public const string ErrorGettingGroupMembers = "❌ Error getting group members: {0}";
            public const string NoGroupDetailsToDisplay = "❌ No group details to display";
            public const string NoMembersToDisplay = "❌ No members to display";
            public const string GroupMembers = "\n👥 Group Members ({0}):";
            public const string GroupNotFoundInDomain = "❌ Group not found in {0} domain: {1}";
            public const string CouldNotConnectToDomain = "❌ Could not connect to {0} domain";
            public const string ErrorTestingDomain = "❌ Error testing {0} domain: {1}";
            public const string ApplicationTitle = " LDAP Console Application với Windows Authentication";
            public const string DomainLabel = "   Domain: {0}";
            public const string CurrentUserLabel = "   Current User: {0}\\{1}";
            public const string AndMoreGroups = "  ... và {0} groups khác";
        }

        public static class Icons
        {
            public const string Success = "✅";
            public const string Error = "❌";
            public const string Warning = "⚠️";
            public const string Group = "📂";
            public const string Description = "📝";
            public const string Members = "👤";
            public const string User = "👤";
            public const string Users = "👥";
            public const string Link = "🔗";
            public const string Name = "📛";
            public const string Id = "🆔";
            public const string Email = "📧";
            public const string Title = "💼";
            public const string Department = "🏢";
            public const string List = "📋";
        }

        public static class LdapProperties
        {
            public const string CommonName = "cn";
            public const string DistinguishedName = "distinguishedName";
            public const string Description = "description";
            public const string DisplayName = "displayName";
            public const string Member = "member";
            public const string MemberOf = "memberOf";
            public const string GroupType = "groupType";
            public const string SamAccountName = "sAMAccountName";
            public const string Mail = "mail";
            public const string Title = "title";
            public const string Department = "department";
        }

        public static class LdapFilters
        {
            public const string GroupObjectClass = "(objectClass=group)";
            public const string GroupByName = "(&(objectClass=group)(cn={0}))";
            public const string UserSearch = "(&(objectClass=user)(objectCategory=person)(|(cn={0})(sAMAccountName={0})(displayName={0})))";
        }

        public static class LdapSchemes
        {
            public const string LdapPrefix = "LDAP://";
        }

        public static class DefaultValues
        {
            public const string NotAvailable = "N/A";
            public const string Unknown = "Unknown";
            public const int DefaultMaxResults = 50;
            public const int DefaultMaxGroupResults = 100;
            public const int DefaultMaxDisplay = 10;
        }

        public static class Configuration
        {
            public const string LdapSettingsSection = "LdapSettings";
            public const string AppSettingsFile = "appsettings.json";
        }
    }
}