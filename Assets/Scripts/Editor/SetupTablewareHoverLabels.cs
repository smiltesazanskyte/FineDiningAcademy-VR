
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class SetupTablewareHoverLabels
{
    [MenuItem("Tools/Setup All Tableware Hover Labels")]
    public static void Setup()
    {
        GameObject tableware = GameObject.Find("TablewareObjects");

        if (tableware == null)
        {
            Debug.LogError("TablewareObjects not found!");
            return;
        }

        // Proper display names for all 14 objects.
        Dictionary<string, string> names =
            new Dictionary<string, string>()
        {
            { "ChargerPlate", "Charger Plate" },
            { "EntreeFork", "Entrée Fork" },
            { "EntreeKnife", "Entrée Knife" },

            { "SoupSpoon", "Soup Spoon" },
            { "SaladFork", "Salad Fork" },
            { "SaladKnife", "Salad Knife" },

            { "BreadPlate", "Bread Plate" },
            { "ButterKnife", "Butter Knife" },
            { "DessertSpoon", "Dessert Spoon" },
            { "DessertFork", "Dessert Fork" },

            { "RedWineGlass", "Red Wine Glass" },
            { "WhiteWineGlass", "White Wine Glass" },
            { "WaterGlass", "Water Glass" },
            { "LinenNapkin", "Linen Napkin" }
        };

        int count = 0;

        foreach (Transform stage in tableware.transform)
        {
            foreach (Transform item in stage)
            {
                GameObject obj = item.gameObject;

                TablewareHoverLabel label =
                    obj.GetComponent<TablewareHoverLabel>();

                if (label == null)
                {
                    label = Undo.AddComponent<TablewareHoverLabel>(obj);
                }

                Undo.RecordObject(label, "Configure Hover Label");

                // Set the proper name.
                if (names.TryGetValue(obj.name, out string name))
                {
                    label.displayName = name;
                }
                else
                {
                    label.displayName = obj.name;
                }

                // Position the label above the object.
                label.labelOffset = new Vector3(0, 0.25f, 0);

                EditorUtility.SetDirty(label);
                count++;
            }
        }

        Debug.Log(
            "Configured hover labels for " + count +
            " tableware objects!"
        );
    }
}