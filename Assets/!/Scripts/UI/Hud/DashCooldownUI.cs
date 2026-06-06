using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    private PlayerMovement player;

    [Header("UI")]
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private Transform dashIcon;

    private Coroutine popRoutine;

    private bool cooldownFinishedLastFrame;
    private bool initialized;

    private void Update()
    {
        if (player == null)
            return;

        float remaining =
            player.GetDashCooldownRemaining();

        float normalized =
            player.GetDashCooldownNormalized();

        cooldownOverlay.fillAmount =
            normalized;

        if (remaining > 0)
        {
            cooldownText.text =
                remaining > 1f
                ? remaining.ToString("F1")
                : remaining.ToString("F2");
        }
        else
        {
            cooldownText.text = "";
        }

        bool cooldownFinished =
            remaining <= 0;

        if (!initialized)
        {
            cooldownFinishedLastFrame =
                cooldownFinished;

            initialized = true;
        }

        if (
            cooldownFinished &&
            !cooldownFinishedLastFrame
        )
        {
            if (popRoutine != null)
                StopCoroutine(popRoutine);

            popRoutine =
                StartCoroutine(PopAnimation());
        }

        cooldownFinishedLastFrame =
            cooldownFinished;
    }
    public void SetPlayer(PlayerMovement movement)
    {
        player = movement;
    }

    IEnumerator PopAnimation()
    {
        float duration = 0.15f;
        float timer = 0f;

        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * 1.2f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            dashIcon.localScale =
                Vector3.Lerp(
                    startScale,
                    targetScale,
                    timer / duration);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            dashIcon.localScale =
                Vector3.Lerp(
                    targetScale,
                    startScale,
                    timer / duration);

            yield return null;
        }

        dashIcon.localScale = Vector3.one;
    }

}