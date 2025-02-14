using Microsoft.AspNetCore.Components.Web;

namespace Blazor2048.Core;

public readonly record struct Move(Direction Direction)
{
    public static Move? FromKeyboardEvent(KeyboardEventArgs e) => e.Key switch
    {
        "ArrowUp" => new Move(Direction.Up),
        "ArrowDown" => new Move(Direction.Down),
        "ArrowLeft" => new Move(Direction.Left),
        "ArrowRight" => new Move(Direction.Right),
        _ => null
    };

    public static Move? FromSwipeEvent(int deltaX, int deltaY)
    {
        if (Math.Abs(deltaX) > Math.Abs(deltaY))
        {
            if (Math.Abs(deltaX) < GameConstants.MinimumSwipeDistance) return null;
            return deltaX > 0 ? new Move(Direction.Right) : new Move(Direction.Left);
        }
        else
        {
            if (Math.Abs(deltaY) < GameConstants.MinimumSwipeDistance) return null;
            return deltaY > 0 ? new Move(Direction.Down) : new Move(Direction.Up);
        }
    }
}
