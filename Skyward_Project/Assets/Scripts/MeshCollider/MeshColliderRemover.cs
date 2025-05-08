using UnityEngine;
using UnityEditor;

public class MeshColliderRemoverEditor
{
    [MenuItem("Tools/Remove Mesh Colliders From Selected Object")]
    static void RemoveMeshColliders()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Hiçbir obje seçilmedi!");
            return;
        }

        GameObject selected = Selection.activeGameObject;
        MeshCollider[] colliders = selected.GetComponentsInChildren<MeshCollider>();

        int count = 0;
        foreach (MeshCollider col in colliders)
        {
            Undo.RecordObject(col.gameObject, "Remove MeshCollider");
            Object.DestroyImmediate(col);
            count++;
        }

        Debug.Log($"{selected.name} içindeki {count} MeshCollider silindi. (Edit > Undo ile geri alınabilir)");
    }
}
