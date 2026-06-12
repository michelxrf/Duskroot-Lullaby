using CombatSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject optionsCanvas;
    [SerializeField] private Animator animator;

    private bool paused;
    private PlayerSetup localPlayer;
    private PlayerControls controls;
    private PlayerHealth localPlayerHealth;
    protected PlayerVFX playerVFX;

    private void Awake()
    {
        playerVFX = GetComponent<PlayerVFX>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.InGame.Pause.performed +=
            OnPausePressed;
    }

    private void OnDisable()
    {
        controls.InGame.Pause.performed -=
            OnPausePressed;
    }

    private void OnPausePressed(
        InputAction.CallbackContext ctx)
    {
        if (paused)
            Resume();
        else
            OpenPause();
    }
    public void OpenPause()
    {
        paused = true;

        pauseCanvas.SetActive(true);

        animator.ResetTrigger("Close");
        animator.SetTrigger("Open");

        localPlayer = FindLocalPlayer();

        if (localPlayer != null)
        {
            localPlayer.EnablePlayerControlsLocal(false);
        }

        //

        localPlayer = FindLocalPlayer();

        if (localPlayer != null)
        {
            localPlayer.EnablePlayerControlsLocal(false);
            Animator playerAnimator = localPlayer.GetComponentInChildren<Animator>();
            localPlayerHealth = localPlayer.GetComponent<PlayerHealth>();

            if (localPlayerHealth != null)
            {
                playerAnimator.SetFloat("Speed", 0f);
                localPlayerHealth.IsInvulnerable = true;
                localPlayer.GetComponent<PlayerVFX>()?.Play(PlayerVFXEvent.PauseOpen);
            }
        }

    }

    public void Resume()
    {
        StartCoroutine(ClosePauseRoutine());
    }
    private IEnumerator ClosePauseRoutine()
    {
        ReadableCard.CloseAllCards();
        paused = false;

        animator.ResetTrigger("Open");
        animator.SetTrigger("Close");

        if (localPlayer != null && !localPlayerHealth.IsDead())
        {
            localPlayer.EnablePlayerControlsLocal(true);
            localPlayerHealth.IsInvulnerable = false;
            localPlayer.GetComponent<PlayerVFX>()?.Play(PlayerVFXEvent.PauseClose);
        }

        yield return new WaitForSeconds(0.4f);

        pauseCanvas.SetActive(false);
    }
    private PlayerSetup FindLocalPlayer()
    {
        PlayerSetup[] players =
            FindObjectsByType<PlayerSetup>(
                FindObjectsSortMode.None);

        foreach (PlayerSetup player in players)
        {
            if (player.IsLocalPlayer())
                return player;
        }

        return null;
    }
    public void OpenOptions()
    {
        optionsCanvas.SetActive(true);
    }
    public async void BackToMenu()
    {
        if (RunnerBootstrap.Instance != null)
        {
            await RunnerBootstrap.Instance.Runner.Shutdown();
        }

        SceneManager.LoadScene("MainMenu1");
    }


}