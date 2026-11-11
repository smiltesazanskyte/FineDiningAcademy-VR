using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class SetupTablewareGuides
{
    [MenuItem("Tools/Setup All Tableware Guides")]
    public static void Setup()
    {
        GameObject snapRoot =
            GameObject.Find("TablewareSnapPoints");

        GameObject tablewareRoot =
            GameObject.Find("TablewareObjects");

        if (snapRoot == null || tablewareRoot == null)
        {
            Debug.LogError(
                "TablewareSnapPoints or TablewareObjects not found!"
            );
            return;
        }

        GameObject guideRoot =
            GameObject.Find("TableSettingGuides");

        if (guideRoot == null)
        {
            guideRoot = new GameObject("TableSettingGuides");
            Undo.RegisterCreatedObjectUndo(
                guideRoot, "Create Table Setting Guides"
            );
        }

        Material guideMaterial = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/Materials/TableSettingGuide.mat"
        );

        if (guideMaterial == null)
        {
            Debug.LogError(
                "TableSettingGuide material not found!"
            );
            return;
        }

        // Shape and dimensions for each item.
        // Circle: radius
        // Rectangle: width and length
        Dictionary<string, Vector3> dimensions =
            new Dictionary<string, Vector3>()
        {
            { "ChargerPlate", new Vector3(0.15f, 0, 0) },
            { "EntreeFork", new Vector3(0, 0.025f, 0.20f) },
            { "EntreeKnife", new Vector3(0, 0.020f, 0.22f) },

            { "SoupSpoon", new Vector3(0, 0.035f, 0.20f) },
            { "SaladFork", new Vector3(0, 0.025f, 0.18f) },
            { "SaladKnife", new Vector3(0, 0.020f, 0.19f) },

            { "BreadPlate", new Vector3(0.09f, 0, 0) },
            { "ButterKnife", new Vector3(0, 0.018f, 0.13f) },
            { "DessertSpoon", new Vector3(0, 0.025f, 0.17f) },
            { "DessertFork", new Vector3(0, 0.025f, 0.17f) },

            { "RedWineGlass", new Vector3(0.035f, 0, 0) },
            { "WhiteWineGlass", new Vector3(0.035f, 0, 0) },
            { "WaterGlass", new Vector3(0.040f, 0, 0) },

            { "LinenNapkin", new Vector3(0, 0.16f, 0.16f) }
        };

        int count = 0;

        foreach (Transform stage in tablewareRoot.transform)
        {
            foreach (Transform item in stage)
            {
                string itemName = item.name;

                if (!dimensions.TryGetValue(
                    itemName, out Vector3 size))
                    continue;

                Transform snapPoint = snapRoot.transform.Find(
                    itemName + "_SnapPoint"
                );

                if (snapPoint == null)
                {
                    Debug.LogWarning(
                        "Missing snap point for " + itemName
                    );
                    continue;
                }

                string guideName = itemName + "_Guide";

                Transform existing =
                    guideRoot.transform.Find(guideName);

                GameObject guide;

                if (existing != null)
                {
                    guide = existing.gameObject;
                }
                else
                {
                    guide = new GameObject(guideName);
                    Undo.RegisterCreatedObjectUndo(
                        guide, "Create Tableware Guide"
                    );
                    guide.transform.SetParent(guideRoot.transform);
                }

                TablewareOutlineGuide outline =
                    guide.GetComponent<TablewareOutlineGuide>();

                if (outline == null)
                    outline = Undo.AddComponent<TablewareOutlineGuide>(
                        guide
                    );

                Undo.RecordObject(
                    outline, "Configure Tableware Guide"
                );

                outline.snapPoint = snapPoint;
                outline.tablewareItem =
                    item.GetComponent<TablewareSnap>();

                if (size.x > 0)
                {
                    outline.shape =
                        TablewareOutlineGuide.OutlineShape.Circle;

                    outline.radius = size.x;
                    outline.dotCount = 48;
                }
                else
                {
                    outline.shape =
                        TablewareOutlineGuide.OutlineShape.Rectangle;

                    outline.width = size.y;
                    outline.length = size.z;
                    outline.dotCount = 40;
                }

                outline.dotSize = 0.005f;
                outline.heightOffset = 0.002f;

                MeshRenderer renderer =
                    guide.GetComponent<MeshRenderer>();

                if (renderer != null)
                {
                    renderer.sharedMaterial = guideMaterial;
                    renderer.shadowCastingMode =
                        UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderer.receiveShadows = false;
                }

                EditorUtility.SetDirty(outline);
                count++;
            }
        }

        Debug.Log(
            "Successfully configured " + count +
            " tableware guides!"
        );
    }
}