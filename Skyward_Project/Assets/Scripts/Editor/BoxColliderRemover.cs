using UnityEngine;
using UnityEditor;

public class BoxColliderRemoverEditor
{
    [MenuItem("Tools/Remove Box Colliders From Selected Object")]
    static void RemoveBoxColliders()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Hiçbir obje seçilmedi!");
            return;
        }

        GameObject selected = Selection.activeGameObject;
        BoxCollider[] colliders = selected.GetComponentsInChildren<BoxCollider>();

        int count = 0;
        foreach (BoxCollider col in colliders)
        {
            Undo.RecordObject(col.gameObject, "Remove BoxCollider");
            Object.DestroyImmediate(col);
            count++;
        }

        Debug.Log($"{selected.name} içindeki {count} BoxCollider silindi. (Edit > Undo ile geri alınabilir)");
    }
}
