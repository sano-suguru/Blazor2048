using Blazor2048.Core;

namespace Blazor2048.Services;

public interface IScoreService
{
    ValueTask<HighScore?> GetHighScoreAsync();
    ValueTask SaveHighScoreAsync(Score score);
    event EventHandler<HighScore>? HighScoreUpdated;
}