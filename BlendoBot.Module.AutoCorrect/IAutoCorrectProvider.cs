using System;
using System.Threading.Tasks;

namespace BlendoBot.Module.AutoCorrect;

internal interface IAutoCorrectProvider : IDisposable {
	Task<string> CorrectAsync(string input);
}
