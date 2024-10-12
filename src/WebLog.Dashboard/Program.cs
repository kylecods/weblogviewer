using CliWrap;
using Grpc.Net.Client.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Proto;
using Shared.Data;
using WebLog.Dashboard;
using WebLog.Dashboard.Components;
using WebLog.Dashboard.ViewModels;



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<IDataStore,LogDataStore>();
builder.Services.AddTransient<WebLogViewModel>();

var dashboardOptions = builder.Configuration.GetSection("DashboardOptions");


builder.Services.AddGrpcClient<FileService.FileServiceClient>(options =>
{
    var url = dashboardOptions.GetValue<string>("Url");

    options.Address = new Uri(url!);
    
}) .ConfigureChannel(options => 
{  
    options.MaxReceiveMessageSize = int.MaxValue; 
    options.MaxSendMessageSize = int.MaxValue; 
})
    
    .ConfigurePrimaryHttpMessageHandler(
    () => new GrpcWebHandler(new HttpClientHandler()));

builder.Services.AddGrpc();

builder.Services.AddFluentUIComponents();

builder.Services.AddWindowsService();
builder.Services.AddHostedService<DashboardWorker>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


const string ServiceName = "WebLog.Dashboard";

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
