using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion;


/// <summary>
/// Temporary solution to force player to wait Fusion to connect and set up the lobby before showing the actual lobby screen
/// </summary>
public class FakeLoadingScreen : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject connectionLostScreen;
    [SerializeField] LobbyManager lobbyManager;
    string baseText;

    private void Start()
    {
        RunnerBootstrap.Instance.OnPlayerConnected += SelfDestroy;
        RunnerBootstrap.Instance.OnFailedToConnect += FailedToConnect;

        baseText = text.text;
        StartCoroutine(AnimateDots());
    }

    private void OnDestroy()
    {
        RunnerBootstrap.Instance.OnPlayerConnected -= SelfDestroy;
        RunnerBootstrap.Instance.OnFailedToConnect -= FailedToConnect;
    }

    private void FailedToConnect()
    {
        StopAllCoroutines();
        connectionLostScreen.SetActive(true);
    }

    /// <summary>
    /// Destroy the loading screen and let player see lobby 
    /// </summary>
    void SelfDestroy()
    {
        Destroy(gameObject);
    }

    IEnumerator AnimateDots()
    {
        while (true)
        {
            text.text = baseText + "";
            yield return new WaitForSeconds(0.5f);
            text.text = baseText + ".";
            yield return new WaitForSeconds(0.5f);
            text.text = baseText + "..";
            yield return new WaitForSeconds(0.5f);
            text.text = baseText + "...";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
