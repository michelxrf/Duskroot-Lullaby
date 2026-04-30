using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Coloque no objeto do jogador (não na câmera).
/// Faz um SphereCast da câmera até o jogador e aplica o efeito de cutout
/// nos materiais das paredes atingidas. Compatível com multiplayer.
/// Os materiais precisam ter as propriedades: _CutoutPos, _CutoutSize, _FalloffSize.
/// </summary>
public class CutoutObject : MonoBehaviour
{
    [Header("Cutout")]
    [SerializeField] private float cutoutSize = 0.1f;
    [SerializeField] private float falloffSize = 0.05f;
    [SerializeField] private float sphereCastRadius = 0.5f;

    [Header("Layer")]
    [SerializeField] private LayerMask wallMask;

    /// <summary>Câmera do jogador. Se vazia, usa a Camera.main.</summary>
    [SerializeField] private Camera playerCamera;

    // Materiais com efeito ativo no frame atual
    private readonly HashSet<Material> _activeMaterials   = new();
    // Materiais com efeito ativo no frame anterior
    private readonly HashSet<Material> _previousMaterials = new();

    // Debug para ativar o Gizmos
    public bool GizmosDebug = false;

    private void Awake()
    {
        // Usa Camera.main como fallback; em multiplayer atribua via Inspector ou SetCamera()
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Camera não foi inicializada corretamente no script: CutoutObject");
            return;
        }

        _activeMaterials.Clear();

        Vector3 camPos    = playerCamera.transform.position;
        Vector3 playerPos = transform.position;
        Vector3 direction = playerPos - camPos;
        float   distance  = direction.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(
            camPos,
            sphereCastRadius,
            direction.normalized,
            distance,
            wallMask
        );

        // WorldToViewportPoint já normaliza por resolução, não precisamos dividir pela aspect ratio
        Vector2 cutoutPos = playerCamera.WorldToViewportPoint(playerPos);

        // Aplica o efeito nos materiais atingidos
        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.transform.GetComponent<Renderer>();
            if (rend == null) continue;

            foreach (Material mat in rend.materials)
            {
                mat.SetVector("_CutoutPos",  cutoutPos);
                mat.SetFloat("_CutoutSize",  cutoutSize);
                mat.SetFloat("_FalloffSize", falloffSize);
                _activeMaterials.Add(mat);
            }
        }

        // Remove o efeito dos materiais que saíram do cast neste frame
        foreach (Material mat in _previousMaterials)
        {
            if (!_activeMaterials.Contains(mat))
                ResetMaterial(mat);
        }

        // Atualiza histórico para o próximo frame
        _previousMaterials.Clear();
        foreach (Material mat in _activeMaterials)
            _previousMaterials.Add(mat);
    }

#if UNITY_EDITOR
    // Visível no editor mesmo sem o jogo rodando.
    // Usa a câmera atribuída no Inspector; se vazia, usa a Scene View camera.
    private void OnDrawGizmos()
    {
        if(!GizmosDebug) return;

        Camera cam = playerCamera != null
            ? playerCamera
            : SceneView.lastActiveSceneView?.camera;

        if (cam == null) return;

        Vector3 origin = cam.transform.position;
        Vector3 end    = transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, end);
        Gizmos.DrawWireSphere(origin, sphereCastRadius);
        Gizmos.DrawWireSphere(end,    sphereCastRadius);
    }
#endif

    /// <summary>Zera o cutout do material sem alterar outros parâmetros.</summary>
    private void ResetMaterial(Material mat)
    {
        mat.SetFloat("_CutoutSize", 0f);
    }

    /// <summary>
    /// Injeta a câmera do jogador via código.
    /// Use no OnStartLocalPlayer do seu NetworkBehaviour.
    /// </summary>
    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }

    private void OnDisable()
    {
        // Remove o efeito de todos os materiais ao desativar o objeto
        foreach (Material mat in _previousMaterials)
            ResetMaterial(mat);

        _previousMaterials.Clear();
        _activeMaterials.Clear();
    }
}