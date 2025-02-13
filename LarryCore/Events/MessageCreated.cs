using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using DSharpPlus.EventArgs;
using DSharpPlus;
using Newtonsoft.Json.Linq;
using LarryCore.Types.FloridaMan;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace LarryCore.Events
{
    internal class MessageCreated
    {
        private static DateTime lastMeme = DateTime.Now.AddMinutes(-5);

        private static List<ulong> channelIgnoreList =
        [
            1217104547739340910,
            1217097657747505224,
            1217089314056306718,
            1282757370682019922,
            1297686036834553997,
            1323471735860891742,
            1325286764713349254
        ];

        internal static async Task OnMessageCreated(DiscordClient client,
            MessageCreatedEventArgs messageCreatedEventArgs)
        {
            if (messageCreatedEventArgs.Author.IsBot) return;
            if (channelIgnoreList.Contains(messageCreatedEventArgs.Channel.Id)) return;
            FloridaManPost(messageCreatedEventArgs);
        }

        private static async void FloridaManPost(MessageCreatedEventArgs messageCreatedEventArgs)
        {
            if ((int)(DateTime.Now - lastMeme).TotalMinutes < 5) return;
            var messageContent = messageCreatedEventArgs.Message.Content;
            if (!Regex.IsMatch(messageContent, @"florida\s+(man|woman|person)", RegexOptions.IgnoreCase))
            {
                return;
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://newsapi.org/v2/everything?q=%22Florida%20Man%22&pageSize=100&apiKey=14508b3daae94c8e95f34f197db51a8f&sortBy=latest");
            request.Headers.Add("User-Agent", "Larry");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            dynamic body = await response.Content.ReadAsStringAsync();
            Root result = JsonConvert.DeserializeObject<Root>(body);
            var articles = result.Articles;
            if (result.Status != "ok") return;

            var rnd = new Random();
            const bool foundItem = false;
            while (!foundItem)
            {
                if (articles.Count == 0)
                {
                    break;
                }

                var itemId = rnd.Next(articles.Count);
                var article = articles[itemId];
                var floridaRegex = new Regex(@"florida\s+(?:man|woman)", RegexOptions.IgnoreCase);
                var title = article.Title ?? "";
                var description = article.Description ?? "";
                var matchInTitle = floridaRegex.IsMatch(title);
                var matchInDescription = floridaRegex.IsMatch(description);
                if (!matchInTitle && !matchInDescription)
                {
                    articles.RemoveAt(itemId);
                }
                else
                {
                    var msgBody = matchInTitle ? article.Title : article.Description;
                    await messageCreatedEventArgs.Message.RespondAsync(msgBody!);
                    lastMeme = DateTime.Now;
                    break;
                }

            }

        }
    }
}