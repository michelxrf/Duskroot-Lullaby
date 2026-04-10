using System.Collections;
using UnityEngine;

/// <summary>
/// Used for props to prevent them from blocking player's view
/// </summary>
public class Fader : MonoBehaviour
{
    private Renderer[] renderers;
    private Material[] runtimeMaterials;

    [SerializeField] private float fadeAlpha = 0.3f;
    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine fadeRoutine;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        runtimeMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            // Clone material ONCE
            Material mat = new Material(renderers[i].material);
            SetupMaterialAsTransparent(mat);

            renderers[i].material = mat;
            runtimeMaterials[i] = mat;
        }
    }

    public void Fade()
    {
        StartFade(fadeAlpha);
    }

    public void UnFade()
    {
        StartFade(1f);
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float time = 0f;
        float startAlpha = runtimeMaterials.Length > 0 ? runtimeMaterials[0].color.a : 1f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);

            ApplyAlpha(alpha);

            yield return null;
        }

        ApplyAlpha(targetAlpha);
    }

    private void ApplyAlpha(float alpha)
    {
        foreach (var mat in runtimeMaterials)
        {
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        }
    }

    /// <summary>
    /// Proper URP transparent setup (this is the important part)
    /// </summary>
    private void SetupMaterialAsTransparent(Material mat)
    {
        // URP properties
        mat.SetFloat("_Surface", 1); // 1 = Transparent
        mat.SetFloat("_Blend", 0);   // Alpha blend

        // Blending
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        // Depth writing OFF for transparency
        mat.SetInt("_ZWrite", 0);

        // Keywords (VERY important in URP)
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        // Render queue
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}