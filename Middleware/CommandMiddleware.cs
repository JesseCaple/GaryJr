using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace GaryJr.Middleware
{
    static class CommandExtensions
    {
        public static Dictionary<String, Type> types;

        public static void UseCommands(this IServiceCollection services)
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

            services.AddSingleton(typeof(IMiddleware), typeof(CommandMiddleware));
        }
    }

    class CommandMiddleware : IMiddleware
    {
        public static string helpText;
        public static string HelpText => helpText;

        private Dictionary<String, Type> types;
        private IServiceProvider services;

        public CommandMiddleware(IServiceProvider services)
        { 
            this.services = services;
            types = CommandExtensions.types;
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

        public async Task<bool> HandleMessage(SocketMessage message)
        {
            if (message.Content.Length > 1 && message.Content[0] == '.')
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
                    if (command.HasPermission(message.Author))
                    {
                        var task = command.RunAsync(message);
                        var continuation = task.ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                Exception ex = t.Exception;
                                while (ex is AggregateException && ex.InnerException != null)
                                {
                                    ex = ex.InnerException;
                                }
                                Console.WriteLine(ex.Message);
                            }
                        });
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
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
