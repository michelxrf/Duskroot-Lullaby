using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UiKeys : MonoBehaviour
{
    [SerializeField] GameObject keyPrefab;
    [SerializeField] private GameObject visualRoot;

    public void FillKeys(List<string> keys)
    {
        ClearKeys();

        bool hasKeys =
            keys != null &&
            keys.Count > 0;

        visualRoot.SetActive(hasKeys);

        if (!hasKeys)
            return;

        foreach (string key in keys)
        {
            SetUpKey(key);
        }
    }

    void SetUpKey(string key)
    {
        GameObject newKey = Instantiate(keyPrefab, visualRoot.transform);
        newKey.GetComponentInChildren<TMPro.TMP_Text>().text = key;
    }

    void ClearKeys()
    {
        foreach (Transform child in visualRoot.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
