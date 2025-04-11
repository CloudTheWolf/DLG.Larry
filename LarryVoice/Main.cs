using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.EventArgs;
using LarryVoice.Actions;
using LarryVoice.Events;
using LarryVoice.Types;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using ILogger = Serilog.ILogger;

namespace LarryVoice
{
    public class Main : IPlugin
    {
        public string Name => "Larry Voice";

        public string Description => "Voice commands / actions for Larry";

        public int Version => 1;

        public void InitPlugin(IBot bot, ILogger logger, DiscordConfiguration discordConfiguration,
            IConfigurationRoot applicationConfig)
        {
            Options.LogPrefix = Name;
            Logger.Initialize();
            LoadConfig(applicationConfig);
            RegisterCommands(bot);
            bot.EventHandlerRegistry.Register(e => e
                    .HandleMessageCreated(MessageCreated.CleanupChannels)
                    );


        }

        private void RegisterCommands(IBot bot)
        {
            var gmCommands = CommandBuilder.From(typeof(LarryGameMasterCommands));
            var luminaryCommands = CommandBuilder.From(typeof(LarryLuminaryCommands));
            bot.CommandsList.Add(gmCommands);
            bot.CommandsList.Add(luminaryCommands);
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
