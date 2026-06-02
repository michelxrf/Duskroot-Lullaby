using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private void OnEnable()
    {
        StartCoroutine(SelectRoutine());
    }

    public void SelectFirstButton()
    {
        StartCoroutine(SelectRoutine());
    }

    public void CloseAndBackToOtherBtt(GameObject returnButton)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(returnButton);
    }
    private IEnumerator SelectRoutine()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}