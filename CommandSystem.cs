using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace GaryJr
{
    class CommandSystem
    {

        private static Dictionary<String, Type> types;
        private static string helpText;

        public static string HelpText
        {
            get => helpText;
        }

        public static void ConfigureCommands(IServiceCollection services)
        {
  
            types = new Dictionary<string, Type>();
            var reflectionResults = 
                Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t) && t != typeof(ICommand));
            foreach (var type in reflectionResults)
            {
                if (type.Name.EndsWith("Command"))
                {
                    var name = 
                        type.Name
                        .Substring(0, type.Name.Length - 7)
                        .ToLower();
                    types.Add(name, type);
                    services.AddScoped(type);
                }
                else
                {
                    Console.Error.WriteLine("Invalid command class name -> " + type.Name);
                }   
            }

            services.AddSingleton<CommandSystem>();
        }

        // ----------------------------------------------------------------

        private IServiceProvider services;

        public CommandSystem(IServiceProvider services)
        { 
            this.services = services;
            BuildHelpText();
        }

        private void BuildHelpText()
        {
            helpText = "**Command List**\r\n```";
            foreach (var keyValuePair in types.OrderBy(f => f.Key))
            {
                helpText += keyValuePair.Key;
                helpText += " - ";
                var command = (ICommand)services.GetService(keyValuePair.Value);
                helpText += command.Description;
                helpText += "\r\n";
            }
            helpText += "```";
        }

        public async Task HandleCommand(SocketMessage message)
        {
            string key;
            var content = message.Content;
            int spaceIndex = content.IndexOf(' ');
            if (spaceIndex == -1)
            {
                key = content.Substring(1).ToLower();
            }
            else
            {
                key = content.Substring(1, spaceIndex - 1).ToLower();
            }

            Type type;
            types.TryGetValue(key, out type);
            if (type != null)
            {
                var command = (ICommand)services.GetService(type);
                if (await command.HasPermission(message.Author))
                {
                    await command.Run(message);
                }
                else
                {
                    await message.Channel.SendMessageAsync($"{message.Author.Mention} Permission denied.");
                }
            }
            else
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention} No such command.");
            }
        }
    }
}
