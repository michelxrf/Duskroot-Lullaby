using UnityEngine;
using TMPro;

public class PlayersGateTooltip : MonoBehaviour
{
    [SerializeField] private PlayersInPlace playersInPlace;
    [SerializeField] private GameObject tooltipVisuals;
    [SerializeField] private TextMeshProUGUI requiredKeysText;

    private int requiredKeysCount = 0;

    private void Start()
    {
        if (tooltipVisuals != null)
        {
            tooltipVisuals.SetActive(false);
        }

        if (playersInPlace == null)
        {
            playersInPlace = GetComponent<PlayersInPlace>();
        }

        if (playersInPlace != null)
        {
            var keys = playersInPlace.RequiredKeys;
            if (keys != null && keys.Length > 0)
            {
                requiredKeysCount = keys.Length;
                if (requiredKeysText != null)
                {
                    // Format required keys
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.AppendLine("KEYS REQUIRED:");
                    foreach (var key in keys)
                    {
                        if (!string.IsNullOrEmpty(key))
                        {
                            sb.AppendLine($"- {key}");
                        }
                    }
                    requiredKeysText.text = sb.ToString().TrimEnd();
                }
            }
            else
            {
                requiredKeysCount = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (requiredKeysCount == 0 || tooltipVisuals == null)
            return;

        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        tooltipVisuals.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (tooltipVisuals == null)
            return;

        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        tooltipVisuals.SetActive(false);
    }
}
