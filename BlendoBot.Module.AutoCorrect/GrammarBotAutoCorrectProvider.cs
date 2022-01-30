using BlendoBot.Core.Entities;
using BlendoBot.Core.Services;
using BlendoBot.Module.AutoCorrect.Schemas;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlendoBot.Module.AutoCorrect;

internal class GrammarBotAutoCorrectProvider : IAutoCorrectProvider {
	private const string endpoint = @"http://api.grammarbot.io/v2/check";

	public GrammarBotAutoCorrectProvider(ILogger logger, string apiKey = null) {
		HttpClient = new();

		ApiKey = apiKey;
		Logger = logger;
	}

	public async Task<string> CorrectAsync(string input) {
		try {
			string escaped = Uri.EscapeDataString(input);
			string uriString;
			if (!string.IsNullOrEmpty(ApiKey)) {
				uriString = $"{endpoint}?api_key={ApiKey}&language=en-AU&text={escaped}";
			} else {
				uriString = $"{endpoint}?language=en-AU&text={escaped}";
			}

			Uri uri = new(uriString);
			HttpResponseMessage httpResponse = await HttpClient.GetAsync(uri).ConfigureAwait(false);
			httpResponse.EnsureSuccessStatusCode();

			string responseJson = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
			GrammarBotResponse response = JsonConvert.DeserializeObject<GrammarBotResponse>(responseJson);

			// Incrementally replace matches from the input string with their replacements
			string outputString = input;
			foreach (GrammarBotMatch match in response.matches) {
				if (match.replacements.Count == 0) continue;
				string bestReplacement = match.replacements[0].value;
				string substringToReplace = input.Substring(match.offset, match.length);
				int offsetInOutputString = outputString.IndexOf(substringToReplace);
				outputString = outputString[..offsetInOutputString]
								+ bestReplacement
								+ outputString[(offsetInOutputString + substringToReplace.Length)..];
			}

			return outputString;
		} catch (Exception ex) {
			Logger.Log(this, new LogEventArgs {
				Type = LogType.Error,
				Message = $"Exception occurred in GrammarBotAutoCorrectProvider.CorrectAsync: {ex}"
			});
			return string.Empty;
		}
	}

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			HttpClient?.Dispose();
		}
	}

	private HttpClient HttpClient { get; init; }
	private string ApiKey { get; init; }
	private ILogger Logger { get; init; }
}
