using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    //
    private Image m_image;

    private void Awake()
    {
        m_image = GetComponent<Image>();
    }

    private void Start()
    {
        m_image.fillAmount = 0;
        StartCoroutine(LoadSceneRoutine(LoadingSceneData.NextSceneToLoad));
    }

    private IEnumerator LoadSceneRoutine(SceneLibrary nextSceneToLoad)
    {
        AsyncOperation loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)nextSceneToLoad);

        while(loadingOperation.progress <= .9f)
        {
            m_image.fillAmount = (loadingOperation.progress / .9f);

            yield return null;
        }

        GameManager.UnFreezeScreen();
        loadingOperation.allowSceneActivation = true;
    }
}
