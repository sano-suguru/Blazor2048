using Blazor2048.Core;

namespace Blazor2048.Services;

public interface IGameStateService
{
    ValueTask<GameState?> LoadGameStateAsync();
    ValueTask SaveGameStateAsync(GameState state);
    ValueTask ClearGameStateAsync();
}