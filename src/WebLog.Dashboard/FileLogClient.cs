using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using Proto;

namespace WebLog.Dashboard;

public class FileLogClient
{
    private readonly GrpcChannel _channel;

    private readonly ILogger<FileLogClient> _logger;

    private readonly FileService.FileServiceClient _client;

    public FileLogClient(ILoggerFactory loggerFactory)
    {
        var address ="http://localhost:5267";
        
        _logger = loggerFactory.CreateLogger<FileLogClient>();
        var httpHandler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            KeepAlivePingDelay = TimeSpan.FromSeconds(20),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests
        };
        
        var methodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };
        _logger.LogDebug("Dashboard configured to connect to: {Address}", address);

        _channel = GrpcChannel.ForAddress(
            address,
            channelOptions: new()
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                ServiceConfig = new() { MethodConfigs = { methodConfig } },
                LoggerFactory = loggerFactory,
                ThrowOperationCanceledOnCancellation = true
            });

        _client = new FileService.FileServiceClient(_channel);
    }
    
}