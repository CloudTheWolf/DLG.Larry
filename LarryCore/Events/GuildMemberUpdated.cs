using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace LarryCore.Events
{
    internal class GuildMemberUpdated
    {
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
