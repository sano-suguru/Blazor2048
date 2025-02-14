namespace Blazor2048.Core;

public class TileMergedEventArgs : EventArgs
{
    public Position Position { get; }
    public int OldValue { get; }
    public int NewValue { get; }

    public TileMergedEventArgs(Position position, int oldValue, int newValue)
    {
        Position = position;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
