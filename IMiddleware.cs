using Discord.WebSocket;
using System.Threading.Tasks;

namespace GaryJr
{
    interface IMiddleware
    {
        Task<bool> HandleMessage(SocketMessage message);
    }
}
