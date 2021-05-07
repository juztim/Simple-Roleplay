using AltV.Net;
using Simple_Roleplay.Factories;
using System;
using System.Threading.Tasks;

namespace Simple_Roleplay.Handlers
{
    public class NotifyHandler : IScript
    {
        internal static Task SendSupportMessage(User player, string message)
        {
            player.Emit("client:cef:notify", 1, message);
            return Task.CompletedTask;
        }

        internal static Task SendSupportMessage(User player, string message, int duration)
        {
            player.Emit("client:cef:notify", 1, message, duration);
            return Task.CompletedTask;
        }

        internal static Task SendNotification(User player, string message)
        {
            player.Emit("client:cef:notify", 5, message);
            return Task.CompletedTask;
        }

        internal static Task SendNotification(User player, string message, int duration)
        {
            player.Emit("client:cef:notify", 5, message, duration);
            return Task.CompletedTask;
        }

        internal static void SendNotificationToAll(string message, int duration)
        {
            foreach (var player1 in Alt.GetAllPlayers())
            {
                var player = (User) player1;
                player.Emit("client:cef:notify", 5, message, duration);
            }
        }

        internal static void sendPdNotify(string message, int duration)
        {
            foreach (User player in Alt.GetAllPlayers())
            {
                if (player.frakId != 1 && player.frakId != 2 && player.IsJobDuty) break;
                player.Emit("client:cef:notify", 5, message, duration);
            }
        }
    }
}
