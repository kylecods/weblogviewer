using System.Globalization;
using System.Text.RegularExpressions;

namespace Shared.FileLogs;

public static partial class FileLogReader
{
    public static Tuple<string, DateTime>[] Read(string path)
    {
        var result = new Tuple<string, DateTime>[2];
        foreach (var line in File.ReadAllLines(path))
        {
            //parse timestamp
            var match = GenerateRfc3339RegEx().Match(line);

            if (!match.Success) continue;
            
            var span = line.AsSpan();

            ReadOnlySpan<char> content;
            
                content = span[(match.Index + match.Length)..]; // Trim whitespace added by logging between timestamp and content.
                if (char.IsWhiteSpace(content[0]))
                {
                    content = content.Slice(1);
                }

            result[0] = new Tuple<string, DateTime>(content.ToString(),
                DateTime.Parse(match.ValueSpan, CultureInfo.InvariantCulture));
            result[1] = new Tuple<string, DateTime>(content.ToString(),
                DateTime.Parse(match.ValueSpan, CultureInfo.InvariantCulture));
            
        }

        return result;

    }
    
    [GeneratedRegex(@"(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})\.(\d{3}) ([+-]\d{2}:\d{2})")]
    private static partial Regex GenerateRfc3339RegEx();
    
    public record struct DateTimeResult(string ModifiedText, DateTime timestamp);
}