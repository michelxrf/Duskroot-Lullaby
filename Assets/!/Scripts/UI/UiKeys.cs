using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UiKeys : MonoBehaviour
{
    [SerializeField] GameObject keyPrefab;


    public void FillKeys(List<string> keys)
    {
        ClearKeys();

        foreach (string key in keys)
        {
            SetUpKey(key);
        }
    }

    void SetUpKey(string key)
    {
        GameObject newKey = Instantiate(keyPrefab, transform);
        newKey.GetComponentInChildren<TMPro.TMP_Text>().text = key;
    }

    void ClearKeys()
    {
        foreach (Transform child in GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
    }

}
