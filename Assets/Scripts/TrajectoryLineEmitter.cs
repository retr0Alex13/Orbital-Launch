using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrajectoryLineEmitter : MonoBehaviour
{
    public event Action OnOrbitHitDetected;

    [SerializeField] private PlayerController player;
    [SerializeField] private LayerMask planetLayerMask;

    [SerializeField] private float rayLength = 6f;
    [SerializeField, Range(0f, 1f)]
    private float minLengthScale = 0.3f;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float dashLength = 0.3f;
    [SerializeField] private float gapLength = 0.2f;
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private Color minPowerColor = Color.white;
    [SerializeField] private Color maxPowerColor = Color.red;

    [Header("Smoothing")]
    [SerializeField, Range(1f, 60f)] private float followSharpness = 25f;

    [Header("Hit Detection")]
    [SerializeField] private float minTimeBeforeHitDetection = 0.4f;

    [Header("Sorting")]
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int sortingOrder = 0;

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private readonly List<Vector3> vertices = new();
    private readonly List<int> triangles = new();
    private readonly List<Color> colors = new();
    private readonly Collider2D[] overlapBuffer = new Collider2D[16];

    private Vector2 smoothedOrigin;
    private Vector2 smoothedDirection = Vector2.up;
    private bool wasVisible;
    private float lastCaptureUnscaledTime = -Mathf.Infinity;

    private Color currentLineColor;

    private void Awake()
    {
        mesh = new Mesh { name = "DashedLineMesh" };
        GetComponent<MeshFilter>().mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = sortingOrder;
    }

    private void OnEnable()
    {
        if (player != null)
            player.OnPlayerCaptured += HandlePlayerCaptured;
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnPlayerCaptured -= HandlePlayerCaptured;
    }

    private void HandlePlayerCaptured(Planet planet)
    {
        lastCaptureUnscaledTime = Time.unscaledTime;
    }

    private void LateUpdate()
    {
        bool visible = player.CurrentPlanet != null && player.IsAiming;

        if (!visible)
        {
            meshRenderer.enabled = false;
            wasVisible = false;
            return;
        }

        meshRenderer.enabled = true;

        float dt = Time.unscaledDeltaTime;

        Vector2 targetOrigin = player.transform.position;
        Vector2 targetDirection = player.AimDirection;

        if (!wasVisible)
        {
            smoothedOrigin = targetOrigin;
            smoothedDirection = targetDirection;
        }
        else
        {
            float t = 1f - Mathf.Exp(-followSharpness * dt);
            smoothedOrigin = Vector2.Lerp(smoothedOrigin, targetOrigin, t);
            smoothedDirection = ((Vector2)Vector3.Slerp(smoothedDirection, targetDirection, t)).normalized;
        }

        wasVisible = true;

        float length = Mathf.Lerp(rayLength * minLengthScale, rayLength, player.AimPower);
        bool hitFound = TryFindNearestOrbitHit(smoothedOrigin, smoothedDirection, player.CurrentPlanet, out float hitDistance);

        if (hitFound)
        {
            length = Mathf.Min(length, hitDistance);

            bool pastMinDelay = Time.unscaledTime - lastCaptureUnscaledTime >= minTimeBeforeHitDetection;
            if (pastMinDelay)
                OnOrbitHitDetected?.Invoke();
        }

        currentLineColor = Color.Lerp(minPowerColor, maxPowerColor, Mathf.SmoothStep(0f, 1f, player.AimPower));

        BuildDashedMesh(smoothedOrigin, smoothedDirection, length, currentLineColor);
    }

    private bool TryFindNearestOrbitHit(Vector2 origin, Vector2 direction, Planet ignorePlanet, out float hitDistance)
    {
        hitDistance = rayLength;
        bool found = false;

        int count = Physics2D.OverlapCircleNonAlloc(origin, rayLength, overlapBuffer, planetLayerMask);

        for (int i = 0; i < count; i++)
        {
            if (!overlapBuffer[i].TryGetComponent(out Planet planet))
                planet = overlapBuffer[i].GetComponentInParent<Planet>();

            if (planet == null || planet == ignorePlanet)
                continue;

            if (RayIntersectsCircle(origin, direction, planet.transform.position, planet.OrbitRadius, out float distance)
                && distance < hitDistance)
            {
                hitDistance = distance;
                found = true;
            }
        }

        return found;
    }

    private static bool RayIntersectsCircle(Vector2 origin, Vector2 direction, Vector2 center, float radius, out float distance)
    {
        Vector2 toCenter = center - origin;
        float tClosest = Vector2.Dot(toCenter, direction);

        if (tClosest < 0f)
        {
            distance = 0f;
            return false;
        }

        Vector2 closestPoint = origin + direction * tClosest;
        float distToCenterSqr = (closestPoint - center).sqrMagnitude;
        float radiusSqr = radius * radius;

        if (distToCenterSqr > radiusSqr)
        {
            distance = 0f;
            return false;
        }

        float halfChord = Mathf.Sqrt(radiusSqr - distToCenterSqr);
        distance = tClosest - halfChord;

        if (distance < 0f)
            distance = tClosest + halfChord;

        return distance >= 0f;
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