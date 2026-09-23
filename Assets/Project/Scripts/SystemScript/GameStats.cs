public static class GameStats
{
    public static int KillCount { get; private set; }
    public static float ElapsedTime { get; private set; }

    public static void Reset() { KillCount = 0; ElapsedTime = 0f; }
    public static void AddKill() => ++KillCount;
    public static void SetElapsed(float t) => ElapsedTime = t;
}
