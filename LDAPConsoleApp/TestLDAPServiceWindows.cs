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
            TestDomainGroup(_settings.SecondaryDomain, _settings.DefaultGroupName);
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
    }
}