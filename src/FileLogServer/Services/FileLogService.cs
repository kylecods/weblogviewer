using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Proto;
using Shared.FileLogs;

namespace FileLogServer.Services;

public class FileLogService : FileService.FileServiceBase
{
    public override Task<FileResponse> Fetch(FileRequest request, ServerCallContext context)
    {
        var result = FileLogReader.Read(request.FilePath);

        return Task.FromResult(new FileResponse
        {
            Content = result[0].Item1,
            TimeStamp = result[0].Item2.ToUniversalTime().ToTimestamp()
        });
    }
}