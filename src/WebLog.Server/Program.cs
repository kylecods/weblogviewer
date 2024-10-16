﻿using CliWrap;
using WebLog.Server;
using WebLog.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = int.MaxValue; 
    options.MaxSendMessageSize = int.MaxValue;
});

builder.Services.AddWindowsService();
builder.Services.AddHostedService<ServerWorker>();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

var app = builder.Build();

app.UseGrpcWeb();
app.UseCors();

app.MapGrpcService<FileLogService>().EnableGrpcWeb().RequireCors("AllowAll");

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");

const string ServiceName = "WebLog.Server";

if (args is { Length: 1 })
{
    try
    {
        string executablePath =
            Path.Combine(AppContext.BaseDirectory, "WebLog.Dashboard.exe");

        if (args[0] is "/Install")
        {
            await Cli.Wrap("sc")
                .WithArguments(["create", ServiceName, $"binPath={executablePath}", "start=auto"])
                .ExecuteAsync();

            await Cli.Wrap("sc")
               .WithArguments(["start", ServiceName])
               .ExecuteAsync();
        }
        else if (args[0] is "/Uninstall")
        {
            await Cli.Wrap("sc")
               .WithArguments(["stop", ServiceName])
               .ExecuteAsync();

            await Cli.Wrap("sc")
                .WithArguments(["delete", ServiceName])
                .ExecuteAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    return;
}

app.Run();

