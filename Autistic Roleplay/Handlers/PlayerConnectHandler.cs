using AltV.Net;
using AltV.Net.Data;
using Simple_Roleplay.Factories;
using System.Threading.Tasks;

namespace Simple_Roleplay.Handlers
{
    class PlayerConnectHandler : IScript
    {
        [ScriptEvent(ScriptEventType.PlayerConnect)]
        public Task onPlayerJoinAsync(User player, string reason)
        {
            
            player.Dimension = player.Id + 5;
            
            player.Emit("player:connect");
            return Task.CompletedTask;
        }
    }
}
