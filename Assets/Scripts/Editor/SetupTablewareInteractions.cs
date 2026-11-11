
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public static class SetupTablewareInteractions
{
    [MenuItem("Tools/Setup All Tableware Interactions")]
    public static void Setup()
    {
        GameObject tableware = GameObject.Find("TablewareObjects");
        GameObject snapPoints = GameObject.Find("TablewareSnapPoints");

        if (tableware == null || snapPoints == null)
        {
            Debug.LogError(
                "TablewareObjects or TablewareSnapPoints not found!"
            );
            return;
        }

        int count = 0;

        foreach (Transform stage in tableware.transform)
        {
            foreach (Transform item in stage)
            {
                GameObject obj = item.gameObject;

                Undo.RegisterFullObjectHierarchyUndo(
                    obj, "Setup Tableware Interaction"
                );

                // Add collider
                Collider collider = obj.GetComponent<Collider>();

                if (collider == null)
                {
                    MeshFilter mesh = obj.GetComponent<MeshFilter>();

                    if (mesh != null)
                    {
                        MeshCollider mc =
                            Undo.AddComponent<MeshCollider>(obj);

                        mc.sharedMesh = mesh.sharedMesh;
                        mc.convex = true;
                    }
                    else
                    {
                        Undo.AddComponent<BoxCollider>(obj);
                    }
                }

                // Configure Rigidbody
                Rigidbody rb = obj.GetComponent<Rigidbody>();

                if (rb == null)
                    rb = Undo.AddComponent<Rigidbody>(obj);

                rb.useGravity = false;
                rb.isKinematic = false;

                rb.collisionDetectionMode =
                    CollisionDetectionMode.ContinuousDynamic;

                rb.interpolation =
                    RigidbodyInterpolation.Interpolate;

                rb.constraints =
                    RigidbodyConstraints.FreezeRotation;

                // Configure VR grabbing
                XRGrabInteractable grab =
                    obj.GetComponent<XRGrabInteractable>();

                if (grab == null)
                    grab = Undo.AddComponent<XRGrabInteractable>(obj);

                grab.movementType =
                    XRBaseInteractable.MovementType.VelocityTracking;

                grab.throwOnDetach = false;
                grab.trackRotation = false;

                // Connect saved snap point
                Transform snap = snapPoints.transform.Find(
                    obj.name + "_SnapPoint"
                );

                if (snap != null)
                {
                    TablewareSnap snapScript =
                        obj.GetComponent<TablewareSnap>();

                    if (snapScript == null)
                        snapScript =
                            Undo.AddComponent<TablewareSnap>(obj);

                    SerializedObject serialized =
                        new SerializedObject(snapScript);

                    serialized.FindProperty("snapPoint")
                        .objectReferenceValue = snap;

                    serialized.FindProperty("snapDistance")
                        .floatValue = 0.25f;

                    serialized.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogWarning(
                        "No snap point found for " + obj.name
                    );
                }

                EditorUtility.SetDirty(obj);
                count++;
            }
        }

        Debug.Log(
            "Configured " + count + " tableware objects!"
        );
    }
}