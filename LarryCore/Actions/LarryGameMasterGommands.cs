using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using LarryCore.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LarryCore.Actions
{
    [Command("gm"), Description("Game Master Commands")]
    class LarryGameMasterGommands
    {
        [Command("make-vc"), Description("Make Voice Channel(s)")]
        public async Task MakeVoiceChannels(SlashCommandContext ctx,
            [Parameter("game"), Description("Game name")] string name,
            [Parameter("count"), Description("Number of channels to make")] int count = 1,
            [Parameter("limit"), Description("Limit channel members (0 = Unliminted [Default])")] int limit = 0)
        {
            var gmRole = await ctx.Guild.GetRoleAsync(1343305945354342562);
            var adminRole = await ctx.Guild.GetRoleAsync(1217539141898866718);
            var parent = await ctx.Guild.GetChannelAsync(1217082125132632169);

            var requiredRoles = new[] { gmRole, adminRole };
            bool hasAnyRequired = requiredRoles.Any(r => ctx.Member.Roles.Contains(r));
            string tempPath = Path.GetTempPath();
            string fileName = "channels.json";
            string fullPath = Path.Combine(tempPath, fileName);
            if (hasAnyRequired)
            {
                await ctx.DeferResponseAsync(true);                
                var guild = ctx.Guild;
                for (int i = 0; i < count; i++)
                {
                    var channel = await guild.CreateVoiceChannelAsync($"🎮 [{i + 1}] {name} ",userLimit: limit,parent:parent);
                    var channelData = new Channel()
                    {
                        Id = channel.Id,
                        Name = channel.Name,
                        LastUsed = DateTime.UtcNow
                    }; 
                    Options.Channels.Add(channelData);
                }
                await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent($"Created {count} voice channel(s) For {name}"));
                await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(Options.Channels));
            }
            else
            {
                await ctx.RespondAsync("You do not have permission to do that",true);
            }
        }
    }
}
