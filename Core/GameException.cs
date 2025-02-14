namespace Blazor2048.Core;

public class GameException : Exception
{
    public GameException(string message) : base(message) { }
    public GameException(string message, Exception inner) : base(message, inner) { }
}
