using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace GaryJr.Commands
{
    class HelpCommand : ICommand
    {
        public string Description => "displays list of bot commands";

        public Task<bool> HasPermission(SocketUser user)
        {
            return Task.FromResult(true);
        }

        public async Task Run(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(CommandSystem.HelpText);
        }
    }
}
