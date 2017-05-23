using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using GaryJr.Middleware;

namespace GaryJr.Commands
{
    class HelpCommand : ICommand
    {
        public string Description => "displays list of bot commands";

        public bool HasPermission(SocketUser user)
        {
            return true;
        }

        public async Task RunAsync(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(CommandMiddleware.HelpText);
        }
    }
}
