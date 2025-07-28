#pragma warning disable 0649

using UnityEngine;

public class PausePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;

    private void OnEnable()
    {
        PauseManager.OnPauseStateChanged += OnPause;
    }

    private void OnDisable()
    {
        PauseManager.OnPauseStateChanged -= OnPause;
    }

    public void OnPause(bool state)
    {
        pausePanel.SetActive(state);
    }

    public void ResumeButton()
    {
        PauseManager.SetPause(false);
    }
}
