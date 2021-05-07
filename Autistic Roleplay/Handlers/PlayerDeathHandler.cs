using AltV.Net;
using AltV.Net.Elements.Entities;
using Simple_Roleplay.Factories;
using System;
using System.Threading.Tasks;
using AltV.Net.Data;

namespace Simple_Roleplay.Handlers
{
    class PlayerDeathHandler : IScript
    {
        [ScriptEvent(ScriptEventType.PlayerDead)]
        public async Task OnPlayerDeath(User player, IEntity killer, UInt32 reason)
        {
            if(player.paintballArena == 0)
            {

                await Task.Delay(120000);
                player.Spawn(new Position(359f, -594, 28.6f), 100);
                player.Health = 200;
                player.Armor = 200;
            }
            else
            {
                PaintballHandler.HandleDeath(player);
            }
            return;
        }
    }
}
