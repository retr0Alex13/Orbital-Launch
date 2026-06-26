using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField] private Planet planetPrefab;
    [SerializeField] private PlanetSpawnerConfig config;
    [SerializeField] private AsteroidSpawner asteroidSpawner;

    [SerializeField] private Player player;

    private Planet[] pool;
    private int poolIndex;

    private readonly List<Planet> mainPath = new();
    private readonly List<Planet> activePlanets = new();

    private float traveledDistance;

    public Planet NextPlanet => mainPath.Count > 1 ? mainPath[1] : null;

    private void Start()
    {
        player.OnPlayerLaunched += AdvanceMainPath;

        pool = new Planet[config.maxPlanetsToSpawn];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(planetPrefab);
            pool[i].gameObject.SetActive(false);
        }

        mainPath.Add(SpawnPlanet(Vector2.zero, 0f));

        for (int i = 0; i < config.lookaheadPlanets; i++)
            SpawnAheadOnMainPath();
    }

    private void OnDisable()
    {
        player.OnPlayerLaunched -= AdvanceMainPath;
    }

    private void AdvanceMainPath()
    {
        if (mainPath.Count > 0)
            mainPath.RemoveAt(0);

        SpawnAheadOnMainPath();
    }

    private void SpawnAheadOnMainPath()
    {
        Planet tail = mainPath[^1];
        float difficulty = GetDifficulty();

        float distance = RandomBiasedHigh(config.minSpawnDistance, config.maxSpawnDistance, difficulty);
        float angle = RandomSignedBiasedHigh(config.maxAngle, difficulty);
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        Vector2 position = (Vector2)tail.transform.position + direction * distance;

        Planet planet = TrySpawnPlanet(position, difficulty);
        if (planet == null) return;

        mainPath.Add(planet);
        traveledDistance += distance;

        SpawnExtraPlanets(planet.transform.position, direction, difficulty);
    }

    private void SpawnExtraPlanets(Vector2 center, Vector2 forward, float difficulty)
    {
        int count = 0;
        if (difficulty > 0.3f && Random.value < 0.35f) count++;
        if (difficulty > 0.7f && Random.value < 0.15f) count++;

        for (int i = 0; i < count; i++)
        {
            float offset = RandomBiasedHigh(config.minDistanceBetween, config.maxDistanceBetween, difficulty);
            float angle = Random.value < 0.5f ? Random.Range(-90f, -40f) : Random.Range(40f, 90f);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * forward;

            TrySpawnPlanet(center + direction * offset, difficulty);
        }
    }

    private Planet TrySpawnPlanet(Vector2 desiredPosition, float difficulty)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            float orbitRadius = RandomBiasedLow(config.minOrbitRadius, config.maxOrbitRadius, difficulty);
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

        planet.Configure(scale, orbitRadius, orbitSpeed);
        planet.SetDifficultyTint(speedDifficulty);

        activePlanets.Add(planet);
        asteroidSpawner.SpawnForPlanet(planet, difficulty);

        return planet;
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