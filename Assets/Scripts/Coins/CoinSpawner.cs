using System.Collections.Generic;
using UnityEngine;

public sealed class CoinSpawner : MonoBehaviour
{
    [SerializeField] private Coin coinPrefab;
    [SerializeField] private CoinSpawnConfig config;

    private CoinPool pool;

    private readonly Dictionary<Planet, List<Coin>> activeCoins = new();

    private void Awake()
    {
        pool = gameObject.AddComponent<CoinPool>();
        pool.Initialize(coinPrefab, config.poolSize);
    }

    public void SpawnForPlanet(Planet planet, Planet nextPlanet, float difficulty)
    {
        DespawnForPlanet(planet);

        if (!ShouldSpawnCoins()) return;

        int count = Random.Range(config.minCoins, config.maxCoins + 1);
        if (count <= 0) return;

        float spacing = config.spacing * Random.Range(1f - config.spacingJitter, 1f + config.spacingJitter);
        bool spawnAlongOrbit = nextPlanet == null || Random.value < config.orbitPathChance;

        List<Coin> coins = spawnAlongOrbit
            ? PlaceAlongOrbit(planet, count, spacing)
            : PlaceBetweenPlanets(planet, nextPlanet, count, spacing);

        if (coins.Count == 0) return;

        activeCoins[planet] = coins;
    }

    private bool ShouldSpawnCoins()
    {
        return Random.value < config.spawnChance;
    }

    public void DespawnForPlanet(Planet planet)
    {
        if (activeCoins.TryGetValue(planet, out List<Coin> coins))
        {
            foreach (Coin coin in coins)
            {
                if (coin.gameObject.activeSelf)
                    pool.Return(coin);
            }
            activeCoins.Remove(planet);
        }
    }

    public void ReturnToPool(Coin coin)
    {
        pool.Return(coin);
    }

    private List<Coin> PlaceAlongOrbit(Planet planet, int count, float spacing)
    {
        float radius = planet.OrbitRadius * config.orbitRadiusMultiplier;
        float angleStepDeg = (spacing / radius) * Mathf.Rad2Deg;
        float startAngle = Random.Range(0f, 360f);

        var coins = new List<Coin>(count);
        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStepDeg * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
            Vector3 position = planet.transform.position + offset;

            coins.Add(SpawnCoinAt(position));
        }
        return coins;
    }

    private List<Coin> PlaceBetweenPlanets(Planet from, Planet to, int count, float spacing)
    {
        Vector3 start = from.transform.position;
        Vector3 end = to.transform.position;
        float totalDistance = Vector3.Distance(start, end);
        Vector3 direction = totalDistance > 0f ? (end - start) / totalDistance : Vector3.right;

        float startMargin = from.OrbitRadius + Mathf.Max(config.planetMargin, spacing);
        float endMargin = to.OrbitRadius + Mathf.Max(config.planetMargin, spacing);
        float usableDistance = totalDistance - startMargin - endMargin;

        var coins = new List<Coin>(count);
        if (usableDistance <= 0f) return coins;

        float trailLength = Mathf.Min(spacing * (count - 1), usableDistance);
        float startOffset = startMargin + (usableDistance - trailLength) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float distanceAlong = startOffset + spacing * i;

            if (distanceAlong > totalDistance - endMargin) break;

            Vector3 position = start + direction * distanceAlong;
            coins.Add(SpawnCoinAt(position));
        }
        return coins;
    }

    private Coin SpawnCoinAt(Vector3 position)
    {
        bool isRare = Random.value < config.rareCoinChance;
        CoinType type = isRare ? CoinType.Rare : CoinType.Normal;
        int value = isRare ? config.rareCoinValue : config.normalCoinValue;

        Coin coin = pool.Get();
        coin.transform.position = position;
        coin.transform.rotation = Quaternion.identity;
        coin.Initialize(value, type);
        return coin;
    }
}