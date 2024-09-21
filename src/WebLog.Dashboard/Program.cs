using Grpc.Net.Client;
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
    options.Address = new Uri("http://localhost:5267");
    
}) .ConfigureChannel(options => 
{  
    options.MaxReceiveMessageSize = 40 * 1024 * 1024; 
    options.MaxSendMessageSize = 50 * 1024 * 1024; 
})
    
    .ConfigurePrimaryHttpMessageHandler(
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


app.Run();