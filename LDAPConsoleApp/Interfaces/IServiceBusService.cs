using System.Threading.Tasks;
using LDAPConsoleApp.Models;

namespace LDAPConsoleApp.Interfaces
{
    public interface IServiceBusService
    {
        Task<bool> ConnectAsync();
        Task<bool> SendGroupMessageAsync(Group group);
        Task DisconnectAsync();
        void Dispose();
    }
}