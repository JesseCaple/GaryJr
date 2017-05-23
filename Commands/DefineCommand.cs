using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using GaryJr.Services;

namespace GaryJr.Commands
{
    class DefineCommand : ICommand
    {
        public string Description => "shows the true definition of a word";

        string key;

        public DefineCommand(ConfigService config)
        {
            key = config.MashapeKey;
        }

        public bool HasPermission(SocketUser user)
        {
            return true;
        }

        public async Task RunAsync(SocketMessage message)
        {
            var contentIndex = message.Content.IndexOf(' ');
            if (contentIndex == -1)
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention} What word did you want a definition for?");
                return;
            }

            var query = message.Content.Substring(contentIndex + 1);
            await message.Channel.TriggerTypingAsync();
            try
            {
                var uri = $"https://mashape-community-urban-dictionary.p.mashape.com/define?term={query}";
                var request = WebRequest.CreateHttp(uri);
                request.Headers["X-Mashape-Key"] = key;
                using (var response = await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var text = await reader.ReadToEndAsync();
                    var json = JObject.Parse(text);
                    if (json != null)
                    {
                        var token = json.SelectToken("list[0].definition");
                        if (token != null)
                        {
                            var definition = token.Value<string>();
                            var word = json.SelectToken("list[0].word");
                            var example = json.SelectToken("list[0].example");
                            var str = $"\r\nDefinition of **{word}** ```\r\n{definition}```\r\n Example\r\n```\r\n{example}```";
                            await message.Channel.SendMessageAsync(str);
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync("wat");
                        }
                    }
                }
            }
            catch (Exception)
            {
                await message.Channel.SendMessageAsync("I left my pocket dictionary at home.");
            }
        }
    }
}
