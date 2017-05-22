using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GaryJr
{
    class Bot
    {
        Config config;
        CommandSystem commandSystem;
        DiscordSocketClient client;

        public Bot(
            Config config,
            CommandSystem commandSystem)
        {
            this.config = config;
            this.commandSystem = commandSystem;
        }

        public async Task Run()
        {
            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += MessageReceived;
            await client.LoginAsync(TokenType.Bot, config.DiscordKey);
            await client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content.Length > 1 && message.Content[0] == '.')
            {
                await commandSystem.HandleCommand(message);
            }
        }
    }
}
