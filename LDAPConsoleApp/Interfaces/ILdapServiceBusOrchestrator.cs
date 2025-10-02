using System.Threading.Tasks;

namespace LDAPConsoleApp.Interfaces
{
    public interface ILdapServiceBusOrchestrator
    {
        Task ProcessGroupsAndSendToServiceBusAsync();
    }
}