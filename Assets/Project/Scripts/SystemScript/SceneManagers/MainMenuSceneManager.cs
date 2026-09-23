using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class MainMenuSceneManager : BaseSceneManager
{
    //
    [SerializeField] private AudioClip m_ambientAudio;
    [SerializeField] private GameObject m_mainMenu;

    protected override void Start()
    {
        base.Start();

        if (m_ambientAudio != null) AmbientAudioManager.Instance.PlayAmbientAudio(m_ambientAudio);
        CursorController.CanPlaySFX = true;
        SingletonUIManager.Instance.SettingPanel.SetSceneLayout(m_mainMenu, true, false);

        //Debug.Log("StartMainMenu: " + CursorController.CanPlaySFX);
    }
}
