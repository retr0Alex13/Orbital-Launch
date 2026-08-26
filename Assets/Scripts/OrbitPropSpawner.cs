using System.Collections.Generic;
using UnityEngine;

public sealed class OrbitPropSpawner : MonoBehaviour
{
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private OrbitPropConfig config;
    [SerializeField] private AsteroidRingSpawner ringSpawner;

    private AsteroidPool pool;
    private int planetsSinceLastSpawn;

    private readonly Dictionary<Planet, List<Asteroid>> activeSolo = new();
    private readonly List<Asteroid> detachedAsteroids = new();

    private void Awake()
    {
        pool = gameObject.AddComponent<AsteroidPool>();
        pool.Initialize(asteroidPrefab, config.poolSize);
        planetsSinceLastSpawn = config.minPlanetsBetweenSpawns;
    }

    public void SpawnForPlanet(Planet planet, float difficulty)
    {
        DespawnForPlanet(planet);
        planetsSinceLastSpawn++;

        if (ringSpawner != null && ringSpawner.HasActiveRing(planet))
            return;

        if (!ShouldSpawn(difficulty)) return;

        int count = Random.Range(config.minCount, config.maxCount + 1);
        if (count <= 0) return;

        List<Asteroid> asteroids = PlaceProps(planet, count);
        if (asteroids.Count == 0) return;

        activeSolo[planet] = asteroids;
        planetsSinceLastSpawn = 0;
    }

    private bool ShouldSpawn(float difficulty)
    {
        if (difficulty < config.spawnThreshold) return false;
        if (planetsSinceLastSpawn < config.minPlanetsBetweenSpawns) return false;

        float t = Mathf.InverseLerp(config.spawnThreshold, 1f, difficulty);
        float chance = Mathf.Lerp(config.minSpawnChance, config.maxSpawnChance, t);

        return Random.value < chance;
    }

    public void DespawnForPlanet(Planet planet)
    {
        if (activeSolo.TryGetValue(planet, out List<Asteroid> asteroids))
        {
            foreach (Asteroid a in asteroids)
                pool.Return(a);
            activeSolo.Remove(planet);
        }
    }

    public void DetachFromPlanet(Planet planet)
    {
        if (!activeSolo.TryGetValue(planet, out List<Asteroid> asteroids))
            return;

        activeSolo.Remove(planet);

        foreach (Asteroid a in asteroids)
        {
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
                pool.Return(a);
                detachedAsteroids.RemoveAt(i);
            }
        }
    }

    private List<Asteroid> PlaceProps(Planet planet, int count)
    {
        var placedAngles = new List<float>(count);
        var props = new List<Asteroid>(count);

        for (int i = 0; i < count; i++)
        {
            if (!TryFindFreeAngle(placedAngles, out float angle))
                break;

            placedAngles.Add(angle);

            OrbitPropVariant variant = config.variants[Random.Range(0, config.variants.Length)];
            float scale = Random.Range(variant.minScale, variant.maxScale);

            float radius = planet.OrbitRadius * config.orbitRadiusMultiplier;

            float baseSpeed = Random.Range(config.minSpeedDeg, config.maxSpeedDeg);
            bool reverse = Random.value < config.reverseDirectionChance;
            float speedDeg = planet.OrbitSpeed >= 0f
                ? (reverse ? -baseSpeed : baseSpeed)
                : (reverse ? baseSpeed : -baseSpeed);

            Asteroid prop = pool.Get();
            prop.Activate(planet.transform, angle, speedDeg, scale, radius);
            prop.SetAsteroidSprite(variant.sprite);
            props.Add(prop);
        }

        return props;
    }

    private bool TryFindFreeAngle(List<float> placedAngles, out float angle)
    {
        for (int attempt = 0; attempt < config.placementMaxAttempts; attempt++)
        {
            float candidate = Random.Range(0f, 360f);
            if (IsFarEnough(candidate, placedAngles))
            {
                angle = candidate;
                return true;
            }
        }

        angle = default;
        return false;
    }

    private bool IsFarEnough(float candidate, List<float> placedAngles)
    {
        foreach (float placed in placedAngles)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(candidate, placed)) < config.angularFootprintDeg)
                return false;
        }
        return true;
    }
}