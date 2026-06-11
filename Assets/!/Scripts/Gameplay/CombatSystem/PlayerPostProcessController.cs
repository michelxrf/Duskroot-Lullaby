using UnityEngine;

public class PlayerPostProcessController : MonoBehaviour
{
    [SerializeField] private GameObject filtroGeral;
    [SerializeField] private GameObject filtroMorto;

    private void Start()
    {
        SetDead(false);
    }

    public void SetDead(bool isDead)
    {
        if (filtroGeral != null)
            filtroGeral.SetActive(!isDead);

        if (filtroMorto != null)
            filtroMorto.SetActive(isDead);
    }
}