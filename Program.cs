using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor2048;
using Blazor2048.GameLogic;
using Blazor2048.Core;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ロギングの設定
builder.Services.AddLogging(builder => builder
    .SetMinimumLevel(LogLevel.Information));

// 依存関係の注入
builder.Services.AddScoped<IRandomGenerator, DefaultRandomGenerator>();
builder.Services.AddScoped<IGameManager, GameManager>();

try
{
    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Application startup failed: {ex}");
}