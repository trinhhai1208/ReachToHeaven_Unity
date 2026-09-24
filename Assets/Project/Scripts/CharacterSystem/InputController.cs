using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, ISubject<Action<InputAction.CallbackContext>>
{
    //
    [SerializeField] private InputAction m_moveAction;
    [SerializeField] private InputAction m_attackAction;
    [SerializeField] private InputAction m_switchWeaponAction;
    [SerializeField] private InputAction m_pauseAction;

    public InputAction SwitchWeaponAction { get => m_switchWeaponAction; }
    public InputAction PauseAction { get => m_pauseAction; }

    private void Start()
    {
        m_moveAction.Enable();
        m_attackAction.Enable();
        m_switchWeaponAction.Enable();
        if (m_pauseAction != null)
        {
            m_pauseAction.Enable();
            m_pauseAction.performed += OnPause;
        }

        if (SingletonUIManager.Instance != null && SingletonUIManager.Instance.SettingPanel != null)
            SingletonUIManager.Instance.SettingPanel.SetInputEvent(EnableInput, DisableInput);
    }

    private void OnDestroy()
    {
        if (m_pauseAction != null)
        {
            m_pauseAction.performed -= OnPause;
            m_pauseAction.Disable();
        }
        m_moveAction?.Disable();
        m_attackAction?.Disable();
        m_switchWeaponAction?.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        var ui = SingletonUIManager.Instance;
        if (ui == null || ui.SettingPanel == null) return;

        // Do not pause if already at end game screens
        if ((ui.GameOverPanel != null && ui.GameOverPanel.gameObject.activeSelf) ||
            (ui.WinPanel != null && ui.WinPanel.gameObject.activeSelf))
            return;

        SettingPanel settingPanel = ui.SettingPanel;
        if (settingPanel.gameObject.activeSelf)
        {
            settingPanel.ExitSettingPanel();
        }
        else
        {
            // Do not pause if already frozen by another screen (e.g. card upgrade select)
            if (Time.timeScale == 0f) return;

            settingPanel.ActivePanel();
        }
    }

    public void EnableCursorClick()
        => m_attackAction.Enable();

    public void DisableCursorClick()
        => m_attackAction.Disable();

    public void EnableInput()
    {
        m_moveAction.Enable();
        m_attackAction.Enable();
        m_switchWeaponAction.Enable();
        m_pauseAction?.Enable();
    }

    public void DisableInput()
    {
        m_moveAction.Disable();
        m_attackAction.Disable();
        m_switchWeaponAction.Disable();
        // Keep m_pauseAction enabled so player can press P to resume / unpause
    }


    #region Implement ISubject
    public void Subscribe(Action<InputAction.CallbackContext> subscriber)
    {
        m_attackAction.started += subscriber;
    }

    public void UnSubscribe(Action<InputAction.CallbackContext> subscriber)
    {
        m_attackAction.started -= subscriber;
    }
    #endregion

    #region Movement
    public bool IsMoveActionPressing()
    {
        return m_moveAction.IsPressed();
    }

    ///<summary>
    ///Return input value from moveAction
    ///</summary>
    public Vector2 GetMoveInput()
    {
        return m_moveAction.ReadValue<Vector2>();
    }
    #endregion
}
