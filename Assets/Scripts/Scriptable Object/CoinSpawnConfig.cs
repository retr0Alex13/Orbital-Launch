using UnityEngine;

[CreateAssetMenu(fileName = "CoinSpawnConfig", menuName = "Config/Coin Spawn Config")]
public sealed class CoinSpawnConfig : ScriptableObject
{
    [Header("Spawn Chance")]
    [Tooltip("Chance that any coins spawn at all for a given planet transition.")]
    [Range(0f, 1f)] public float spawnChance = 0.6f;

    [Tooltip("Chance the coins are placed along the planet's orbit vs. along the path between planets.")]
    [Range(0f, 1f)] public float orbitPathChance = 0.5f;

    [Header("Coin Count")]
    public int minCoins = 3;
    public int maxCoins = 8;

    [Header("Spacing")]
    [Tooltip("Base distance between consecutive coins.")]
    public float spacing = 1.5f;

    [Tooltip("Random +/- variation applied to spacing, as a fraction of spacing.")]
    [Range(0f, 1f)] public float spacingJitter = 0.15f;

    [Header("Coin Values")]
    public int normalCoinValue = 1;
    public int rareCoinValue = 3;

    [Tooltip("Chance for any individual spawned coin to be rare instead of normal.")]
    [Range(0f, 1f)] public float rareCoinChance = 0.15f;

    [Header("Orbit Placement")]
    [Tooltip("Multiplier applied to the planet's orbit radius to place the coin arc (mirrors AsteroidRingConfig.ringRadiusMultiplier).")]
    public float orbitRadiusMultiplier = 1f;

    [Header("Between-Planets Placement")]
    [Tooltip("Minimum empty distance to leave near each planet so coins don't spawn on top of it.")]
    public float planetMargin = 1.5f;

    [Header("Pooling")]
    public int poolSize = 24;
}