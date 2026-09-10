using System;
using UnityEngine;

public class PlayerCoinsController : MonoBehaviour
{
    public event Action<int> OnCoinCollected;
    public event Action<int> OnCoinsSpent;

    public int Coins { get; private set; }

    private void Awake()
    {
        Coins = PlayerPrefs.GetInt(Constants.PLAYER_COINS_KEY, 0);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Coins += amount;
        Save();
        OnCoinCollected?.Invoke(Coins);
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount <= 0 || Coins < amount)
            return false;

        Coins -= amount;
        Save();
        OnCoinsSpent?.Invoke(Coins);
        return true;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(Constants.PLAYER_COINS_KEY, Coins);
        PlayerPrefs.Save();
    }
}