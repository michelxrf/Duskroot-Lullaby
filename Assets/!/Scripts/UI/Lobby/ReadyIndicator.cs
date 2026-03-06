using UnityEngine;
using UnityEngine.UI;

public class ReadyIndicator : MonoBehaviour
{
    [Header("References")]
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
