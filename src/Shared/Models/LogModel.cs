namespace Shared.Models
{
	public sealed record LogModel
	{
		public int EventId { get; set; }

		public DateTime Timestamp { get; set; }

		public FileLogLevel LogLevel { get; set; }

		public LogColor Color { get; set; }

        public object? State { get; set; }

        public string? Exception { get; set; }
        
        public int LineNumber { get; set; }
    }
}
