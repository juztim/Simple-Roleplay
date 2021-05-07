using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace Simple_Roleplay.Discord
{
    public class BotMain
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var config = new DiscordConfiguration
            {
                Token = "<Discord Bot Token>",
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug

            };

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new []{"."},
                EnableMentionPrefix = true,
                EnableDms = true,
                CaseSensitive = false,
                IgnoreExtraArguments = true
            };


            Client = new DiscordClient(config);
            Client.Ready += Client_Ready;
            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<Commands>(); 
            await Client.ConnectAsync();
            await Task.Delay(-1);

            System.Timers.Timer timer = new System.Timers.Timer {Interval = 600};
            timer.Elapsed += Timer_Elapsed;
            timer.Start();


        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateUserCount();
            Console.WriteLine($"UPDATE COUNT");
        }

        private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            this.Client.UpdateStatusAsync(new DiscordActivity($"{Alt.GetAllPlayers().Count} Spieler", ActivityType.Watching));
            return Task.CompletedTask;
        }

        public Task UpdateUserCount()
        {
            this.Client.UpdateStatusAsync(new DiscordActivity($"{Alt.GetAllPlayers().Count} Spieler", ActivityType.Watching));
            return Task.CompletedTask;
        }
    }
}
