using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using LarryVoice.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LarryVoice.Actions
{
    [Command("luminary"), Description("Commands for Luminary members")]
    class LarryLuminaryCommands
    {

        [Command("make-vc"), Description("Make A Voice Channel")]
        public async Task MakeVoiceChannels(SlashCommandContext ctx,
            [Parameter("limit"), Description("Limit number of members (0 = Unliminted [Default])")] int limit = 0)
        {
            var adminRole = await ctx.Guild.GetRoleAsync(1217539141898866718);
            var modRole = await ctx.Guild.GetRoleAsync(1356182450241278042);
            var babyLuminaryRole = await ctx.Guild.GetRoleAsync(1348123840466649120);
            var luminaryRole = await ctx.Guild.GetRoleAsync(1338004463826374666);
            var juniorLuminaryRole = await ctx.Guild.GetRoleAsync(1348124415543742595);
            var elderLuminaryRole = await ctx.Guild.GetRoleAsync(1348122720344473661);
            var parent = await ctx.Guild.GetChannelAsync(1217082125132632169);

            var requiredRoles = new[] { adminRole, modRole, babyLuminaryRole, luminaryRole, juniorLuminaryRole, elderLuminaryRole };
            bool hasAnyRequired = requiredRoles.Any(r => ctx.Member.Roles.Contains(r));
            string tempPath = Path.GetTempPath();
            string fileName = "channels.json";
            string fullPath = Path.Combine(tempPath, fileName);
            if (hasAnyRequired)
            {
                await ctx.DeferResponseAsync(true); 
                var name = $"{ctx.Member.DisplayName}'s VC";
                var guild = ctx.Guild;
                var channel = await guild.CreateVoiceChannelAsync($"👑 {name} ",userLimit: limit,parent:parent);
                var channelData = new Channel()
                {
                    Id = channel.Id,
                    Name = channel.Name,
                    LastUsed = DateTime.UtcNow,
                    Owner = ctx.Member.Id

                }; 
                Options.Channels.Add(channelData);                
                await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent($"Your VC is ready! (It will be deleted if inactive for a 5 minutes or more)"));
                await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(Options.Channels));
            }
            else
            {
                await ctx.RespondAsync("You do not have permission to do that",true);
            }
        }
    }
}
