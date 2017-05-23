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
    class ImageCommand : ICommand
    {
        public string Description => "posts a picture related to whatever else you type";

        private Random random = new Random();
        private string key, cx;

        public ImageCommand(ConfigService config)
        {
            key = config.GoogleKey;
            cx = config.GoogleSearchCx;
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
                await message.Channel.SendMessageAsync($"{message.Author.Mention} What image did you want to see?");
                return;
            }

            var query = message.Content.Substring(contentIndex + 1);
            await message.Channel.TriggerTypingAsync();
            try
            {
                string resultLink = null;
                int offset = this.random.Next(0, 45);
                var uri = $"https://www.googleapis.com/customsearch/v1?q={query}&num=1&start={offset}&imgSize=medium&searchType=image&key={key}&cx={cx}";
                var request = WebRequest.CreateHttp(uri);
                using (var response = await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var text = await reader.ReadToEndAsync();
                    var json = JObject.Parse(text);
                    if (json != null)
                    {
                        var token = json.SelectToken("items[0].link");
                        if (token != null)
                        {
                            resultLink = token.Value<string>();
                        }
                    }
                }


                // if result contained a url to an image, download it and send to discord
                if (resultLink != null)
                {
                    var temp = Path.GetTempFileName();
                    request = WebRequest.CreateHttp(resultLink);
                    using (var response = await request.GetResponseAsync())
                    using (var stream = response.GetResponseStream())
                    {
                        temp = temp + "." + response.ContentType.Substring(6);
                        using (var writer = File.Create(temp))
                        {
                            await stream.CopyToAsync(writer);
                        }
                    }
                    await message.Channel.SendFileAsync(temp);
                    File.Delete(temp);
                }
                else
                {
                    await message.Channel.SendMessageAsync("Cat not found exeption. Maybe they are hiding.");
                }
            }
            catch (Exception)
            {
                await message.Channel.SendMessageAsync("Sorry bro, fresh out of quality images for today.");
            }
        }
    }
}
