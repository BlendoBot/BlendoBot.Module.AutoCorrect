using BlendoBot.Core.Entities;
using BlendoBot.Core.Module;
using BlendoBot.Core.Services;
using System;
using System.Threading.Tasks;

namespace BlendoBot.Module.AutoCorrect;

[Module(Guid = "com.biendeo.blendobot.module.autocorrect", Name = "AutoCorrect", Author = "Mozzarella", Version = "2.0.0", Url = "https://github.com/BlendoBot/BlendoBot.Module.AutoCorrect")]
public class AutoCorrect : IModule, IDisposable {
	public AutoCorrect(IConfig config, IDiscordInteractor discordInteractor, IModuleManager moduleManager, ILogger logger) {
		Config = config;
		DiscordInteractor = discordInteractor;
		ModuleManager = moduleManager;
		Logger = logger;

		AutoCorrectCommand = new(this);
	}

	internal ulong GuildId { get; private set; }

	internal readonly AutoCorrectCommand AutoCorrectCommand;
	internal IAutoCorrectProvider AutoCorrectProvider;

	internal readonly IConfig Config;
	internal readonly IDiscordInteractor DiscordInteractor;
	internal readonly IModuleManager ModuleManager;
	internal readonly ILogger Logger;


	public Task<bool> Startup(ulong guildId) {
		GuildId = guildId;
		string apiKey = Config.ReadConfig(this, "AutoCorrect", "ApiKey");
		if (apiKey == null) {
			Logger.Log(this, new LogEventArgs {
				Type = LogType.Log,
				Message = $"BlendoBot Currency Converter has not been supplied a valid API key. You can raise the rate limit from 100 to 250 by acquiring a key from https://www.grammarbot.io/ and adding it in the config under the [AutoCorrect] section."
			});
		}
		AutoCorrectProvider = new GrammarBotAutoCorrectProvider(Logger, apiKey);
		return Task.FromResult(ModuleManager.RegisterCommand(this, AutoCorrectCommand, out _));
	}

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			AutoCorrectProvider?.Dispose();
		}
	}
}
