using CombatSystem;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls revive interaction UI visibility and progress fill for a tombstone.
/// </summary>
public class ReviveInteractUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ReviveTombstone reviveTombstone;
    [SerializeField] CanvasGroup rootCanvasGroup;
    [SerializeField] Image sliderBackground;

    NetworkRunner runner;

    void Awake()
    {
        if (reviveTombstone == null)
            reviveTombstone = GetComponentInParent<ReviveTombstone>();

        if (rootCanvasGroup == null)
            rootCanvasGroup = GetComponent<CanvasGroup>();

        if (sliderBackground == null)
        {
            Transform slider = transform.Find("SliderBackground");
            if (slider != null)
                sliderBackground = slider.GetComponent<Image>();
        }

        runner = FindFirstObjectByType<NetworkRunner>();

        SetVisible(false);
        SetProgress(0f);
    }

    void Update()
    {
        if (reviveTombstone == null || sliderBackground == null)
            return;

        if (reviveTombstone.IsCompleted)
        {
            SetVisible(false);
            return;
        }

        bool inRangeAndValid = IsLocalReviverInsideRange();
        SetVisible(inRangeAndValid);
        SetProgress(GetNormalizedProgress());
    }

    bool IsLocalReviverInsideRange()
    {
        if (runner == null || runner.LocalPlayer == PlayerRef.None)
            return false;

        NetworkObject localPlayerObject = runner.GetPlayerObject(runner.LocalPlayer);
        if (localPlayerObject == null)
            return false;

        PlayerHealth localHealth = localPlayerObject.GetComponent<PlayerHealth>();
        if (localHealth != null && localHealth.IsDead())
            return false;

        float maxDistance = reviveTombstone.InteractionRadius + 0.3f;
        float sqrDistance = (localPlayerObject.transform.position - reviveTombstone.transform.position).sqrMagnitude;
        return sqrDistance <= maxDistance * maxDistance;
    }

    float GetNormalizedProgress()
    {
        if (reviveTombstone.RequiredButtonPresses <= 0)
            return 0f;

        return Mathf.Clamp01(reviveTombstone.ReviveProgress / (float)reviveTombstone.RequiredButtonPresses);
    }

    void SetVisible(bool value)
    {
        if (rootCanvasGroup == null)
        {
            gameObject.SetActive(value);
            return;
        }

        rootCanvasGroup.alpha = value ? 1f : 0f;
        rootCanvasGroup.interactable = false;
        rootCanvasGroup.blocksRaycasts = false;
    }

    void SetProgress(float value)
    {
        sliderBackground.fillAmount = value;
    }
}
