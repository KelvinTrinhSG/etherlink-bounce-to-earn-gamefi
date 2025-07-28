using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoaderButton : MonoBehaviour
{
    [Header("Button dùng để chuyển scene")]
    public Button loadSceneButton;

    public string targetSceneName;

    private void Start()
    {
        if (loadSceneButton != null)
        {
            loadSceneButton.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogWarning("❗ Button hoặc tên scene chưa được gán trong SceneLoaderButton.");
        }
    }

    private void LoadScene()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("❌ PlayerDataManager.Instance is null.");
            return;
        }

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("❌ Tên scene chưa được gán.");
        }
    }

}
