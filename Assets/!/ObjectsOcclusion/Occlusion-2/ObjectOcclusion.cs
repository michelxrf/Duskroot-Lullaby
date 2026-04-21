using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coloque na câmera do player.
/// Para adicionar suporte a um novo shader, crie uma classe dentro de RegisterStrategies()
/// e registre na lista. A ordem importa: primeiro match vence.
/// </summary>
public class ObjectOcclusion : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Detecção")]
    public LayerMask occlusionLayer;
    public Vector3   playerOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Transparência")]
    [Range(0f, 1f)]
    public float occludedAlpha = 0.25f;
    public float fadeSpeed     = 8f;

    // ── Estratégias ──────────────────────────────────────────────────────────

    private interface IShaderStrategy
    {
        bool Supports(Material mat);
        void MakeTransparent(Material mat);
        void SetAlpha(Material mat, float alpha);
    }

    // -- URP/Lit padrão -------------------------------------------------------
    private class URPLitStrategy : IShaderStrategy
    {
        static readonly int PropSurface  = Shader.PropertyToID("_Surface");
        static readonly int PropSrcBlend = Shader.PropertyToID("_SrcBlend");
        static readonly int PropDstBlend = Shader.PropertyToID("_DstBlend");
        static readonly int PropZWrite   = Shader.PropertyToID("_ZWrite");
        static readonly int PropColor    = Shader.PropertyToID("_BaseColor");

        public bool Supports(Material mat) =>
            mat.shader.name.StartsWith("Universal Render Pipeline/");

        public void MakeTransparent(Material mat)
        {
            if (mat.HasProperty(PropSurface))  mat.SetFloat(PropSurface,  1f);
            if (mat.HasProperty(PropSrcBlend)) mat.SetFloat(PropSrcBlend, (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            if (mat.HasProperty(PropDstBlend)) mat.SetFloat(PropDstBlend, (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            if (mat.HasProperty(PropZWrite))   mat.SetFloat(PropZWrite,   0f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        public void SetAlpha(Material mat, float alpha)
        {
            if (!mat.HasProperty(PropColor)) return;
            Color c = mat.GetColor(PropColor); c.a = alpha;
            mat.SetColor(PropColor, c);
        }
    }

    // -- Shaders com _OcclusionAlpha ------------------------------------------
    // Detecta por HasProperty OU por prefixo de nome do shader.
    // Para um novo shader: basta adicionar o prefixo do nome em _shaderPrefixes
    // OU simplesmente adicionar _OcclusionAlpha no shader que ele é detectado sozinho.
    private class OcclusionAlphaStrategy : IShaderStrategy
    {
        static readonly int PropOcclusionAlpha = Shader.PropertyToID("_OcclusionAlpha");

        // Adicione aqui prefixos de shaders que usam _OcclusionAlpha
        // mas que o HasProperty falha por serem gerados por ASE/ShaderGraph
        static readonly string[] _shaderPrefixes =
        {
            "Polytope Studio/",
            // "MeuOutroShader/",
        };

        public bool Supports(Material mat)
        {
            // Tenta HasProperty primeiro (caso ideal)
            if (mat.HasProperty(PropOcclusionAlpha)) return true;

            // Fallback: checa pelo nome do shader
            foreach (var prefix in _shaderPrefixes)
                if (mat.shader.name.StartsWith(prefix)) return true;

            return false;
        }

        public void MakeTransparent(Material mat)
        {
            // O shader já cuida do blend/queue internamente.
        }

        public void SetAlpha(Material mat, float alpha)
        {
            mat.SetFloat(PropOcclusionAlpha, alpha);
        }
    }

    // ── Registre novas estratégias aqui ──────────────────────────────────────

    private List<IShaderStrategy> _strategies;

    private void RegisterStrategies()
    {
        _strategies = new List<IShaderStrategy>
        {
            new OcclusionAlphaStrategy(),   // mais específico → primeiro
            new URPLitStrategy(),
            // new MinhaNovaStrategy(),
        };
    }

    // ── Tracking interno ─────────────────────────────────────────────────────

    private class OccluderData
    {
        public Material[]        originalMaterials;
        public Material[]        transparentMaterials;
        public IShaderStrategy[] strategies;
        public float             currentAlpha = 1f;
        public bool              isOccluding;
    }

    private readonly Dictionary<Renderer, OccluderData> _tracked = new();
    private readonly RaycastHit[] _hits = new RaycastHit[16];

    // ── Unity ────────────────────────────────────────────────────────────────

    void Awake() => RegisterStrategies();

    void Start()
    {
        StartCoroutine(GetPlayer());
    }

    IEnumerator GetPlayer()
    {
        while (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");

            if (obj != null)
            {
                player = obj.transform;
                yield break; // achou e encerra coroutine
            }

            yield return new WaitForSeconds(1f); // tenta de novo em 1 segundo
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        foreach (var data in _tracked.Values)
            data.isOccluding = false;

        Vector3 origin    = transform.position;
        Vector3 targetPos = player.position + playerOffset;
        Vector3 dir       = targetPos - origin;
        float   dist      = dir.magnitude;

        int count = Physics.RaycastNonAlloc(origin, dir.normalized, _hits, dist, occlusionLayer);

        for (int i = 0; i < count; i++)
        {
            Renderer rend = _hits[i].collider.GetComponentInParent<Renderer>();
            if (rend == null || rend.transform == player) continue;

            if (!_tracked.TryGetValue(rend, out OccluderData data))
            {
                data = CreateData(rend);
                if (data == null) continue;
                _tracked[rend] = data;
            }

            data.isOccluding = true;
        }

        var toRemove = new List<Renderer>();

        foreach (var kvp in _tracked)
        {
            Renderer     rend = kvp.Key;
            OccluderData data = kvp.Value;

            if (rend == null) { toRemove.Add(rend); continue; }

            float target = data.isOccluding ? occludedAlpha : 1f;
            data.currentAlpha = Mathf.MoveTowards(data.currentAlpha, target, fadeSpeed * Time.deltaTime);

            if (data.currentAlpha < 1f)
            {
                ApplyAlpha(data, data.currentAlpha);
                rend.materials = data.transparentMaterials;
            }
            else
            {
                rend.materials = data.originalMaterials;
                if (!data.isOccluding)
                {
                    DestroyTransparentMaterials(data);
                    toRemove.Add(rend);
                }
            }
        }

        foreach (var r in toRemove) _tracked.Remove(r);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    OccluderData CreateData(Renderer rend)
    {
        Material[] shared    = rend.sharedMaterials;
        var original         = new Material[shared.Length];
        var transparent      = new Material[shared.Length];
        var strategyPerSlot  = new IShaderStrategy[shared.Length];

        for (int i = 0; i < shared.Length; i++)
        {
            if (shared[i] == null) return null;

            IShaderStrategy strategy = FindStrategy(shared[i]);
            if (strategy == null)
            {
                Debug.LogWarning($"[CameraOcclusion] Shader '{shared[i].shader.name}' em '{rend.name}' não tem estratégia. Ignorando.", this);
                return null;
            }

            original[i]        = shared[i];
            strategyPerSlot[i] = strategy;

            Material copy = new Material(shared[i]);
            copy.name = shared[i].name + "_occluded";
            strategy.MakeTransparent(copy);
            transparent[i] = copy;
        }

        return new OccluderData
        {
            originalMaterials    = original,
            transparentMaterials = transparent,
            strategies           = strategyPerSlot,
            currentAlpha         = 1f,
        };
    }

    void ApplyAlpha(OccluderData data, float alpha)
    {
        for (int i = 0; i < data.transparentMaterials.Length; i++)
        {
            if (data.transparentMaterials[i] == null) continue;
            data.strategies[i].SetAlpha(data.transparentMaterials[i], alpha);
        }
    }

    IShaderStrategy FindStrategy(Material mat)
    {
        foreach (var s in _strategies)
            if (s.Supports(mat)) return s;
        return null;
    }

    void DestroyTransparentMaterials(OccluderData data)
    {
        foreach (var mat in data.transparentMaterials)
            if (mat != null) Destroy(mat);
    }

    void OnDestroy()
    {
        foreach (var kvp in _tracked)
        {
            if (kvp.Key != null) kvp.Key.materials = kvp.Value.originalMaterials;
            DestroyTransparentMaterials(kvp.Value);
        }
        _tracked.Clear();
    }
}