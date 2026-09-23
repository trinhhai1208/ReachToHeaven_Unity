using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

///<summary>
///Attach to a main-menu "start" button that has been repurposed as a mode entry.
///On click it records the chosen mode into RunConfig; the button's existing onClick
///(the menu's start/cutscene transition) still runs and carries the run into GamePlay,
///where WaveDirector reads RunConfig.Current.Mode.
/// </summary>
[RequireComponent(typeof(Button))]
public class ModeStartButton : MonoBehaviour
{
    //
    [SerializeField] private GameModeDefinition m_mode;
    [SerializeField] private MapDefinition m_defaultMap;
    [Tooltip("If true, navigates to the MapSelect / LevelSelect scene instead of continuing to GamePlay")]
    [SerializeField] private bool m_loadMapSelectScene = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        ApplyMode();
        if (m_loadMapSelectScene)
        {
            OpenMapSelect();
        }
    }

    public void ApplyMode()
    {
        if (m_mode == null)
        {
            return;
        }

        RunConfig.Set(new RunConfig
        {
            Map = m_defaultMap,
            Mode = m_mode,
            DifficultyMultiplier = 1f,
            Seed = Random.Range(int.MinValue, int.MaxValue)
        });
    }

    public void OpenMapSelect()
    {
        GameManager.UnFreezeScreen();
        LoadingSceneData.NextSceneToLoad = SceneLibrary.MapSelect;
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)SceneLibrary.LoadingScene, LoadSceneMode.Additive);
    }
}
