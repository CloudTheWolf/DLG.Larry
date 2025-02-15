using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.EventArgs;
using LarryCore.Actions;
using LarryCore.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace LarryCore
{
    public class Main : IPlugin
    {
        public string Name => "Larry Core";

        public string Description => "Core commands / actions for Larry";

        public int Version => 1;

        public void InitPlugin(IBot bot, ILogger logger, DiscordConfiguration discordConfiguration,
            IConfigurationRoot applicationConfig)
        {
            Options.LogPrefix = Name;
            Logger.Initialize();
            LoadConfig(applicationConfig);
            RegisterCommands(bot);
            bot.EventHandlerRegistry.Register(e => e
                    .HandleMessageCreated(MessageCreated.ModerateNewMessages)
                    .HandleMessageCreated(MessageCreated.OnMessageCreated));
                
            
        }

        private void RegisterCommands(IBot bot)
        {
            var helpCommands = CommandBuilder.From(typeof(LarryHelpCommands));
            bot.CommandsList.Add(helpCommands);
        }

        private void LoadConfig(IConfigurationRoot applicationConfig)
        {
            Options.ModNotificationsChannel = applicationConfig.GetValue<ulong>("modChannel");
        }
    }
}
