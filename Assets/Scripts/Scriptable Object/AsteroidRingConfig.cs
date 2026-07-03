using UnityEngine;

[CreateAssetMenu(fileName = "AsteroidRingConfig", menuName = "Config/Asteroid Ring Config")]
public sealed class AsteroidRingConfig : ScriptableObject
{
    [Header("Spawn Gating")]
    [Tooltip("Difficulty at which the first ring can appear.")]
    [Range(0f, 1f)] public float spawnThreshold = 0.20f;

    [Header("Coverage Scaling")]
    [Tooltip("Max fraction of the usable arc covered by asteroids at full difficulty.")]
    [Range(0f, 1f)] public float maxCoverage = 0.85f;
    [Tooltip("Power curve on t before computing coverage. >1 = sparse early, dense late.")]
    public float coverageCurvePower = 1.5f;

    [Header("Safe Gap")]
    [Tooltip("Minimum gap in degrees. ALWAYS guaranteed — fairness contract.")]
    [Range(20f, 120f)] public float minGapDegrees = 38f;
    [Tooltip("Gap at minimum spawn difficulty.")]
    [Range(45f, 180f)] public float maxGapDegrees = 90f;

    [Header("Asteroid Size")]
    public float minAsteroidScale = 0.25f;
    public float maxAsteroidScale = 0.70f;
    [Tooltip("Effective degrees each asteroid blocks including a collision buffer. " +
             "Tune to match your sprite diameter at the expected orbit radius.")]
    public float asteroidAngularFootprintDeg = 14f;

    [Header("Ring Speed (degrees / second)")]
    public float minRingSpeedDeg = 18f;
    public float maxRingSpeedDeg = 85f;

    [Header("Outer Ring Radius")]
    [Tooltip("Ring radius as a multiple of the planet's orbit radius. " +
         "Must stay > 1.0 so asteroids sit outside the capture orbit.")]
    [Range(1.05f, 2.5f)] public float ringRadiusMultiplier = 1.4f;

    [Tooltip("Per-asteroid radius jitter, purely visual.")]
    [Range(0f, 0.15f)] public float radiusJitter = 0.05f;

    [Header("Spawn Probability")]
    [Tooltip("Chance a ring spawns at all, once past spawnThreshold, at minimum qualifying difficulty.")]
    [Range(0f, 1f)] public float minSpawnChance = 0.15f;

    [Tooltip("Chance a ring spawns at maximum difficulty. Keep below 1.0 so even hard planets " +
             "sometimes give the player a breather.")]
    [Range(0f, 1f)] public float maxSpawnChance = 0.65f;

    [Tooltip("Minimum number of planets between two consecutive rings, to avoid rings back-to-back.")]
    public int minPlanetsBetweenRings = 2;

    [Header("Object Pools")]
    public int poolSize = 64;

    [SerializeField]
    public Sprite[] asteroidSprites;
}