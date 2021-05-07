using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;

namespace Simple_Roleplay.Handlers
{
    public class AnimationMenuHandler : IScript
    {
        [ClientEvent("Server:Animation:GetAnimations")]
        public Task GetAnimations(User player)
        {
            string interactHTML = "";
            interactHTML += "<li><p id='AnimationMenu-Title'>Schließen</p></li><li class='animationitem' data-action='close' data-actionstring='Abbrechen'><img src='img/interact/close.png'></li>";
            foreach (var anim in Main.database.AnimationCollection.AsQueryable().ToList())
            {
                interactHTML += $"<li class='animationitem' data-animdict='{anim.AnimDict}' data-animname='{anim.AnimName}' data-animflag='{anim.Flags}' data-actionstring='{anim.Name}' data-duration='{anim.Duration}'><img src='img/Animation/{anim.Category}.png'></li>";
            }
            player.Emit("Client:Animations:SetInfo", interactHTML);
            return Task.CompletedTask;
        }
    }
}
