using System.Threading.Tasks;
using ClientConsoleApp.Models;

namespace ClientConsoleApp.Interfaces
{
    public interface IServiceBusConsumerService
    {
        Task<bool> ConnectAsync();
        Task StartReceivingAsync(Func<Group, Task> onMessageReceived);
        Task DisconnectAsync();
        void Dispose();
    }
}