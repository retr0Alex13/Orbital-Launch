using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public int Value { get; private set; }
    public CoinType Type { get; private set; }

    public void Initialize(int value, CoinType type = CoinType.Normal)
    {
        Value = value;
        Type = type;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Coin OnTriggerEnter");
        if (collision.TryGetComponent(out CoinCollector coinCollector))
        {
            coinCollector.Collect(this);
        }
    }
}

public enum CoinType
{
    Normal,
    Rare
}