using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands;
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
        public async Task FaqResponse(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Frequently Asked Questions!",
                Color = new DiscordColor("#d0f87c")
            };
            embed.AddField("This Looks cool! How can I join?", "Yay, welcome! Head on over to SecretGoblin.com to create an account!");
            embed.AddField("Why is X not in the game list?", "You mean not yet! At the moment we are manually curating the list of games!");
            embed.AddField("Why hasn't my profile been approved",
                "Lise goes through and approves profiles ~3 times a week, if you suspect yours missed the last approval round, we need you to add something! (See <#1325286764713349254> for more details)");
            var websiteEmoji = new DiscordComponentEmoji(1282759850413002773);
            var gameEmoji = new DiscordComponentEmoji("🎮");
            var websiteButton = new DiscordLinkButtonComponent("https://SecretGoblin.com", "Website", false,websiteEmoji);
            var gameButton = new DiscordLinkButtonComponent("https://forms.gle/vjcAWCkx1RVE8fjCA", "Suggest a game",
                false, gameEmoji);
            var discordMessage = new DiscordMessageBuilder().AddComponents([gameButton,websiteButton]).AddEmbed(embed
                .WithAuthor("Larry", iconUrl: "https://r2.fivemanage.com/IhCibjCMuV7gSbF16zgPO/images/Larry.png")
                .WithImageUrl("https://r2.fivemanage.com/IhCibjCMuV7gSbF16zgPO/images/dlg_banner.png")
                .Build());
            ctx.EditResponseAsync(discordMessage);

        }
    }
}
