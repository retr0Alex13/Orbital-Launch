using System.Collections.Generic;
using UnityEngine;

public sealed class CoinPool : MonoBehaviour
{
    private Coin prefab;
    private readonly Queue<Coin> available = new();

    public void Initialize(Coin coinPrefab, int poolSize)
    {
        prefab = coinPrefab;
        for (int i = 0; i < poolSize; i++)
            available.Enqueue(CreateInstance());
    }

    private Coin CreateInstance()
    {
        Coin instance = Instantiate(prefab, transform);
        instance.gameObject.SetActive(false);
        return instance;
    }

    public Coin Get()
    {
        Coin coin = available.Count > 0 ? available.Dequeue() : CreateInstance();
        coin.gameObject.SetActive(true);
        return coin;
    }

    public void Return(Coin coin)
    {
        coin.gameObject.SetActive(false);
        coin.transform.SetParent(transform);
        available.Enqueue(coin);
    }
}