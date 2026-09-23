///<summary>
///Holds the map, mode, difficulty and seed chosen for a single run.
///Set by the map/mode selection screen, read by WaveDirector/MapLoader.
///Plain C# holder so it survives a scene load without a MonoBehaviour.
/// </summary>
public class RunConfig
{
    //
    public MapDefinition Map;
    public GameModeDefinition Mode;
    public LevelDefinition Level;
    public float DifficultyMultiplier = 1f;
    public int Seed;

    public static RunConfig Current { get; private set; }

    public static void Set(RunConfig config) => Current = config;

    public static void Clear() => Current = null;
}
