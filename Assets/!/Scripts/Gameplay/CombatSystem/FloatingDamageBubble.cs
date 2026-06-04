using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The floating damage bubble is a visual effect that appears when a character takes damage, displaying the amount of damage dealt. It floats upwards and fades out over time before being destroyed. This class manages the behavior of the damage bubble, including its movement, fading, and lifespan.
/// </summary>
public class FloatingDamageBubble : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float floatSpeed = .5f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float lifetime = 1f;


    [Header("References")]
    [SerializeField] private TMP_Text damageText;

    private float elapsedTime = 0f;
    private bool isFading = false;

    public void Init(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
        damageText.color = damageAmount > 0 ? Color.red : Color.green;
        elapsedTime = 0f;
        isFading = false;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < lifetime)
        {
            transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
        }
        else if (!isFading && elapsedTime >= lifetime)
        {
            isFading = true;
            elapsedTime = 0f;
        }

        if (isFading)
        {
            float fadeProgress = elapsedTime / fadeDuration;
            Color newColor = damageText.color;
            newColor.a = Mathf.Lerp(1f, 0f, fadeProgress);
            damageText.color = newColor;

            if (elapsedTime >= fadeDuration)
            {
                Despawn();
            }
        }
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

}
