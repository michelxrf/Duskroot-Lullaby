using UnityEngine;

public class FadaOffset : MonoBehaviour
{
    [SerializeField] float offset = -45;

    private void Awake()
    {
       
        Vector3 currentEulerAngles = transform.localEulerAngles;
        currentEulerAngles.x = offset;
        transform.localEulerAngles = currentEulerAngles;

    }


}
