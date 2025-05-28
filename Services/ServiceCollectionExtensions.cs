using Blazor2048.Core;
using Blazor2048.Core.Commands;
using Blazor2048.GameLogic;

namespace Blazor2048.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        // シングルトンとして登録するサービス
        services.AddSingleton<IRandomGenerator, DefaultRandomGenerator>();
        services.AddSingleton<IMoveCommandFactory, MoveCommandFactory>();

        // スコープとして登録するサービス
        services.AddScoped<IScoreService, ScoreService>();
        services.AddScoped<IGameManager, GameManager>();
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<IGameStateService, GameStateService>();

        return services;
    }
}
