using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Net.Http;

namespace GaryJr.Commands
{
    class CatCommand : ICommand
    {
        string key;

        public CatCommand(Config config)
        {
            key = config.CatKey;
        }

        public string Description => "posts a random picture of a cat";

        public Task<bool> HasPermission(SocketUser user)
        {
            return Task.FromResult(true);
        }

        public async Task Run(SocketMessage message)
        {
            await message.Channel.TriggerTypingAsync();
            try
            {
                // download the xml result from cat api site
                string resultUri = null;
                string apiUri = $"https://thecatapi.com/api/images/get?api_key={key}&format=xml";
                var request = WebRequest.CreateHttp(apiUri);
                using (var response = await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var text = await reader.ReadToEndAsync();
                    var xml = XDocument.Parse(text);
                    var url = xml.Descendants("url").SingleOrDefault();
                    if (url != null)
                    {
                        resultUri = url.Value;
                    }
                }

                // if result contained a url to an image, download it and send to discord
                if (resultUri != null)
                {
                    var temp = Path.GetTempFileName();
                    request = WebRequest.CreateHttp(resultUri);
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
                await message.Channel.SendMessageAsync("Cat overflow exception. Too many cats.");
            }
        }
    }
}
