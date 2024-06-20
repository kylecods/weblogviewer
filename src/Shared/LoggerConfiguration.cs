using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared
{
	public sealed class LoggerConfiguration
	{
		public int EventId { get; set; }

		public Dictionary<LogLevel, LogColor> LogColors { get; set; } = new()
		{
			[LogLevel.Information] = new () { CssBackgroundColor = "#AAFF00" },
			[LogLevel.Error] = new() { CssBackgroundColor = "#EE4B2B" },
			[LogLevel.Warning] = new() { CssBackgroundColor = "#ffbd33" }
		};

	}
}
