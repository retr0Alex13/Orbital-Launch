using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField]
    private Planet planetPrefab;

    [SerializeField]
    private float minSpawnDistance;

    [SerializeField]
    private float maxSpawnDistance;

    [SerializeField]
    private float minDistanceBetween;

    [SerializeField]
    private float maxDistanceBetween;

    [SerializeField]
    private int maxPlanetsToSpawn;

    [SerializeField]
    private float maxAngle;

    [SerializeField]
    private Player player;

    private Planet previousPlanet;

    private void Start()
    {
        player.OnPlayerLaunched += SpawnNextPlanet;

        previousPlanet = SpawnPlanet(Vector2.zero);
        SpawnNextPlanet();
    }

    private void OnDisable()
    {
        player.OnPlayerLaunched -= SpawnNextPlanet;
    }

    private void SpawnNextPlanet()
    {
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        float angle = Random.Range(-maxAngle, maxAngle);

        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        Vector2 position = (Vector2)previousPlanet.transform.position + direction * distance;

        Planet newPlanet = SpawnPlanet(position);
        Planet secondNewPlanet = SpawnPlanet(new Vector3(position.x, position.y + 5f));
        previousPlanet = newPlanet;
    }

    private Planet SpawnPlanet(Vector2 position)
    {
        return Instantiate(planetPrefab, position, Quaternion.identity);
    }
}
