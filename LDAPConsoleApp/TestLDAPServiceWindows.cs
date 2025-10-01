using System;
using System.Linq;

namespace LDAPConsoleApp
{
    public static class LDAPTest
    {
        public static void RunTest()
        {
            var ldapService = new LDAPService();

            // Test DE domain for the specific group
            TestDEDomainGroup(ldapService, "IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN");
        }

        public static void TestDEDomainGroup(LDAPService ldapService, string groupName)
        {
            try
            {
                // Create a new LDAP service instance for DE domain
                var deLdapService = new LDAPService();
                
                if (deLdapService.ConnectToSpecificDomainQuiet("DE.bosch.com"))
                {
                    // Test the specific group
                    var groupDetails = deLdapService.GetGroupDetails(groupName);
                    if (groupDetails != null)
                    {
                        // Get group members
                        var members = deLdapService.GetGroupMembers(groupName);
                        deLdapService.ShowGroupMembers(members);
                    }
                    else
                    {
                        Console.WriteLine($"❌ Group not found in DE domain: {groupName}");
                    }
                    
                    deLdapService.Disconnect();
                }
                else
                {
                    Console.WriteLine("❌ Could not connect to DE domain");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error testing DE domain: {ex.Message}");
            }
        }
    }
}