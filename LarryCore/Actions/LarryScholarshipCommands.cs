using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LarryCore.Actions
{
    [Command("freeme")]
    [Description("Scholarship Specific Commands")]
    [RequirePermissions(botPermissions: [], userPermissions: [DiscordPermission.UseApplicationCommands])]
    internal class LarryScholarshipCommands
    {
        [Command("larry")]
        [Description("Start the Scholarship Process")]
        [RequirePermissions(botPermissions: [], userPermissions: [DiscordPermission.UseApplicationCommands])]
        public async Task Claim(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);
            var hasPatreonRole = false;
            var excludedRoles = new List<DiscordRole>
            {
                await ctx.Guild.GetRoleAsync(1348123840466649120),
                await ctx.Guild.GetRoleAsync(1338004463826374666),
                await ctx.Guild.GetRoleAsync(1348124415543742595),
                await ctx.Guild.GetRoleAsync(1348122720344473661),
                // Scholarship role
                await ctx.Guild.GetRoleAsync(1402724236933599383)
            };

            if (ctx.Member.Roles.Any(role => excludedRoles.Contains(role)))
            {
                hasPatreonRole = true;
            }
            

            
            var interactivity = (ctx.Client.ServiceProvider.GetService(typeof(InteractivityExtension)) as InteractivityExtension);
            if (hasPatreonRole)
            {
                await ctx.EditResponseAsync(new DiscordInteractionResponseBuilder()
                    .WithContent("<:Goblin:1282761081319723084> You already have access. You do not need to claim scholarship!")
                    .AsEphemeral(true));
                return;
            }
            var initEmbed = new DiscordEmbedBuilder
            {
                Title = "What’s keeping you from subscribing to Goblins’ paid Discord?",
                Description = "We understand that not everyone can afford $3/month or may not wish to pay.\nWe get it, and there’s no judgment either way.",
                Color = DiscordColor.SpringGreen,
                Timestamp = DateTimeOffset.UtcNow
            };
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent("Pick the option that best reflects your current situation.")
                .AddEmbed(initEmbed)
                .AddActionRowComponent(
                    new DiscordButtonComponent(DiscordButtonStyle.Success, "apply_scholarship", "I can’t afford the $3/month"),
                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "apply_feedback", "I don’t want to pay $3/month")
                    )
                .AsEphemeral(true);


            await ctx.EditResponseAsync(builder);
            var clicked = await ctx.GetResponseAsync();
            var result = await clicked.WaitForButtonAsync();
           

            if (!result.TimedOut)
            {
                if (result.Result.Id == "apply_scholarship")
                {                    
                    await StartScholarshipModal(ctx, result.Result.Interaction, interactivity);
                }
                else if (result.Result.Id == "apply_feedback")
                {                   
                    await StartFeedbackModal(ctx, result.Result.Interaction, interactivity);
                }
            }
            ctx.DeleteResponseAsync();
        }


        private async Task StartScholarshipModal(SlashCommandContext ctx, DiscordInteraction interaction, InteractivityExtension interactivity)
        {


            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Scholarship Claim Form")
                .WithCustomId("scholarship_modal_1")
                .AddTextInputComponent(new DiscordTextInputComponent(label: "Username or Email", customId: "s1", placeholder: "Username or Email of your Goblins account", required: true, style: DiscordTextInputStyle.Short, min_length: 5, max_length: 50))
                .AddTextInputComponent(new DiscordTextInputComponent(label: "Why are you requesting a scholarship?", customId: "s2", placeholder: "", required: true, style: DiscordTextInputStyle.Paragraph, max_length: 2000))
                .AddTextInputComponent(new DiscordTextInputComponent(label: "How has/will the community help you?", customId: "s3", placeholder: "", required: true, style: DiscordTextInputStyle.Paragraph, min_length: 5, max_length:2000))
                .AddTextInputComponent(new DiscordTextInputComponent(label: "Do you need longer than 3 months?", customId: "s4", placeholder: "Yes or No", required: true, style: DiscordTextInputStyle.Short, min_length:2, max_length: 3))
                .AddTextInputComponent(new DiscordTextInputComponent(label: "Are you open to contributing?", customId: "s5", placeholder: "Yes or No", required: true, style: DiscordTextInputStyle.Short, min_length: 2, max_length: 3));

            await interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, modal);

            var modalResult1 = await interactivity.WaitForModalAsync("scholarship_modal_1", ctx.User, TimeSpan.FromMinutes(15));
            if (modalResult1.TimedOut)
            {
                await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("⏰ Request timed out. Please start again.")
                    .AsEphemeral(true));
                return;
            }

            var values = modalResult1.Result.Values;

            await modalResult1.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("✅ Scholarship request submitted successfully!")
                    .AsEphemeral(true));

            var logChannel = await ctx.Client.GetChannelAsync(Options.ClaimChannelId);
            var embedMessage = new DiscordEmbedBuilder
            {
                Title = "📨 New Scholarship Request",                
                Color = DiscordColor.SpringGreen,
                Timestamp = DateTimeOffset.UtcNow
            }.WithFooter($"Submitted by {ctx.User.Username}", ctx.User.AvatarUrl);
            embedMessage.AddField("Discord User", $"{ctx.User.Mention} [{ctx.User.Id}]", true);
            embedMessage.AddField("Username/Email", values["s1"]);
            embedMessage.AddField("Reason For Requesting", values["s2"]);
            embedMessage.AddField("Community Impact", values["s3"]);
            embedMessage.AddField("Longer Than 3 Months?", values["s4"], true);
            if (values.ContainsKey("s5") && !string.IsNullOrEmpty(values["s5"]))
            {
                embedMessage.AddField("Willing to Contribute", values["s5"], true);
            }
            await ctx.Member.GrantRoleAsync(await ctx.Guild.GetRoleAsync(1402724236933599383));
            await logChannel.SendMessageAsync(embedMessage);
        }

        private async Task StartFeedbackModal(SlashCommandContext ctx, DiscordInteraction interaction, InteractivityExtension interactivity)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Goblins Feedback Form")
                .WithCustomId("feedback_modal")
                .AddTextInputComponent(new DiscordTextInputComponent(label: "Why aren't you paying?", customId: "f1", placeholder: "e.g. I didn’t know, too many expenses", required: true, style: DiscordTextInputStyle.Paragraph, min_length: 15, max_length: 2000))
                .AddTextInputComponent(new DiscordTextInputComponent(label: "What would make it a no-brainer?", customId: "f2", placeholder: "What would you want to see added that would make it worth the monthly cost.", required: true, style: DiscordTextInputStyle.Paragraph, min_length:15, max_length:2000))
                .AddTextInputComponent(new DiscordTextInputComponent(label: "How was your experience so far?", customId: "f3", placeholder: "Let us know, even if it's bad. (Especially if it's bad so we can fix it)", required: true, style: DiscordTextInputStyle.Paragraph, min_length: 15, max_length: 2000))
                .AddTextInputComponent(new DiscordTextInputComponent(label: "Other ways you’d support us?", customId: "f4", placeholder: "Merch, social media, etc.", required: true, style: DiscordTextInputStyle.Paragraph, min_length: 15, max_length: 2000))
                .AddTextInputComponent(new DiscordTextInputComponent(label: "Anything else we should know?", customId: "f5", placeholder: "Optional", required: false, style: DiscordTextInputStyle.Paragraph, max_length: 2000));

            await interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, modal);

            var modalResult = await interactivity.WaitForModalAsync("feedback_modal", ctx.User, TimeSpan.FromMinutes(15));

            if (modalResult.TimedOut)
            {
                await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("⏰ Request timed out. Please start again.")
                    .AsEphemeral(true));
                return;
            }

            var values = modalResult.Result.Values;

            await modalResult.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("✅ Feedback submitted successfully!")
                    .AsEphemeral(true));

            var logChannel = await ctx.Client.GetChannelAsync(Options.ClaimChannelId);
            var embedMessage = new DiscordEmbedBuilder
            {
                Title = "📨 New Feedback Submission",
                Color = DiscordColor.CornflowerBlue,
                Timestamp = DateTimeOffset.UtcNow
            }.WithFooter($"Submitted by {ctx.User.Username}", ctx.User.AvatarUrl);
            embedMessage.AddField("Discord User", $"{ctx.User.Mention} [{ctx.User.Id}]", true);
            embedMessage.AddField("Why aren't you paying?", values["f1"]);
            embedMessage.AddField("What would make it a no-brainer?", values["f2"]);
            embedMessage.AddField("How was your experience so far?", values["f3"]);
            embedMessage.AddField("Other ways you’d support us?", values["f4"]);
            if (values.ContainsKey("f5") && !string.IsNullOrEmpty(values["f5"]))
            {
                embedMessage.AddField("Anything else we should know?", values["f5"]);
            }
            await ctx.Member.GrantRoleAsync(await ctx.Guild.GetRoleAsync(1402724236933599383));
            await logChannel.SendMessageAsync(embedMessage);
        }
    }
}
