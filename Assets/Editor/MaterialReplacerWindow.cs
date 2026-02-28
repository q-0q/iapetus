using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MaterialReplacerWindow : EditorWindow
{
    private Material materialToFind;
    private Material materialToReplace;
    private bool includeInactive = true;

    [MenuItem("Tools/Material Replacer")]
    public static void ShowWindow()
    {
        GetWindow<MaterialReplacerWindow>("Material Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Material In Current Scene", EditorStyles.boldLabel);

        materialToFind = (Material)EditorGUILayout.ObjectField(
            "Material To Find",
            materialToFind,
            typeof(Material),
            false);

        materialToReplace = (Material)EditorGUILayout.ObjectField(
            "Replace With",
            materialToReplace,
            typeof(Material),
            false);

        includeInactive = EditorGUILayout.Toggle("Include Inactive Objects", includeInactive);

        EditorGUILayout.Space();

        GUI.enabled = materialToFind != null && materialToReplace != null;

        if (GUILayout.Button("Replace Materials"))
        {
            ReplaceMaterials();
        }

        GUI.enabled = true;
    }

    private void ReplaceMaterials()
    {
        int replacedCount = 0;

        Renderer[] renderers = includeInactive
            ? Resources.FindObjectsOfTypeAll<Renderer>()
            : FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            // Skip assets/prefabs not in scene
            if (!renderer.gameObject.scene.IsValid() || 
                renderer.gameObject.scene != SceneManager.GetActiveScene())
                continue;

            Material[] materials = renderer.sharedMaterials;
            bool changed = false;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == materialToFind)
                {
                    materials[i] = materialToReplace;
                    changed = true;
                    replacedCount++;
                }
            }

            if (changed)
            {
                Undo.RecordObject(renderer, "Replace Materials");
                renderer.sharedMaterials = materials;
                EditorUtility.SetDirty(renderer);
            }
        }

        Debug.Log($"Material replacement complete. Replaced {replacedCount} material slot(s).");
    }
}