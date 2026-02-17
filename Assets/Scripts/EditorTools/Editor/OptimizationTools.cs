using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public static class OptimizationTools
{
    [MenuItem("Optimization/Fix TMP Text With No Font Asset")]
    public static void FixTMPMissingFont()
    {
        TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
        if (defaultFont == null)
        {
            Debug.LogWarning("[Optimization] TMP Settings has no default font. Assign one in Edit → Project Settings → TextMesh Pro.");
            return;
        }
        int fixedCount = 0;
        foreach (var tmp in Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (tmp.font == null)
            {
                tmp.font = defaultFont;
                fixedCount++;
                EditorUtility.SetDirty(tmp);
            }
        }
        if (fixedCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log($"[Optimization] Assigned default font to {fixedCount} TMP text component(s). Save the scene.");
        }
        else
            Debug.Log("[Optimization] No TMP text with missing font found in the scene.");
    }

    [MenuItem("Optimization/Enable GPU Instancing on All Materials")]
    public static void EnableGpuInstancingOnMaterials()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int enabled = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null && mat.shader != null && mat.enableInstancing == false)
            {
                mat.enableInstancing = true;
                enabled++;
                EditorUtility.SetDirty(mat);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"[Optimization] GPU Instancing enabled on {enabled} materials (total scanned: {guids.Length}).");
    }

    [MenuItem("Optimization/Group Hierarchy - Hazards and Environment")]
    public static void GroupHierarchy()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            Debug.LogWarning("[Optimization] No active scene loaded. Open the scene first.");
            return;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        if (roots == null || roots.Length == 0) return;

        string[] hazardNames = new[] { "Fire", "Arrow Shooter", "Arrow", "Hazard" };
        string[] envNames = new[] { "Castle", "floor", "Door", "Wall", "Environment" };

        Transform hazardsRoot = CreateOrGetRoot(scene, roots, "--- HAZARDS ---");
        Transform envRoot = CreateOrGetRoot(scene, roots, "--- ENVIRONMENT ---");

        int movedHazards = 0;
        int movedEnv = 0;

        List<Transform> toMove = new List<Transform>();
        foreach (GameObject go in roots)
        {
            Transform t = go.transform;
            if (t == hazardsRoot || t == envRoot) continue;
            string name = t.name;
            foreach (string key in hazardNames)
            {
                if (name.Contains(key))
                {
                    toMove.Add(t);
                    break;
                }
            }
        }
        foreach (Transform t in toMove)
        {
            Undo.SetTransformParent(t, hazardsRoot, "Group Hazards");
            movedHazards++;
        }

        roots = scene.GetRootGameObjects();
        toMove.Clear();
        foreach (GameObject go in roots)
        {
            Transform t = go.transform;
            if (t == hazardsRoot || t == envRoot) continue;
            string name = t.name;
            foreach (string key in envNames)
            {
                if (name.Contains(key))
                {
                    toMove.Add(t);
                    break;
                }
            }
        }
        foreach (Transform t in toMove)
        {
            Undo.SetTransformParent(t, envRoot, "Group Environment");
            movedEnv++;
        }

        Debug.Log($"[Optimization] Grouped hierarchy: {movedHazards} under --- HAZARDS ---, {movedEnv} under --- ENVIRONMENT ---. Save the scene to keep changes.");
    }

    private static Transform CreateOrGetRoot(Scene scene, GameObject[] roots, string name)
    {
        foreach (GameObject go in roots)
            if (go.name == name) return go.transform;
        GameObject newGo = new GameObject(name);
        SceneManager.MoveGameObjectToScene(newGo, scene);
        return newGo.transform;
    }
}
