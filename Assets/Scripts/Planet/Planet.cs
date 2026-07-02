using System;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float GravityStrength => gravityStrength;
    public float OrbitSpeed => orbitSpeed;
    public float OrbitRadius => orbitCollider.radius * orbitCollider.transform.lossyScale.x;

    [SerializeField]
    private float gravityStrength = 10f;

    [SerializeField]
    private float orbitSpeed = 5f;

    [SerializeField]
    private CircleCollider2D orbitCollider;

    [SerializeField]
    private Transform planetSprite;

    [SerializeField]
    private SpriteRenderer planetSpriteRenderer;

    private Color baseColor;

    private void Awake()
    {
        baseColor = planetSpriteRenderer.color;
    }

    public void Configure(float planetScale, float orbitRadius, float newOrbitSpeed)
    {
        planetSprite.localScale = Vector3.one * planetScale;

        float orbitScale = orbitRadius / orbitCollider.radius;
        orbitCollider.transform.localScale = Vector3.one * orbitScale;

        orbitSpeed = newOrbitSpeed;
    }

    public void SetPlanetSprite(Sprite sprite)
    {
        planetSpriteRenderer.sprite = sprite;
    }

    public void SetDifficultyTint(float difficulty)
    {
        planetSpriteRenderer.color = Color.Lerp(baseColor, Color.red, difficulty);
    }

    public float TriggerRadius
    {
        get
        {
            if (orbitCollider == null)
                orbitCollider = GetComponent<CircleCollider2D>();
            return orbitCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        }
    }
}