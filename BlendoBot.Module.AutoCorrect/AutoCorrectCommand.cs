using BlendoBot.Core.Command;
using BlendoBot.Core.Entities;
using BlendoBot.Core.Module;
using BlendoBot.Core.Utility;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlendoBot.Module.AutoCorrect;

internal class AutoCorrectCommand : ICommand {
	public AutoCorrectCommand(AutoCorrect module) {
		this.module = module;
	}

	private readonly AutoCorrect module;
	public IModule Module => module;

	public string Guid => "autocorrect.command";
	public string DesiredTerm => "ac";
	public string Description => "Performs autocorrect on a message";
	public Dictionary<string, string> Usage => new() {
		{ "[message]", "Corrects the given message" },
	};
		
	public async Task OnMessage(MessageCreateEventArgs e, string[] tokenizedInput) {
		await e.Channel.TriggerTypingAsync().ConfigureAwait(false);

		if (tokenizedInput.Length == 0) {
			await module.DiscordInteractor.Send(this, new SendEventArgs {
				Message = $"Too few arguments specified to {module.ModuleManager.GetHelpTermForCommand(this).Code()}",
				Channel = e.Channel,
				Tag = "AutoCorrectTooFewArgs"
			}).ConfigureAwait(false);
			return;
		}

		string inputMessage = string.Join(' ', tokenizedInput);
		string correctedMessage = await module.AutoCorrectProvider.CorrectAsync(inputMessage).ConfigureAwait(false);

		if (string.IsNullOrEmpty(correctedMessage)) {
			// uh oh
			await module.DiscordInteractor.Send(this, new SendEventArgs {
				Message = $"Failed to autocorrect '{inputMessage}",
				Channel = e.Channel,
				Tag = "AutoCorrectFailure"
			}).ConfigureAwait(false);
			return;
		}

		string commandOutput = $"'{inputMessage}' autocorrected to '{correctedMessage}'";
		await module.DiscordInteractor.Send(this, new SendEventArgs {
			Message = commandOutput,
			Channel = e.Channel,
			Tag = "AutoCorrectSuccess"
		}).ConfigureAwait(false);
	}
}
