using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GaryJr
{
    interface ICommand
    {
        Task<bool> HasPermission(SocketUser user);
        Task Run(SocketMessage message);
        string Description { get; }
    }
}
