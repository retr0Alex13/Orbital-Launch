using System;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public event Action OnCoinCollected;

    [SerializeField]
    private PlayerCoinsController coinsController;

    [SerializeField]
    private CoinSpawner coinSpawner;


    private void Awake()
    {
        if (coinsController == null)
        {
            coinsController = FindAnyObjectByType<PlayerCoinsController>();
        }
    }

    public void Collect(Coin coin)
    {
        coinsController.AddCoins(coin.Value);
        coinSpawner.ReturnToPool(coin);
        OnCoinCollected?.Invoke();

        Debug.Log($"Collected {coin.Value} coins. Total coins: {coinsController.Coins}");
    }
}