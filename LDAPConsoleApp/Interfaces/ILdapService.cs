using System.Collections.Generic;

namespace LDAPConsoleApp.Interfaces
{
    public interface ILdapService
    {
        bool Connect();
        bool ConnectToSpecificDomain(string domain);
        bool ConnectToSpecificDomainQuiet(string domain);
        List<Dictionary<string, object?>> GetAllGroups(int maxResults = 100);
        List<Dictionary<string, object?>> GetGroupsByName(string searchPattern, int maxResults = 50);
        List<Dictionary<string, object?>> SearchUsers(string searchPattern, int maxResults = 50);
        Dictionary<string, object?>? GetGroupDetails(string groupName);
        List<Dictionary<string, object?>> GetGroupMembers(string groupName);
        void ShowGroups(List<Dictionary<string, object?>> groups, int maxDisplay = 10);
        void ShowGroupDetails(Dictionary<string, object?> group);
        void ShowGroupMembers(List<Dictionary<string, object?>> members);
        void Disconnect();
        void Dispose();
    }
}