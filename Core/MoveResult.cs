namespace Blazor2048.Core;

/// <summary>
/// 移動操作の結果を表すValue Object
/// </summary>
public readonly record struct MoveResult
{
    public bool HasMoved { get; init; }
    public int TilesMoved { get; init; }
    public int TilesMerged { get; init; }
    public Score ScoreGained { get; init; }
    public IReadOnlyList<TileMergedEventArgs> MergeEvents { get; init; }

    public MoveResult(
        bool hasMoved,
        int tilesMoved = 0,
        int tilesMerged = 0,
        Score? scoreGained = null,
        IEnumerable<TileMergedEventArgs>? mergeEvents = null)
    {
        HasMoved = hasMoved;
        TilesMoved = tilesMoved;
        TilesMerged = tilesMerged;
        ScoreGained = scoreGained ?? new Score(0);
        MergeEvents = mergeEvents?.ToList() ?? [];
    }

    public static MoveResult NoMove() => new(false);

    public static MoveResult Success(
        int tilesMoved,
        int tilesMerged,
        Score scoreGained,
        IEnumerable<TileMergedEventArgs> mergeEvents) =>
        new(true, tilesMoved, tilesMerged, scoreGained, mergeEvents);

    public MoveResult WithScoreGained(Score additionalScore) =>
        this with { ScoreGained = new Score(ScoreGained.Value + additionalScore.Value) };

    public override string ToString() =>
        HasMoved
            ? $"Moved: {TilesMoved} tiles, Merged: {TilesMerged} tiles, Score: +{ScoreGained.Value}"
            : "No movement";
}
