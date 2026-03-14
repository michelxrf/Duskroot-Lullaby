using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FootstepManager : MonoBehaviour
{
    [Header("FMOD")]
    public EventReference footstepEvent;

    [Header("Raycast")]
    public Transform rayOrigin;
    public float rayDistance = 1.5f;
    public LayerMask groundLayer;

    Terrain terrain;
    TerrainData terrainData;
    float[,,] splatmapData;

    int alphamapWidth;
    int alphamapHeight;
    int numTextures;

    enum SurfaceType
    {
        Terra = 0,
        Grama = 1,
        Agua = 2,
        Madeira = 3
    }

    void Start()
    {
        terrain = Terrain.activeTerrain;

        if (terrain != null)
        {
            terrainData = terrain.terrainData;

            alphamapWidth = terrainData.alphamapWidth;
            alphamapHeight = terrainData.alphamapHeight;

            splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

            numTextures = splatmapData.GetLength(2);
        }
    }

    public void PlayFootstep()
    {
        SurfaceType surface = DetectSurface();

        EventInstance footstep = RuntimeManager.CreateInstance(footstepEvent);

        footstep.setParameterByName("Surface", (float)surface);

        footstep.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        footstep.start();
        footstep.release();
    }

    SurfaceType DetectSurface()
    {
        if (Physics.Raycast(rayOrigin.position, Vector3.down, out RaycastHit hit, rayDistance, groundLayer))
        {
            //Debug.Log("Entrou na phy: Origin:" + rayOrigin.position + "  groundlayer:" + groundLayer);
            Terrain hitTerrain = hit.collider.GetComponent<Terrain>();

            if (hitTerrain != null && terrain != null)
            {
                int textureIndex = GetTerrainTextureIndex(hit.point);
                //Debug.Log("Terrain=" + GetTerrainTextureIndex(hit.point));
                switch (textureIndex)
                {
                    case 0: return SurfaceType.Terra;
                    case 1: return SurfaceType.Grama;
                    case 2: return SurfaceType.Agua;
                    default: return SurfaceType.Grama;
                }
            }
            else
            {
                //Debug.Log("props=" + hit.collider.tag);
                switch (hit.collider.tag)
                {
                    case "Wood":
                        return SurfaceType.Madeira;

                    case "Water":
                        return SurfaceType.Agua;

                    case "Ground":
                        return SurfaceType.Terra;

                    default:
                        return SurfaceType.Grama;
                }
            }
        }

        return SurfaceType.Grama;
    }

    int GetTerrainTextureIndex(Vector3 worldPos)
    {
        Vector3 terrainPos = terrain.transform.position;

        int mapX = Mathf.Clamp(
            (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * alphamapWidth),
            0,
            alphamapWidth - 1
        );

        int mapZ = Mathf.Clamp(
            (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * alphamapHeight),
            0,
            alphamapHeight - 1
        );

        float maxMix = 0;
        int maxIndex = 0;

        for (int i = 0; i < numTextures; i++)
        {
            float mix = splatmapData[mapZ, mapX, i];

            if (mix > maxMix)
            {
                maxMix = mix;
                maxIndex = i;
            }
        }

        return maxIndex;
    }
}