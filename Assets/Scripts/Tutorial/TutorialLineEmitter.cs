using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TutorialLineEmitter : MonoBehaviour
{
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float dashLength = 0.3f;
    [SerializeField] private float gapLength = 0.2f;
    [SerializeField] private float scrollSpeed = 1f;

    [Header("Sorting")]
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int sortingOrder = 0;

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private readonly List<Vector3> vertices = new();
    private readonly List<int> triangles = new();
    private readonly List<Color> colors = new();

    private bool isVisible;
    private Vector2 currentOrigin;
    private Vector2 currentDirection;
    private float currentLength;
    private Color currentColor;

    private void Awake()
    {
        mesh = new Mesh { name = "TutorialDashedLineMesh" };
        GetComponent<MeshFilter>().mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = sortingOrder;

        meshRenderer.enabled = false;
    }

    // Call this to display the tutorial guide line
    public void ShowLine(Vector2 origin, Vector2 direction, float length, Color color)
    {
        currentOrigin = origin;
        currentDirection = direction.normalized;
        currentLength = length;
        currentColor = color;

        isVisible = true;
        meshRenderer.enabled = true;
    }

    // Call this to hide the tutorial guide line
    public void HideLine()
    {
        isVisible = false;
        meshRenderer.enabled = false;
    }

    private void Update()
    {
        if (isVisible)
        {
            // We rebuild the mesh every frame so the dashes animate smoothly using Time.unscaledTime
            BuildDashedMesh(currentOrigin, currentDirection, currentLength, currentColor);
        }
    }

    private void BuildDashedMesh(Vector2 origin, Vector2 direction, float length, Color color)
    {
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        if (length <= 0f || dashLength <= 0f)
        {
            mesh.Clear();
            return;
        }

        Vector2 normal = new Vector2(-direction.y, direction.x) * (lineWidth * 0.5f);
        float tile = Mathf.Max(dashLength + gapLength, 0.001f);

        // Using unscaledTime ensures the dashes move even when Time.timeScale = 0
        float phase = (Time.unscaledTime * scrollSpeed) % tile;
        float traveled = phase - tile;

        while (traveled < length)
        {
            float dashStart = Mathf.Max(traveled, 0f);
            float dashEnd = Mathf.Min(traveled + dashLength, length);

            if (dashEnd > dashStart)
            {
                Vector2 segStart = origin + direction * dashStart;
                Vector2 segEnd = origin + direction * dashEnd;

                int baseIndex = vertices.Count;

                vertices.Add(segStart - normal);
                vertices.Add(segStart + normal);
                vertices.Add(segEnd + normal);
                vertices.Add(segEnd - normal);

                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 2);

                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 3);

                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
            }

            traveled += tile;
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colors);
        mesh.RecalculateBounds();
    }
}