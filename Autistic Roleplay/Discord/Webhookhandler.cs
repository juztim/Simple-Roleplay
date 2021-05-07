using System.Collections.Generic;
using System.Net.Http;

namespace Simple_Roleplay.Discord
{
    public class Webhookhandler
    {
        private static readonly HttpClient client = new HttpClient();

        public static void sendMessage(string message)
        {
            var stringContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "Simple Roleplay"),
                new KeyValuePair<string, string>("avatar_url", ""),
                new KeyValuePair<string, string>("content", message),
            });
            client.PostAsync("Discord Webhook URL", stringContent);
        }

    }
}
