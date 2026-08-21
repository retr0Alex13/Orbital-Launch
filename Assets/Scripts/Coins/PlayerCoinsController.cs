using System;
using UnityEngine;

public class PlayerCoinsController : MonoBehaviour
{
    public event Action<int> OnCoinCollected;

    public int Coins { get; private set; }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Coins += amount;
        OnCoinCollected?.Invoke(Coins);
    }
}
