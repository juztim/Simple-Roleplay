using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Simple_Roleplay.Discord
{
    public class Commands : BaseCommandModule
    {
        [Command("status")]
        public async Task status_CMD(CommandContext ctx)
        {
            await ctx.Message.Channel.SendMessageAsync($"Spieler Online: {Alt.GetAllPlayers().Count}").ConfigureAwait(false);
        }
    }
}
