using Discord;
using Discord.WebSocket;
using GaryJr.Middleware;
using GaryJr.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GaryJr
{
    class Bot
    {
        ConfigService config;
        DiscordSocketClient client;
        IEnumerable<IMiddleware> middleware;

        public Bot(
            ConfigService config,
            IEnumerable<IMiddleware> middleware)
        {
            this.config = config;
            this.middleware = middleware;
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
            foreach (var m in middleware)
            {
                if (await m.HandleMessage(message))
                {
                    break;
                }
            }
        }
    }
}
