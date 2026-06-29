using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private PlanetSpawnerConfig config;

    public void SpawnForPlanet(Planet planet, float difficulty)
    {
        int count = CountForDifficulty(difficulty);

        float angleStep = count > 0 ? 360f / count : 0f;
        float baseAngle = Random.Range(0f, 360f);

        for (int i = 0; i < count; i++)
        {
            Asteroid a = Instantiate(asteroidPrefab, planet.transform);

            float actualOrbitRadius = planet.OrbitRadius * Random.Range(0.7f, 1.0f);
            float degsPerSec = (planet.OrbitSpeed / actualOrbitRadius) * Mathf.Rad2Deg;
            float speed = degsPerSec;
            float scale = Random.Range(config.minAsteroidScale, config.maxAsteroidScale);

            a.Configure(planet, baseAngle + angleStep * i, speed, scale, actualOrbitRadius);
        }
    }

    private int CountForDifficulty(float difficulty)
    {
        if (difficulty < 0.2f) return 0;

        float t = Mathf.InverseLerp(0.2f, 1f, difficulty);
        int max = Mathf.RoundToInt(Mathf.Lerp(1f, 4f, t));
        return Random.Range(0, max + 1);
    }
}