using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PauseManager : MonoBehaviour
{
    private static PauseManager instance;

    private PauseCallback onPauseStateChanged;
    public static PauseCallback OnPauseStateChanged
    {
        get { return instance.onPauseStateChanged; }
        set { instance.onPauseStateChanged = value; }
    }

    private PauseStateCallback isAutoUnpauseAllowed;
    public static PauseStateCallback IsAutoUnpauseAllowed
    {
        get { return instance.isAutoUnpauseAllowed; }
        set { instance.isAutoUnpauseAllowed = value; }
    }

    private bool pauseState = false;
    public static bool PauseState
    {
        get { return instance.pauseState; }
    }

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    
    private void OnSceneChanged(Scene prevScene, Scene currentScene)
    {
        SetPause(false);
    }

    private void OnApplicationPause(bool pause)
    {
#if SHOW_PROTOTYPE_LOGS
        Debug.Log("[PauseManager]: OnApplicationPause " + pause);
#endif

        if (isAutoUnpauseAllowed != null)
        {
            bool allowUnpause = false;

            foreach (var handler in isAutoUnpauseAllowed.GetInvocationList())
            {
                allowUnpause |= (bool)handler.DynamicInvoke();
                
                if (allowUnpause)
                    break;
            }

            if (pause && !allowUnpause)
            {
                SetPause(true);
            }
            else if (!pause && allowUnpause)
            {
                SetPause(false);
            }
        }
        else
        {
            if (pause && !pauseState)
            {
                SetPause(true);
            }
            else
            {
                SetPause(false);
            }
        }
    }

    public static void TogglePause()
    {
        SetPause(!PauseState);
    }
    
    public static void SetPause(bool state, bool useCustomCallbacks = true)
    {
        if (state == instance.pauseState)
            return;
        
#if SHOW_PROTOTYPE_LOGS
        Debug.Log("[PauseManager]: Pause state - " + state);
#endif

        if (state)
        {
            Time.timeScale = 0;

            instance.CustomPauseMethods();
        }
        else
        {
            Time.timeScale = 1;

            instance.CustomResumeMethods();
        }

        if (useCustomCallbacks && OnPauseStateChanged != null)
            OnPauseStateChanged.Invoke(state);

        instance.pauseState = state;
    }

    public delegate void PauseCallback(bool state);
    public delegate bool PauseStateCallback();
}
