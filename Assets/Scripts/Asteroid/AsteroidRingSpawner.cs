using System.Collections.Generic;
using UnityEngine;

public sealed class AsteroidRingSpawner : MonoBehaviour
{
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private AsteroidRingConfig config;

    private AsteroidPool pool;
    private int planetsSinceLastRing;

    private readonly Dictionary<Planet, AsteroidRing> activeRings = new();

    private void Awake()
    {
        pool = gameObject.AddComponent<AsteroidPool>();
        pool.Initialize(asteroidPrefab, config.poolSize);
        planetsSinceLastRing = config.minPlanetsBetweenRings;
    }

    public void SpawnForPlanet(Planet planet, float difficulty)
    {
        DespawnForPlanet(planet);
        planetsSinceLastRing++;

        if (!ShouldSpawnRing(difficulty)) return;
        if (!TryComputeParameters(planet, difficulty, out RingParameters p)) return;

        float gapCenter = Random.Range(0f, 360f);
        List<Asteroid> asteroids = PlaceAsteroids(planet, p, gapCenter);

        float ringRadius = planet.OrbitRadius * config.ringRadiusMultiplier;
        var ring = new AsteroidRing(planet, asteroids, gapCenter, p.GapDeg, p.SpeedDeg, ringRadius);
        activeRings[planet] = ring;

        planetsSinceLastRing = 0;
    }

    private bool ShouldSpawnRing(float difficulty)
    {
        if (difficulty < config.spawnThreshold) return false;
        if (planetsSinceLastRing < config.minPlanetsBetweenRings) return false;

        float t = Mathf.InverseLerp(config.spawnThreshold, 1f, difficulty);
        float chance = Mathf.Lerp(config.minSpawnChance, config.maxSpawnChance, t);

        return Random.value < chance;
    }

    public void DespawnForPlanet(Planet planet)
    {
        if (activeRings.TryGetValue(planet, out AsteroidRing ring))
        {
            foreach (Asteroid a in ring.Asteroids)
                pool.Return(a);
            activeRings.Remove(planet);
        }
    }


    private bool TryComputeParameters(Planet planet, float difficulty, out RingParameters result)
    {
        float t = Mathf.InverseLerp(config.spawnThreshold, 1f, difficulty);

        float coverage = Mathf.Lerp(0f, config.maxCoverage,
                                    Mathf.Pow(t, config.coverageCurvePower));

        float gapDeg = Mathf.Lerp(config.maxGapDegrees, config.minGapDegrees, t);

        float usableArc = 360f - gapDeg;
        int count = Mathf.FloorToInt(coverage * usableArc
                                           / config.asteroidAngularFootprintDeg);

        if (count <= 0) { result = default; return false; }

        float baseSpeed = Mathf.Lerp(config.minRingSpeedDeg, config.maxRingSpeedDeg, t);
        float jitter = Random.Range(0.85f, 1.15f);
        float speedDeg = planet.OrbitSpeed >= 0f
            ? baseSpeed * jitter
            : -baseSpeed * jitter;

        result = new RingParameters(count, gapDeg, speedDeg);
        return true;
    }

    private List<Asteroid> PlaceAsteroids(Planet planet, RingParameters p, float gapCenterDeg)
    {
        float halfGap = p.GapDeg * 0.5f;
        float usableArc = 360f - p.GapDeg;
        float angleStep = usableArc / p.Count;
        float startAngle = gapCenterDeg + halfGap;

        float ringRadius = planet.OrbitRadius * config.ringRadiusMultiplier;

        var asteroids = new List<Asteroid>(p.Count);
        for (int i = 0; i < p.Count; i++)
        {
            float angle = startAngle + angleStep * i;
            float scale = Random.Range(config.minAsteroidScale, config.maxAsteroidScale);
            float radius = ringRadius * Random.Range(1f - config.radiusJitter, 1f + config.radiusJitter);

            Asteroid a = pool.Get();
            a.Activate(planet.transform, angle, p.SpeedDeg, scale, radius);
            asteroids.Add(a);
        }
        return asteroids;
    }


    private readonly struct RingParameters
    {
        public readonly int Count;
        public readonly float GapDeg;
        public readonly float SpeedDeg;

        public RingParameters(int count, float gapDeg, float speedDeg)
        {
            Count = count;
            GapDeg = gapDeg;
            SpeedDeg = speedDeg;
        }
    }
}