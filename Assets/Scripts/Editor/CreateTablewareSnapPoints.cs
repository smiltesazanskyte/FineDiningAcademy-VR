
using UnityEngine;
using UnityEditor;

public static class CreateTablewareSnapPoints
{
    [MenuItem("Tools/Create Tableware Snap Points")]
    public static void CreateSnapPoints()
    {
        GameObject tableware = GameObject.Find("TablewareObjects");

        if (tableware == null)
        {
            Debug.LogError("TablewareObjects not found!");
            return;
        }

        GameObject snapGroup = new GameObject("TablewareSnapPoints");
        Undo.RegisterCreatedObjectUndo(snapGroup, "Create Snap Points");

        foreach (Transform item in tableware.transform)
        {
            GameObject snap = new GameObject(item.name + "_SnapPoint");

            Undo.RegisterCreatedObjectUndo(snap, "Create Snap Point");

            snap.transform.SetParent(snapGroup.transform);
            snap.transform.SetPositionAndRotation(
                item.position,
                item.rotation
            );
        }

        Debug.Log("Tableware snap points created!");
    }
}
