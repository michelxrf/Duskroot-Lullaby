using UnityEngine;

public class LoadNetworkedScene : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        RunnerBootstrap.Instance.LoadScene(sceneName);
    }
}
