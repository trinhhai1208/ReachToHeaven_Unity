using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoader : MonoBehaviour
{
    //
    [Tooltip("Chose the next scene will be loaded")]
    [SerializeField] private SceneLibrary m_nextScene;

    public SceneLibrary NextScene { get => m_nextScene; set => m_nextScene = value; }

    [Tooltip("Switch to LoadScene first")]
    public void LoadNextScene()
    {
        LoadingSceneData.NextSceneToLoad = m_nextScene;
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)SceneLibrary.LoadingScene, LoadSceneMode.Additive);
    }
}
