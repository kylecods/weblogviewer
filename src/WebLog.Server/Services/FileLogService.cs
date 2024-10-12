using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Proto;
using Shared.FileLogs;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace WebLog.Server.Services;

public class FileLogService(ILogger<FileLogService> logger) : FileService.FileServiceBase
{
    public override async Task Fetch(FileRequest request, IServerStreamWriter<FileResult> responseStream, ServerCallContext context)
    {
        var result = new FileResult();
        
        if (!File.Exists(request.FilePath))
        {
            logger.LogError("NO FILE FOUND");
            
            await responseStream.WriteAsync(result).ConfigureAwait(false);
            
            return;
        }
        
        await foreach (var logLines in FileLogReader.ReadAsync(request.FilePath).ConfigureAwait(false))
        {
            foreach (var (lineNumber,content,logLevel, timestamp) in logLines)
            {
                result.LogLines.Add(new FileResponse
                {
                    Content = content,
                    TimeStamp = timestamp?.ToUniversalTime().ToTimestamp() ?? DateTime.UtcNow.ToTimestamp(),
                    LogLevel = logLevel switch
                    {
                        LogLevel.Information => Proto.LogLevel.Information,
                        LogLevel.Debug => Proto.LogLevel.Debug,
                        LogLevel.Warning => Proto.LogLevel.Warning,
                        LogLevel.Error => Proto.LogLevel.Error,
                        _ => Proto.LogLevel.None
                    },
                    LineNumber = lineNumber
                });
            }

            await responseStream.WriteAsync(result).ConfigureAwait(false);
        }
    }
}