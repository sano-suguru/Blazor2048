namespace Blazor2048.Core;

public static class GameConstants
{
    public const int BoardSize = 4;
    public const int InitialTileCount = 2;
    public const int NewTileProbability2 = 90;  // 90% chance for 2
    public const int MinimumSwipeDistance = 30;

    public static class Scores
    {
        public const int MergeTile2 = 4;
        public const int MergeTile4 = 8;
        public const int MergeTile8 = 16;
        public const int MergeTile16 = 32;
        public const int MergeTile32 = 64;
        public const int MergeTile64 = 128;
        public const int MergeTile128 = 256;
        public const int MergeTile256 = 512;
        public const int MergeTile512 = 1024;
        public const int MergeTile1024 = 2048;
    }

    public static class UI
    {
        public const string GameOverMessage = "ゲームオーバー！";
        public const string ScoreLabel = "スコア";
        public const string RestartButtonText = "もう一度プレイ";
    }
}
