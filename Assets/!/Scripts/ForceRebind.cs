using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ForceRebind : MonoBehaviour
{
    public Transform root;

    void Start()
    {
        var renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        var boneMap = root
            .GetComponentsInChildren<Transform>()
            .ToDictionary(b => b.name, b => b);

        foreach (var smr in renderers)
        {
            var newBones = new Transform[smr.bones.Length];

            for (int i = 0; i < smr.bones.Length; i++)
            {
                var boneName = smr.bones[i].name;

                if (boneMap.TryGetValue(boneName, out var bone))
                    newBones[i] = bone;
                else
                    Debug.LogError($"Missing bone: {boneName}");
            }

            smr.bones = newBones;
            smr.rootBone = root;
        }

        Debug.Log("Rebind complete");
    }
}