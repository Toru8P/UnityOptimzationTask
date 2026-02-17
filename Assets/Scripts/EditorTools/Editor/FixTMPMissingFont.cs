using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

public static class FixTMPMissingFont
{
    [MenuItem("Tools/Fix TMP Missing Font Asset")]
    public static void Run()
    {
        TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
        if (defaultFont == null)
        {
            Debug.LogWarning("TMP Settings has no default font. Assign in Edit → Project Settings → TextMesh Pro.");
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
            Debug.Log($"Assigned default font to {fixedCount} TMP text component(s). Save the scene.");
        }
        else
            Debug.Log("No TMP text with missing font found.");
    }
}
