using System.Text.RegularExpressions;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace Shared.FileLogs;

public static partial class FileLogReader
{
    public static async IAsyncEnumerable<IReadOnlyList<LogLine>> ReadAsync(string path)
    {
        var channel = Channel.CreateUnbounded<LogLine>(
            new UnboundedChannelOptions { AllowSynchronousContinuations = false, SingleReader = true, SingleWriter = true });
        
            bool TryParseOutTime(string line, out TimeParseResult? result)
            {
                //parse timestamp
                var match = GenerateRfc3339RegEx().Match(line);

                if (!match.Success)
                {
                    result = default;
                    return false;
                }
                var span = line.AsSpan();

                ReadOnlySpan<char> content;
            
                content = span[(match.Index + match.Length)..]; // Trim whitespace added by logging between timestamp and content.
                if (char.IsWhiteSpace(content[0]))
                {
                    content = content.Slice(1);
                }

                result = new(content.ToString(), DateTime.Parse(match.ValueSpan));
                
                return true;
            }

            bool TryParseOutLogLevel(string lineContent, out LogLevelResult? result)
            {
                var match = MatchLogLevel().Match(lineContent);
                
                if (!match.Success)
                {
                    result = default;
                    return false;
                }
                
                var span = lineContent.AsSpan();

                ReadOnlySpan<char> content;
            
                content = span[(match.Index + match.Length)..]; 
                
                if (char.IsWhiteSpace(content[0]))
                {
                    content = content.Slice(1);
                }

                var logLevel = match.ValueSpan.ToString() switch
                {
                    "[Debug]" => LogLevel.Debug,
                    "[Information]" => LogLevel.Information,
                    "[Error]" => LogLevel.Error,
                    "[Warning]" => LogLevel.Warning,
                    _ => LogLevel.None
                };

                result = new(content.ToString(), logLevel);

                return true;
            }

            static LogLine[] ToLogLineArray(IReadOnlyList<LogLine> lines)
            {
                var logLines = new LogLine[lines.Count];
                for (int i = 0; i < lines.Count; i++)
                {
                    logLines[i] = lines[i];
                }

                return logLines;
            }
            
            int lineNumber = 1;
            var readTask = Task.Run(async () =>
            {
                try
                {
                    await foreach (var fileLine in File.ReadLinesAsync(path))
                    {
                        var content = fileLine;

                        DateTime? timestamp = null;

                        LogLevel logLevel = LogLevel.None;

                        //we parse time
                        if (TryParseOutTime(content, out var timeParseResult))
                        {
                            timestamp = timeParseResult!.Value.timestamp;
                            content = timeParseResult.Value.ModifiedText;
                        }

                        //we parse log level
                        if (TryParseOutLogLevel(content, out var logLevelResult))
                        {
                            logLevel = logLevelResult!.Value.LogLevel;
                            content = logLevelResult.Value.Content;
                        }

                        LogLine logLine = new(lineNumber,content, logLevel, timestamp);
                        channel.Writer.TryWrite(logLine);

                        lineNumber++;
                    }
                }
                finally
                {
                    channel.Writer.TryComplete();
                }
            });

            await foreach (var entries in channel.GetBatchesAsync().ConfigureAwait(false))
            {
                yield return ToLogLineArray(entries);
            }


            await readTask.ConfigureAwait(false);


    }
    
    [GeneratedRegex(@"(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})\.(\d{3}) ([+-]\d{2}:\d{2})")]
    private static partial Regex GenerateRfc3339RegEx();
    
    [GeneratedRegex(@"(\[(Information|Debug|Error)\])")]
    private static partial Regex MatchLogLevel();
    
    public record struct TimeParseResult(string ModifiedText, DateTime timestamp);
    
    public record struct LogLevelResult(string Content, LogLevel LogLevel);
    public record struct LogLine(int lineNumber, string Content, LogLevel LogLevel, DateTime? timestamp);
    
}