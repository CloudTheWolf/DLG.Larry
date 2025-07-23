using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.EventArgs;
using LarryCore.Actions;
using LarryCore.Events;
using LarryCore.Types;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
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
                    .HandleMessageCreated(MessageCreated.OnMessageCreated)
                    .HandleMessageReactionAdded(ReactionAdded.HandleMesssageReactionAdded)
                    .HandleGuildMemberUpdated(GuildMemberEvents.HandleGuildMemberUpdated)
                    .HandleGuildMemberRemoved(GuildMemberEvents.HandleGuildMemberRemoved)
                    .HandleGuildBanAdded(GuildBanEvents.HandleGuildBanAdded)                    
                    );
                
            
        }

        private void RegisterCommands(IBot bot)
        {
            var helpCommands = CommandBuilder.From(typeof(LarryHelpCommands));            
            bot.CommandsList.Add(helpCommands);
        }

        private void LoadConfig(IConfigurationRoot applicationConfig)
        {
            string tempPath = Path.GetTempPath();
            string fileName = "channels.json";
            string fullPath = Path.Combine(tempPath, fileName);
            if (!File.Exists(fullPath))
            {               
                var defaultData = new List<Channel>();
                string jsonData = JsonSerializer.Serialize(defaultData);
                File.WriteAllText(fullPath, jsonData);   
            }
            string fileContent = File.ReadAllText(fullPath);
            var channels = JsonSerializer.Deserialize<List<Channel>>(fileContent);
            Options.ModNotificationsChannel = applicationConfig.GetValue<ulong>("modChannel");
            Options.Channels = channels;
        }
    }
}
