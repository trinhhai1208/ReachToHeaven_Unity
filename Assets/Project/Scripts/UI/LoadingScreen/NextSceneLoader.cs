using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoader : MonoBehaviour
{
    //
    [Tooltip("Chose the next scene will be loaded")]
    [SerializeField] private SceneLibrary m_nextScene;

    [Tooltip("Switch to LoadScene first")]
    public void LoadNextScene()
    {
        GameManager.UnFreezeScreen();
        LoadingSceneData.NextSceneToLoad = m_nextScene;
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)SceneLibrary.LoadingScene, LoadSceneMode.Additive);
    }
}
