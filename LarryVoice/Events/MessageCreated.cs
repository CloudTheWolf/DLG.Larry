using DSharpPlus.EventArgs;
using DSharpPlus;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using LarryVoice.Types;

namespace LarryVoice.Events
{
    internal class MessageCreated
    {
        private static DateTime lastClean = DateTime.Now.AddMinutes(-1);
       
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