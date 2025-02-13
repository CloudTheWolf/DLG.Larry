using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;

namespace LarryCore.Actions
{
    [Command("help"), Description("Ask Larry for help"), RequirePermissions(botPermissions: [], userPermissions: [DiscordPermission.UseApplicationCommands])]
    class LarryHelpCommands
    {
        [
            Command("faq"),
            Description("Ask larry for the requently asked questions"),
            RequirePermissions(botPermissions: [], userPermissions:
                [DiscordPermission.UseApplicationCommands])
        ]
        public async Task FaqResponse(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Frequently Asked Questions!",
                Color = new DiscordColor("#d0f87c")
            };
            embed.AddField("This Looks cool! How can I join?", "Yay, welcome! Head on over to SecretGoblin.com to create an account!");
            embed.AddField("Why is [my favorite game] not in the game list?", "You mean not yet! At the moment we are manually curating the list of games!");
            embed.AddField("Why hasn't my profile been approved",
                "Lise goes through and approves profiles ~3 times a week, if you suspect yours missed the last approval round, we need you to add something!");
            embed.AddField("How do I contact the Team/Get Help?",
                "Head on down the the <#1217555516461547570> where you'll not only find members of the Goblin team, but helpful members of the community.");
            embed.AddField("Help, there's a bug in here",
                "Sorry to hear that! Please file a bug report and we'll get right on squishing that!");
            embed.AddField("Where can I share feedback about the beta?", "Just click the Feedback Button below!");
            embed.AddField("Who is the team behind Goblins?", "You can see the team behind the goblins, as well as the Full FAQ Here: <#1325286764713349254>");
            
            var websiteButton = new DiscordLinkButtonComponent("https://SecretGoblin.com", "Website", false, new DiscordComponentEmoji(1282759850413002773));
            var gameButton = new DiscordLinkButtonComponent("https://forms.gle/vjcAWCkx1RVE8fjCA", "Suggest a game",
                false, new DiscordComponentEmoji("🎮"));
            var bugButton =
                new DiscordLinkButtonComponent("https://forms.gle/awtHVKtbrPMT5pKAA", "Report a Bug!", false, new DiscordComponentEmoji("🦟"));
            var feedbackButton = new DiscordLinkButtonComponent("https://forms.gle/ZQHohycVKSYJ5quC6", "Share Feedback",
                false, new DiscordComponentEmoji("💡"));
            var articleButton = new DiscordLinkButtonComponent(
                "https://datelikegoblins.substack.com/p/why-your-dating-profile-sucks", "Profile Advice", false,
                new DiscordComponentEmoji("🤔"));
            var discordMessage = new DiscordMessageBuilder().AddComponents([gameButton,websiteButton,bugButton,feedbackButton,articleButton]).AddEmbed(embed
                .WithAuthor("Larry", iconUrl: "https://r2.fivemanage.com/IhCibjCMuV7gSbF16zgPO/images/larry_128.png")
                .WithImageUrl("https://r2.fivemanage.com/IhCibjCMuV7gSbF16zgPO/images/dlg_banner.png")
                .Build());
            ctx.EditResponseAsync(discordMessage);

        }

        [
            Command("profile-review"),
            Description("Tell someone about profile reviews"),
            RequirePermissions(botPermissions: [], userPermissions:
                [DiscordPermission.UseApplicationCommands])
        ]
        public async Task ProfileReview(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var embed = new DiscordEmbedBuilder
            {
                Title = "Why your dating profile sucks!",
                Color = new DiscordColor("#d0f87c"),
                Description = "And how to fix it!\n\n" +
                              "Having reviewed hundreds of thousands of dating app profiles over the last decade both personally and professionally, I’ve identified why profiles aren’t successful...",
                Url = "https://datelikegoblins.substack.com/p/why-your-dating-profile-sucks"
            };
            var websiteButton = new DiscordLinkButtonComponent(
                "https://datelikegoblins.substack.com/p/why-your-dating-profile-sucks", "Read The Article!", false,
                new DiscordComponentEmoji(1282759850413002773));
            var message = new DiscordMessageBuilder
            {
                Content =
                    "Profile approvals are usually done at least once a week, If you're still awaiting approval check your emails." +
                    "\n\nIn the meantime, check out this article to help get your profile ready!"
            };
            message.AddEmbed(embed
                .WithAuthor("Larry", iconUrl: "https://r2.fivemanage.com/IhCibjCMuV7gSbF16zgPO/images/larry_128.png")
                .WithImageUrl(
                    "https://substackcdn.com/image/fetch/f_auto,q_auto:good,fl_progressive:steep/https%3A%2F%2Fsubstack-post-media.s3.amazonaws.com%2Fpublic%2Fimages%2F444ce51c-7fb1-4884-9f5f-5a5ef17359e9_1369x578.png")
                .Build());
            message.AddComponents([websiteButton]);
            ctx.EditResponseAsync(message);
        }
    }
}
