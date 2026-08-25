using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField] private float initialSpawnOffset = 2f;
    [SerializeField] private float planetCleanupDistance = 40f;
    [SerializeField] private float detachedObjectCleanupDistance = 30f;
    [SerializeField] private Planet planetPrefab;
    [SerializeField] private PlanetSpawnerConfig config;
    [SerializeField] private AsteroidRingSpawner asteroidSpawner;
    [SerializeField] private CoinSpawner coinSpawner;

    [SerializeField] private PlayerController player;

    private Planet[] pool;
    private int poolIndex;

    private readonly List<Planet> mainPath = new();
    private readonly List<Planet> activePlanets = new();
    private readonly List<Planet> skippedPlanets = new();

    private float traveledDistance;
    private int previousSpriteIndex;

    private void Start()
    {
        player.OnPlayerCaptured += AdvanceMainPathTo;
        player.OnPlanetLeft += HandlePlanetLeft;

        pool = new Planet[config.maxPlanetsToSpawn];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(planetPrefab);
            pool[i].gameObject.SetActive(false);
        }

        mainPath.Add(SpawnPlanet(Vector2.right * initialSpawnOffset, 0f));

        for (int i = 0; i < config.lookaheadPlanets; i++)
            SpawnAheadOnMainPath();
    }

    private void OnDisable()
    {
        player.OnPlayerCaptured -= AdvanceMainPathTo;
        player.OnPlanetLeft -= HandlePlanetLeft;
    }

    private void Update()
    {
        Vector2 playerPosition = player.transform.position;
        coinSpawner.CleanupBehindPlayer(playerPosition, detachedObjectCleanupDistance);
        asteroidSpawner.CleanupBehindPlayer(playerPosition, detachedObjectCleanupDistance);
        CleanupSkippedPlanets(playerPosition);
    }

    private void CleanupSkippedPlanets(Vector2 playerPosition)
    {
        for (int i = skippedPlanets.Count - 1; i >= 0; i--)
        {
            Planet planet = skippedPlanets[i];

            if (!planet.gameObject.activeSelf)
            {
                skippedPlanets.RemoveAt(i);
                continue;
            }

            if (Vector2.Distance(playerPosition, planet.transform.position) <= planetCleanupDistance)
                continue;

            skippedPlanets.RemoveAt(i);
            ReturnPlanetToPool(planet);
        }
    }

    private void AdvanceMainPathTo(Planet landedPlanet)
    {
        int index = mainPath.IndexOf(landedPlanet);

        if (index > 0)
        {
            for (int i = 0; i < index; i++)
                skippedPlanets.Add(mainPath[i]);

            mainPath.RemoveRange(0, index);
        }

        SpawnAheadOnMainPath();
    }

    private void SpawnAheadOnMainPath()
    {
        Planet tail = mainPath[^1];
        float difficulty = GetDifficulty();

        float newOrbitRadius = RandomBiasedLow(config.minOrbitRadius, config.maxOrbitRadius, difficulty);

        float gap = RandomBiasedHigh(config.minSpawnDistance, config.maxSpawnDistance, difficulty);
        float distance = tail.OrbitRadius + newOrbitRadius + gap;

        float angle = RandomSignedBiasedHigh(config.maxAngle, difficulty);
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        Vector2 position = (Vector2)tail.transform.position + direction * distance;

        Planet planet = TrySpawnPlanet(position, difficulty, newOrbitRadius);
        if (planet == null) return;

        mainPath.Add(planet);
        traveledDistance += distance;
        coinSpawner.SpawnForPlanet(tail, planet, difficulty);
    }

    private Planet TrySpawnPlanet(Vector2 desiredPosition, float difficulty, float orbitRadius = -1f)
    {
        if (orbitRadius < 0f)
            orbitRadius = RandomBiasedLow(config.minOrbitRadius, config.maxOrbitRadius, difficulty);

        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 candidate = desiredPosition + Random.insideUnitCircle * attempt * 2f;

            if (IsPositionSafe(candidate, orbitRadius))
                return SpawnPlanet(candidate, difficulty, orbitRadius);
        }

        return null;
    }

    private bool IsPositionSafe(Vector2 position, float orbitRadius)
    {
        foreach (Planet planet in activePlanets)
        {
            float dist = Vector2.Distance(position, planet.transform.position);
            float required = orbitRadius + planet.OrbitRadius + 2f;
            if (dist < required) return false;
        }

        return true;
    }

    private Planet SpawnPlanet(Vector2 position, float difficulty, float orbitRadius = -1f)
    {
        Planet planet = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Length;

        asteroidSpawner.DespawnForPlanet(planet);
        coinSpawner.DespawnForPlanet(planet);
        skippedPlanets.Remove(planet);

        planet.OnDespawned -= HandlePlanetDespawned;

        if (planet.gameObject.activeSelf)
            activePlanets.Remove(planet);

        planet.transform.position = position;
        planet.gameObject.SetActive(true);

        float scale = RandomBiasedLow(config.minPlanetScale, config.maxPlanetScale, difficulty);

        if (orbitRadius < 0f)
            orbitRadius = RandomBiasedLow(config.minOrbitRadius, config.maxOrbitRadius, difficulty);

        float speedDifficulty = Mathf.Clamp01(
            difficulty + Mathf.Sin(traveledDistance * config.speedOscillationFrequency) * config.speedOscillationAmplitude
        );
        float orbitSpeed = RandomBiasedHigh(config.minOrbitSpeed, config.maxOrbitSpeed, speedDifficulty);

        int randomIndex = Random.Range(0, config.planets.Length);

        if (randomIndex == previousSpriteIndex && config.planets.Length > 1)
        {
            randomIndex = (randomIndex + 1) % config.planets.Length;
        }

        previousSpriteIndex = randomIndex;

        PlanetSettings planetSettings = new PlanetSettings(scale, config.scaleAnimationPercent, config.minRotationSpeed, config.maxRotationSpeed, orbitRadius, orbitSpeed);

        planet.Configure(planetSettings);
        planet.SetPlanetSprite(config.planets[randomIndex]);
        planet.SetDifficultyTint(speedDifficulty);

        activePlanets.Add(planet);
        asteroidSpawner.SpawnForPlanet(planet, difficulty);

        return planet;
    }

    private void HandlePlanetLeft(Planet planet)
    {
        if (planet == null) return;
        ReturnPlanetToPool(planet);
    }

    private void ReturnPlanetToPool(Planet planet)
    {
        if (!planet.gameObject.activeSelf)
            return;

        planet.OnDespawned -= HandlePlanetDespawned;
        planet.OnDespawned += HandlePlanetDespawned;
        planet.ReturnToPool();
    }

    private void HandlePlanetDespawned(Planet planet)
    {
        planet.OnDespawned -= HandlePlanetDespawned;
        activePlanets.Remove(planet);
        asteroidSpawner.DetachFromPlanet(planet);
        coinSpawner.DetachFromPlanet(planet);
    }

    public Planet GetNextPlanetAfter(Planet current)
    {
        int index = mainPath.IndexOf(current);
        if (index >= 0 && index + 1 < mainPath.Count)
            return mainPath[index + 1];

        return null;
    }

    private float GetDifficulty() =>
        1f - Mathf.Exp(-traveledDistance / config.difficultyRampDistance);

    private float RandomBiasedHigh(float min, float max, float difficulty) =>
        Random.Range(Mathf.Lerp(min, max, difficulty * 0.5f), max);

    private float RandomBiasedLow(float min, float max, float difficulty) =>
        Random.Range(min, Mathf.Lerp(max, min, difficulty * 0.5f));

    private float RandomSignedBiasedHigh(float maxMagnitude, float difficulty)
    {
        float magnitude = Random.Range(Mathf.Lerp(0f, maxMagnitude, difficulty * 0.5f), maxMagnitude);
        return Random.value < 0.5f ? -magnitude : magnitude;
    }
}