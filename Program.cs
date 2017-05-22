using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using GaryJr.Commands;

namespace GaryJr
{
    class Program
    {
        public static void Main(string[] args)
        {
            var program = new Program();
            var services = new ServiceCollection();
            program.ConfigureServices(services);
            program.Run(services.BuildServiceProvider())
                .GetAwaiter()
                .GetResult();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Config>();
            CommandSystem.ConfigureCommands(services);
            services.AddSingleton<Bot>();
        }

        private async Task Run(IServiceProvider provider)
        {
            await provider.GetService<Bot>().Run();
        }

    }
}