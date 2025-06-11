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
    internal class ReactionAdded
    {        
        internal static async Task HandleMesssageReactionAdded(DiscordClient client,
            MessageReactionAddedEventArgs args)
        {
            if(args.User.IsBot)
            {
                return;
            }

            var unverified = await args.Guild.GetRoleAsync(1382362440607469752);
            var verified = await args.Guild.GetRoleAsync(1282385508818751629);

            if(args.Channel.Id == 1217104547739340910 && args.Emoji.Name == "💚")
            {
                var member = await args.Guild.GetMemberAsync(args.User.Id);
                var hasRole = member.Roles.Select(r => r.Id).ToList();
                if (hasRole.Contains(unverified.Id))
                {
                    await member.RevokeRoleAsync(unverified);
                }
                if (!hasRole.Contains(verified.Id))
                {
                    await member.GrantRoleAsync(verified);
                }
                
                
            }

        }
        
    }
}