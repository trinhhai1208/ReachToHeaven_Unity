using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    private TextMeshProUGUI m_statsText;
    private bool m_initialized;

    ///<summary>
    ///Build the button labels and stats text once, lazily. Runs even while the panel is inactive
    ///because Active() is the first time this component is ever touched.
    /// </summary>
    private void EnsureInit()
    {
        if (m_initialized) return;
        m_initialized = true;

        Button[] buttons = GetComponentsInChildren<Button>(true);
        if (buttons.Length >= 1)
        {
            Vector2 pos = buttons[0].GetComponent<RectTransform>().anchoredPosition;
            AddLabel("Menu", new Vector2(pos.x, pos.y - 65));

            var loader = buttons[0].GetComponent<NextSceneLoader>();
            if (loader != null) loader.NextScene = SceneLibrary.MapSelect;
        }
        if (buttons.Length >= 2)
        {
            Vector2 pos = buttons[1].GetComponent<RectTransform>().anchoredPosition;
            AddLabel("Play Again", new Vector2(pos.x, pos.y - 65));

            var loader = buttons[1].GetComponent<NextSceneLoader>();
            if (loader != null) loader.NextScene = SceneLibrary.GamePlay;
        }
        m_statsText = AddStatsText();
    }

    private void AddLabel(string text, Vector2 anchoredPos)
    {
        var go = new GameObject("BtnLabel");
        go.transform.SetParent(transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(200, 35);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }

    private TextMeshProUGUI AddStatsText()
    {
        var go = new GameObject("Stats");
        go.transform.SetParent(transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 70);
        rt.sizeDelta = new Vector2(280, 40);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 16;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return tmp;
    }

    public void Active()
    {
        GameManager.FreezeScreen();

        EnsureInit();

        foreach (var btn in GetComponentsInChildren<Button>(true))
        {
            btn.interactable = true;
            btn.onClick.RemoveListener(OnButtonClicked);
            btn.onClick.AddListener(OnButtonClicked);
        }

        int min = (int)GameStats.ElapsedTime / 60;
        int sec = (int)GameStats.ElapsedTime % 60;
        m_statsText.text = $"SURVIVED  {min:D2}:{sec:D2}   KILLED  {GameStats.KillCount}";

        gameObject.SetActive(true);
    }

    private void OnButtonClicked()
    {
        foreach (var btn in GetComponentsInChildren<Button>(true))
            btn.interactable = false;
    }

    public void Deactive()
    {
        GameManager.UnFreezeScreen();
        gameObject.SetActive(false);
    }
}
