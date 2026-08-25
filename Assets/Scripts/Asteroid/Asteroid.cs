using UnityEngine;

public sealed class Asteroid : MonoBehaviour
{
    public bool IsActive { get; private set; }

    [SerializeField]
    private CircleCollider2D asteroidCollider;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField, Range(0.5f, 1f)]
    private float colliderFitMultiplier = 1f;

    private Transform planetTransform;
    private float angleDeg;
    private float orbitRadius;
    private float angularSpeedDeg;
    private float targetWorldDiameter;
    private Vector2 driftVelocity;
    private bool isOrbiting;

    public void Activate(
        Transform target,
        float startAngleDeg,
        float angularSpeedDeg,
        float worldDiameter,
        float radius)
    {
        planetTransform = target;
        angleDeg = startAngleDeg;
        this.angularSpeedDeg = angularSpeedDeg;
        orbitRadius = radius;
        targetWorldDiameter = worldDiameter;

        gameObject.SetActive(true);
        IsActive = true;
        asteroidCollider.enabled = true;
        UpdatePosition();
    }

    public void Deactivate()
    {
        planetTransform = null;
        asteroidCollider.enabled = false;
        gameObject.SetActive(false);
        IsActive = false;
    }

    public void SetAsteroidSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;

        float visualRadius = Mathf.Max(sprite.bounds.extents.x, sprite.bounds.extents.y);
        asteroidCollider.radius = visualRadius * colliderFitMultiplier;

        ApplyTargetWorldScale(sprite);
    }

    private void ApplyTargetWorldScale(Sprite sprite)
    {
        float nativeDiameter = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);

        float scaleFactor = targetWorldDiameter / nativeDiameter;
        transform.localScale = Vector3.one * scaleFactor;
    }

    private void Update()
    {
        if (!IsActive) return;

        if (planetTransform != null)
        {
            angleDeg += angularSpeedDeg * Time.deltaTime;
            UpdatePosition();
        }
        else if (!isOrbiting)
        {
            transform.position += (Vector3)(driftVelocity * Time.deltaTime);
        }
    }

    private void UpdatePosition()
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        transform.position = (Vector2)planetTransform.position
            + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
    }

    public void DetachAnchor()
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
        float angularSpeedRad = angularSpeedDeg * Mathf.Deg2Rad;
        driftVelocity = tangent * (angularSpeedRad * orbitRadius);

        planetTransform = null;
        isOrbiting = false;
    }
}