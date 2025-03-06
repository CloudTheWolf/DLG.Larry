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
using DSharpPlus.Entities;
using LarryCore.Types;
using DSharpPlus.Exceptions;

namespace LarryCore.Events
{
    internal class MessageCreated
    {
        private static DateTime lastMeme = DateTime.Now.AddMinutes(-5);
        private static DateTime lastClean = DateTime.Now.AddMinutes(-1);
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
            MessageCreatedEventArgs args)
        {
            if (args.Author.IsBot) return;
            if (channelIgnoreList.Contains(args.Channel.Id)) return;
            FloridaManPost(args);
        }

        private static async void FloridaManPost(MessageCreatedEventArgs args)
        {
            if ((int)(DateTime.Now - lastMeme).TotalMinutes < 5) return;
            var messageContent = args.Message.Content;
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
                    await args.Message.RespondAsync(msgBody!);
                    lastMeme = DateTime.Now;
                    break;
                }

            }

        }

        internal static async Task ModerateNewMessages(DiscordClient client, MessageCreatedEventArgs args)
        {
            if(args.Guild == null) return;
            if (args.Author.IsBot) return;
            var member = await args.Guild.GetMemberAsync(args.Author.Id);
            // If the user has Admin or Kick user perms, they are a moderator so don't act
            if (member.Permissions.HasPermission(DiscordPermission.Administrator) || member.Permissions.HasPermission(DiscordPermission.KickMembers)) return;
            var message = args.Message;
            if (message.MentionEveryone || message.Content.Contains("@here") || message.Content.Contains("@everyone"))
            {
                bool warned = true;
                try
                {
                    _ = await message.Author.SendMessageAsync(
                        $"The following message was deleted in the channel {message.Channel.Mention} in {args.Guild.Name} due to it being flagged as spam:\n`{message.Content}`\n\n" +
                        $"This action has been logged to the moderator team and further actions may result in a timeout, kick or even ban.\n" +
                        $"If you believe you have received this in error, or your account was recently compromised, please let a member of the DLG Team know");
                }
                catch (Exception e)
                {
                    warned = false;
                }

                await message.DeleteAsync();
                var notifyChannel = await client.GetChannelAsync(Options.ModNotificationsChannel);
                var modMessage =
                    $"The following message by {message.Author.Mention} was deleted in the channel {message.Channel.Name} due to it being flagged as spam:\n`{message.Content}`";
                if (!warned)
                {
                    modMessage += "\nI could not notify them of this via DMs due to their privacy Settings";
                }
                client.SendMessageAsync(notifyChannel,
                    modMessage);
            }
        }

        internal static async Task CleanupChannels(DiscordClient client, MessageCreatedEventArgs args)
        {
            var channelsToRemove = new List<Channel>();
            if ((int)(DateTime.Now - lastClean).TotalMinutes < 1) return;
            foreach (var channel in Options.Channels)
            {
                DiscordChannel channelData = null;
                try
                {
                    channelData = await client.GetChannelAsync(channel.Id);
                } catch(NotFoundException)
                {
                    channelsToRemove.Add(channel);
                    continue;
                }

                if (channelData == null)
                {
                    channelsToRemove.Add(channel);
                    continue;
                }

                if ((DateTime.Now - channel.LastUsed).TotalMinutes < 5) continue;
                if(channelData.Users.Count > 0)
                {
                    channel.LastUsed = DateTime.UtcNow;
                    continue;
                }

                await channelData.DeleteAsync();
                channelsToRemove.Add(channel);
            }
            foreach(var channel in channelsToRemove)
            {
                Options.Channels.Remove(channel);
            }
            string tempPath = Path.GetTempPath();
            string fileName = "channels.json";
            string fullPath = Path.Combine(tempPath, fileName);
            await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(Options.Channels));
        }
    }
}