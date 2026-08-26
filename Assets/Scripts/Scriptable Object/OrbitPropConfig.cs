using UnityEngine;

[CreateAssetMenu(fileName = "OrbitPropConfig", menuName = "Config/Orbit Prop Config")]
public sealed class OrbitPropConfig : ScriptableObject
{
    [Header("Spawn Gating")]
    [Range(0f, 1f)] public float spawnThreshold = 0.10f;
    [Range(0f, 1f)] public float minSpawnChance = 0.20f;
    [Range(0f, 1f)] public float maxSpawnChance = 0.55f;
    public int minPlanetsBetweenSpawns = 1;

    [Header("Count")]
    public int minCount = 1;
    public int maxCount = 2;

    public float angularFootprintDeg = 25f;

    [Header("Orbit Radius")]
    [Range(1.05f, 3f)] public float orbitRadiusMultiplier = 1.3f;

    [Header("Speed (degrees / second)")]
    public float minSpeedDeg = 10f;
    public float maxSpeedDeg = 60f;
    [Range(0f, 1f)] public float reverseDirectionChance = 0.3f;

    [Header("Placement")]
    public int placementMaxAttempts = 12;

    [Header("Object Pool")]
    public int poolSize = 16;

    [Header("Visual Variants")]
    public OrbitPropVariant[] variants;
}

[System.Serializable]
public struct OrbitPropVariant
{
    public Sprite sprite;
    public float minScale;
    public float maxScale;
}