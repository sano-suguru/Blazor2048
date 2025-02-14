using Blazor2048.Core;
using Blazor2048.GameLogic;

namespace Blazor2048.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        // シングルトンとして登録するサービス
        services.AddSingleton<IRandomGenerator, DefaultRandomGenerator>();

        // スコープとして登録するサービス
        services.AddScoped<IGameManager, GameManager>();
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<IGameStateService, GameStateService>();

        return services;
    }
}
