namespace Blazor2048.Core;

public class DefaultRandomGenerator : IRandomGenerator
{
    private readonly Random _random = new();
    public int Next(int maxValue) => _random.Next(maxValue);
}
