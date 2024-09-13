using Grpc.Net.Client.Web;
using Proto;
using Shared.Data;
using WebLog.Dashboard.Components;
using WebLog.Dashboard.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IDataStore, LogDataStore>();
builder.Services.AddSingleton<WebLogViewModel>();
builder.Services.AddGrpcClient<FileService.FileServiceClient>(options =>
{
    options.Address = new Uri("http://localhost:5000");
}).ConfigurePrimaryHttpMessageHandler(
    () => new GrpcWebHandler(new HttpClientHandler()));

builder.Services.AddGrpc();

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


var client = app.Services.GetRequiredService<FileService.FileServiceClient>();

var response = await client.FetchAsync(new FileRequest { FilePath = "C:\\Chui_logs\\Chui-winsvc-log-20240912_011.log"});

app.Logger.LogInformation("{Timestamp} {Message}",response.TimeStamp,response.Content);

app.Run();