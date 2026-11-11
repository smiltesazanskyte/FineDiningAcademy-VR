
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TablewareOutlineGuide : MonoBehaviour
{
    public enum OutlineShape
    {
        Circle,
        Rectangle
    }

    [Header("References")]
    public Transform snapPoint;
    public TablewareSnap tablewareItem;

    [Header("Outline Shape")]
    public OutlineShape shape = OutlineShape.Circle;

    public float radius = 0.15f;
    public float width = 0.03f;
    public float length = 0.20f;
    public float rotationOffset = 0f;

    [Header("Dot Appearance")]
    public int dotCount = 48;
    public float dotSize = 0.005f;
    public float heightOffset = 0.002f;

    private MeshFilter meshFilter;
    private Mesh generatedMesh;

    private void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void LateUpdate()
    {
        if (snapPoint == null)
            return;

        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        // Hide the outline once the item is correctly placed.
        if (Application.isPlaying && tablewareItem != null)
        {
            meshRenderer.enabled = !tablewareItem.IsSnapped;

            if (tablewareItem.IsSnapped)
                return;
        }
        else
        {
            meshRenderer.enabled = true;
        }

        int count = Mathf.Max(4, dotCount);

        Vector3[] vertices = new Vector3[count * 4];
        int[] triangles = new int[count * 6];

        for (int i = 0; i < count; i++)
        {
            float x;
            float z;

            if (shape == OutlineShape.Circle)
            {
                float angle = i * Mathf.PI * 2f / count;

                x = Mathf.Cos(angle) * radius;
                z = Mathf.Sin(angle) * radius;
            }
            else
            {
                // Distribute dots evenly around a rectangle.
                float w = Mathf.Max(0.001f, width);
                float l = Mathf.Max(0.001f, length);
                float perimeter = 2f * (w + l);
                float distance = i * perimeter / count;

                if (distance < w)
                {
                    x = -w / 2f + distance;
                    z = -l / 2f;
                }
                else if (distance < w + l)
                {
                    x = w / 2f;
                    z = -l / 2f + (distance - w);
                }
                else if (distance < 2f * w + l)
                {
                    x = w / 2f - (distance - w - l);
                    z = l / 2f;
                }
                else
                {
                    x = -w / 2f;
                    z = l / 2f - (distance - 2f * w - l);
                }
            }

            // Rotate the dotted outline around its center.
            Quaternion rotation = Quaternion.Euler(0f, rotationOffset, 0f);

            Vector3 rotatedOffset = rotation * new Vector3(x, 0f, z);

            // Position the outline at the snap point.
            Vector3 worldPosition =
                snapPoint.position +
                rotatedOffset +
                Vector3.up * heightOffset;

            Vector3 center =
                transform.InverseTransformPoint(worldPosition);
            
            float half = dotSize * 0.5f;

            int v = i * 4;
            int t = i * 6;

            vertices[v] = center + new Vector3(-half, 0, -half);
            vertices[v + 1] = center + new Vector3(-half, 0, half);
            vertices[v + 2] = center + new Vector3(half, 0, half);
            vertices[v + 3] = center + new Vector3(half, 0, -half);

            triangles[t] = v;
            triangles[t + 1] = v + 1;
            triangles[t + 2] = v + 2;

            triangles[t + 3] = v;
            triangles[t + 4] = v + 2;
            triangles[t + 5] = v + 3;
        }

        Mesh newMesh = new Mesh();
        newMesh.name = "Dotted Guide Mesh";
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.RecalculateNormals();

        meshFilter.sharedMesh = newMesh;

        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);
        }

        generatedMesh = newMesh;
    }

    private void OnDisable()
    {
        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);

            generatedMesh = null;
        }
    }
}