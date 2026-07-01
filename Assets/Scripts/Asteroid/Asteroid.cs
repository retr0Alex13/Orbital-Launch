using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public sealed class Asteroid : MonoBehaviour
{
    public bool IsActive { get; private set; }

    private CircleCollider2D hazardCollider;
    private Transform planetTransform;
    private float angleDeg;
    private float orbitRadius;
    private float angularSpeedDeg;

    private void Awake()
    {
        hazardCollider = GetComponent<CircleCollider2D>();
        hazardCollider.isTrigger = true;
    }

    public void Activate(
        Transform target,
        float startAngleDeg,
        float angularSpeedDeg,
        float scale,
        float radius)
    {
        planetTransform = target;
        angleDeg = startAngleDeg;
        this.angularSpeedDeg = angularSpeedDeg;
        orbitRadius = radius;
        transform.localScale = Vector3.one * scale;

        gameObject.SetActive(true);
        IsActive = true;
        hazardCollider.enabled = true;
        UpdatePosition();
    }

    public void Deactivate()
    {
        planetTransform = null;
        hazardCollider.enabled = false;
        gameObject.SetActive(false);
        IsActive = false;
    }

    private void Update()
    {
        if (!IsActive || planetTransform == null) return;
        angleDeg += angularSpeedDeg * Time.deltaTime;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        transform.position = (Vector2)planetTransform.position
            + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
    }
}