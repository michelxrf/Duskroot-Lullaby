using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Fusion;

public class ForceRebind : NetworkBehaviour
{
    public Transform root;

    public override void Spawned()
    {
        Rebind();
    }

    public void Rebind()
    {
        if (root == null)
        {
            Debug.LogError("ForceRebind: Root transform is not assigned!");
            return;
        }

        // Include inactive renderers so we rebind everything at once
        var renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);

        // Build the bone map manually to avoid crashes on duplicate names (like "cajado")
        var boneMap = new Dictionary<string, Transform>();
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (!boneMap.ContainsKey(t.name))
            {
                boneMap.Add(t.name, t);
            }
        }

        foreach (var smr in renderers)
        {
            var newBones = new Transform[smr.bones.Length];

            for (int i = 0; i < smr.bones.Length; i++)
            {
                if (smr.bones[i] == null) continue;
                
                var boneName = smr.bones[i].name;

                if (boneMap.TryGetValue(boneName, out var bone))
                    newBones[i] = bone;
                else
                    Debug.LogWarning($"Missing bone: {boneName} for renderer {smr.name}");
            }

            smr.bones = newBones;
            smr.rootBone = root;
        }

        Debug.Log("Rebind complete");
    }
}