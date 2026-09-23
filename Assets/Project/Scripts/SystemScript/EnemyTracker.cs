public static class EnemyTracker
{
    public static int LiveCount { get; private set; }

    public static void Reset()
    {
        LiveCount = 0;
        GameEvents.TriggerEnemyCountChanged(0);
    }

    public static void OnActivated()
    {
        ++LiveCount;
        GameEvents.TriggerEnemyCountChanged(LiveCount);
    }

    public static void OnDeactivated()
    {
        if (LiveCount > 0) --LiveCount;
        GameEvents.TriggerEnemyCountChanged(LiveCount);
    }

    ///<summary>
    ///Called only from the real death path (not from a plain despawn/scene teardown),
    ///so KillCount reflects kills instead of every deactivation.
    /// </summary>
    public static void OnKilled()
        => GameStats.AddKill();
}
