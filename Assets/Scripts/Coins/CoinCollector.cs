using AudioSystem;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private PlayerCoinsController coinsController;
    [SerializeField] private CoinSpawner coinSpawner;
    [SerializeField] private SoundData coinSound;

    [Header("Coin Pickup Combo Pitch")]
    [SerializeField] private float basePitch = 1f;
    [SerializeField] private float comboPitchStep = 0.1f;
    [SerializeField] private float maxComboPitch = 2f;
    [SerializeField] private float comboResetTime = 0.5f;

    private int comboCount;
    private float lastCollectTime = -Mathf.Infinity;

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
        PlayCoinSound();
    }

    private void PlayCoinSound()
    {
        if (coinSound == null) return;

        if (Time.time - lastCollectTime > comboResetTime)
        {
            comboCount = 0;
        }

        lastCollectTime = Time.time;

        float pitch = Mathf.Min(basePitch + comboCount * comboPitchStep, maxComboPitch);
        comboCount++;

        SoundEmitter emitter = SoundManager.Instance.Get();
        emitter.Initialize(coinSound);
        emitter.WithPitch(pitch);
        emitter.Play();
    }
}