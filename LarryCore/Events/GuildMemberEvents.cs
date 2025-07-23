using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Entities.AuditLogs;
using DSharpPlus.EventArgs;

namespace LarryCore.Events
{
    internal class GuildMemberEvents
    {
        private static async Task HandleMemeberLeave(DiscordClient client, GuildMemberRemovedEventArgs args)
        {
            await Task.Delay(15000);
            var banEvents = false;
            var kickEvents = false;

            await foreach (DiscordAuditLogBanEntry log in (args.Guild.GetAuditLogsAsync(150, null, DiscordAuditLogActionType.Ban)))
            {

                if(log.Target.Id == args.Member.Id)
                {
                    banEvents = true;
                    break;
                }
            }

            await foreach (DiscordAuditLogKickEntry log in (args.Guild.GetAuditLogsAsync(150, null, DiscordAuditLogActionType.Kick)))
            {

                if (log.Target.Id == args.Member.Id)
                {
                    kickEvents = true;
                    break;
                }
            }

            var logChannel = await args.Guild.GetChannelAsync(1340322868155908096);
            if (banEvents || kickEvents)
            {
                await logChannel.SendMessageAsync($"Member {args.Member.Username} ({args.Member.Id}) has left the server. ban or kick events found in the last 150 logs");
                return;
            }
            

            
            await logChannel.SendMessageAsync($"Member {args.Member.Username} ({args.Member.Id}) has left the server. No ban or kick events found in the last 150 logs, so left cleanly");

        }

        internal static async Task HandleGuildMemberRemoved(DiscordClient client, GuildMemberRemovedEventArgs args)
        {
            // Queue Member Leave Event
            HandleMemeberLeave(client, args);
        }

        internal static async Task HandleGuildMemberUpdated(DiscordClient client, GuildMemberUpdatedEventArgs args)
        {
            
            var unverified = await args.Guild.GetRoleAsync(1382362440607469752);


            if (args.RolesAfter.Contains(unverified))
            {
                // User has just been assigned the unverified role, let's have then read the rules
                var rulesChannel = await args.Guild.GetChannelAsync(1217104547739340910);                
                var message = await rulesChannel.SendMessageAsync($"Welcome {args.Member.Mention}! Please read the rules in {rulesChannel.Mention} and react with 💚 to verify your account.");
                Thread.Sleep(30000);
                message.DeleteAsync();

            }
        }
    }
}
