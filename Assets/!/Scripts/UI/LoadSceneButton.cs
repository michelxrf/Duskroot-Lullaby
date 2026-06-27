using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public async void LoadScene()
    {
        if (RunnerBootstrap.Instance != null)
        {
            await RunnerBootstrap.Instance.Runner.Shutdown();
        }

        SceneManager.LoadScene(sceneName);
    }
}