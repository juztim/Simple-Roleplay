using AltV.Net;
using Simple_Roleplay.Factories;

namespace Simple_Roleplay.Handlers
{
    public class SaltyHandler : IScript
    {
        [ClientEvent("SaltyChat_SetVoiceRange")]
        public void SetVoiceRange(User player, dynamic voicerange)
        {

        }
    }
}
