using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor2048;
using Blazor2048.GameLogic;
using Blazor2048.Core;
using Blazor2048.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ロギングの設定
builder.Services.AddLogging(builder => builder
    .SetMinimumLevel(LogLevel.Information)
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("System", LogLevel.Warning));

// カスタムサービスの登録
builder.Services.AddGameServices();

try
{
    var app = builder.Build();
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Application startup failed: {ex}");
}