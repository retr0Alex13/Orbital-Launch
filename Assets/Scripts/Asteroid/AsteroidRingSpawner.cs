using System.Collections.Generic;
using UnityEngine;

public sealed class AsteroidRingSpawner : MonoBehaviour
{
    public bool HasActiveRing(Planet planet) => activeRings.ContainsKey(planet);

    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private AsteroidRingConfig config;

    private AsteroidPool pool;
    private int planetsSinceLastRing;

    private readonly Dictionary<Planet, AsteroidRing> activeRings = new();
    private readonly Dictionary<Asteroid, Planet> asteroidOwners = new();
    private readonly List<Asteroid> detachedAsteroids = new();

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
        var ring = new AsteroidRing(planet, asteroids, gapCenter, p.GapDeg, p.SpeedDeg,
                                    ringRadius, p.GapCount);
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
                ReturnToPool(a);
            activeRings.Remove(planet);
        }
    }

    public void DetachFromPlanet(Planet planet)
    {
        if (!activeRings.TryGetValue(planet, out AsteroidRing ring))
            return;

        activeRings.Remove(planet);

        foreach (Asteroid a in ring.Asteroids)
        {
            asteroidOwners.Remove(a);
            a.DetachAnchor();
            detachedAsteroids.Add(a);
        }
    }

    public void CleanupBehindPlayer(Vector2 playerPosition, float cleanupDistance)
    {
        for (int i = detachedAsteroids.Count - 1; i >= 0; i--)
        {
            Asteroid a = detachedAsteroids[i];

            if (!a.IsActive)
            {
                detachedAsteroids.RemoveAt(i);
                continue;
            }

            if (Vector2.Distance(playerPosition, a.transform.position) > cleanupDistance)
            {
                ReturnToPool(a);
                detachedAsteroids.RemoveAt(i);
            }
        }
    }

    private void SubscribeAsteroid(Asteroid asteroid, Planet planet)
    {
        asteroid.OnDestroyedByCollision += HandleAsteroidDestroyed;
        asteroidOwners[asteroid] = planet;
    }

    private void HandleAsteroidDestroyed(Asteroid asteroid)
    {
        if (asteroidOwners.TryGetValue(asteroid, out Planet planet))
        {
            if (activeRings.TryGetValue(planet, out AsteroidRing ring))
            {
                ring.RemoveAsteroid(asteroid);
                if (ring.IsEmpty)
                    activeRings.Remove(planet);
            }
        }
        else
        {
            detachedAsteroids.Remove(asteroid);
        }

        ReturnToPool(asteroid);
    }

    private void ReturnToPool(Asteroid asteroid)
    {
        asteroid.OnDestroyedByCollision -= HandleAsteroidDestroyed;
        asteroidOwners.Remove(asteroid);
        pool.Return(asteroid);
    }

    private int GetGapCount(float t)
    {
        if (t >= config.fourGapsFromT) return 4;
        if (t >= config.twoGapsFromT) return 2;
        return 1;
    }

    private bool TryComputeParameters(Planet planet, float difficulty, out RingParameters result)
    {
        float t = Mathf.InverseLerp(config.spawnThreshold, 1f, difficulty);

        int gapCount = GetGapCount(t);
        float sectorArc = 360f / gapCount;

        float gapDeg = Mathf.Lerp(config.maxGapDegrees, config.minGapDegrees, t);
        gapDeg = Mathf.Min(gapDeg, sectorArc - config.asteroidAngularFootprintDeg);

        float asteroidArc = sectorArc - gapDeg;
        float coverage = Mathf.Lerp(0f, config.maxCoverage,
                                    Mathf.Pow(t, config.coverageCurvePower));

        int perSector = Mathf.FloorToInt(coverage * asteroidArc
                                         / config.asteroidAngularFootprintDeg);

        if (gapCount > 1) perSector = Mathf.Max(perSector, 1);
        if (perSector <= 0) { result = default; return false; }

        float baseSpeed = Mathf.Lerp(config.minRingSpeedDeg, config.maxRingSpeedDeg,
                                     Mathf.Pow(t, config.speedCurvePower));
        float jitter = Random.Range(0.85f, 1.15f);
        float speedDeg = planet.OrbitSpeed >= 0f
            ? baseSpeed * jitter
            : -baseSpeed * jitter;

        result = new RingParameters(gapCount, perSector, gapDeg, speedDeg);
        return true;
    }

    private List<Asteroid> PlaceAsteroids(Planet planet, RingParameters p, float gapCenterDeg)
    {
        float sectorArc = 360f / p.GapCount;
        float step = (sectorArc - p.GapDeg) / p.PerSector;
        float ringRadius = planet.OrbitRadius * config.ringRadiusMultiplier;

        var asteroids = new List<Asteroid>(p.GapCount * p.PerSector);

        for (int s = 0; s < p.GapCount; s++)
        {
            float sectorStart = gapCenterDeg + s * sectorArc + p.GapDeg * 0.5f;

            for (int i = 0; i < p.PerSector; i++)
            {
                float angle = sectorStart + step * (i + 0.5f);
                float scale = Random.Range(config.minAsteroidScale, config.maxAsteroidScale);
                float radius = ringRadius * Random.Range(1f - config.radiusJitter,
                                                          1f + config.radiusJitter);
                int randomIndex = Random.Range(0, config.asteroidSprites.Length);

                Asteroid asteroid = pool.Get();
                asteroid.Activate(planet.transform, angle, p.SpeedDeg, scale, radius);
                asteroid.SetAsteroidSprite(config.asteroidSprites[randomIndex]);
                SubscribeAsteroid(asteroid, planet);
                asteroids.Add(asteroid);
            }
        }
        return asteroids;
    }

    private readonly struct RingParameters
    {
        public readonly int GapCount;
        public readonly int PerSector;
        public readonly float GapDeg;
        public readonly float SpeedDeg;

        public RingParameters(int gapCount, int perSector, float gapDeg, float speedDeg)
        {
            GapCount = gapCount;
            PerSector = perSector;
            GapDeg = gapDeg;
            SpeedDeg = speedDeg;
        }
    }
}