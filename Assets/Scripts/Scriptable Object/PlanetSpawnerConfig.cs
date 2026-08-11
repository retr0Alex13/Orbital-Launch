using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlanetSpawnerConfig", menuName = "Config/Planet Spawner Config")]
public class PlanetSpawnerConfig : ScriptableObject
{
    [Header("Spacing")]
    public float minSpawnDistance;
    public float maxSpawnDistance;
    public float maxAngle;

    [Header("Lookahead")]
    public int lookaheadPlanets = 2;

    [Header("Planet")]
    public float minPlanetScale;
    public float maxPlanetScale;

    public float minRotationSpeed;
    public float maxRotationSpeed;

    public float scaleAnimationPercent;

    [Header("Orbit")]
    public float minOrbitRadius;
    public float maxOrbitRadius;
    public float minOrbitSpeed;
    public float maxOrbitSpeed;

    [Header("Orbit speed fluctuation")]
    public float speedOscillationAmplitude = 0.3f;
    public float speedOscillationFrequency = 0.01f;

    [Header("Difficulty")]
    public float difficultyRampDistance = 500f;

    [Header("Pooling")]
    public int maxPlanetsToSpawn;

    [Header("Asteroids")]
    public float minAsteroidScale = 0.2f;
    public float maxAsteroidScale = 0.6f;

    [Header("Planet Sprites")]
    public Sprite[] planets;
}