using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a visual indicator showing whether a lobby seat's player is ready.
/// Updates dynamically based on the ready state of the associated lobby seat.
/// </summary>
public class ReadyIndicator : MonoBehaviour
{
    [Header("References")]
    /// <summary>The image component to display/hide the ready indicator</summary>
    [SerializeField] private Image readyIndicatorObject;

    private LobbySeat lobbySeat;

    private void Awake()
    {
        lobbySeat = GetComponent<LobbySeat>();     
        readyIndicatorObject.enabled = false;
    }

    private void Start()
    {
        lobbySeat.OnReadyStateChanged += UpdateIndicator;
    }

    /// <summary>
    /// Updates the visual indicator based on the current ready state of the lobby seat.
    /// </summary>
    private void UpdateIndicator()
    {
        if (lobbySeat == null || readyIndicatorObject == null)
            return;

        readyIndicatorObject.enabled = !lobbySeat.IsEmpty && lobbySeat.IsReady;
    }

    private void OnDestroy()
    {
        if (lobbySeat != null)
        {
            lobbySeat.OnReadyStateChanged -= UpdateIndicator;
        }
    }
}
