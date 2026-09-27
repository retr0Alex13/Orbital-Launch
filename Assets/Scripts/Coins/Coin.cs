using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public int Value { get; private set; }
    public CoinType Type { get; private set; }

    private CoinMagnetSettings magnet;
    private float magnetCurrentSpeed;
    private bool isMagnetized;

    public void Initialize(int value, CoinType type, CoinMagnetSettings magnetSettings)
    {
        Value = value;
        Type = type;
        magnet = magnetSettings;

        isMagnetized = false;
        magnetCurrentSpeed = 0f;
    }

    private void Update()
    {
        if (magnet.Target == null || magnet.Radius <= 0f) return;

        Vector2 toTarget = (Vector2)magnet.Target.position - (Vector2)transform.position;

        if (!isMagnetized)
        {
            if (toTarget.sqrMagnitude > magnet.Radius * magnet.Radius) return;
            isMagnetized = true;
        }

        magnetCurrentSpeed = Mathf.Min(magnetCurrentSpeed + magnet.Acceleration * Time.deltaTime, magnet.MaxSpeed);
        transform.position += (Vector3)(toTarget.normalized * magnetCurrentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

public readonly struct CoinMagnetSettings
{
    public readonly Transform Target;
    public readonly float Radius;
    public readonly float Acceleration;
    public readonly float MaxSpeed;

    public CoinMagnetSettings(Transform target, float radius, float acceleration, float maxSpeed)
    {
        Target = target;
        Radius = radius;
        Acceleration = acceleration;
        MaxSpeed = maxSpeed;
    }
}