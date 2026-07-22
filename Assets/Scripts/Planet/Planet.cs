using PrimeTween;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [SerializeField]
    private OrbitShockWaveController orbitVisual;

    private float rotatedAnimationSpeed;
    private float scaleAnimationPercent;
    private float planetScale;
    private Color baseColor;
    private Tween scaleTween;

    private void Awake()
    {
        baseColor = planetSpriteRenderer.color;
    }

    private void Update()
    {
        planetSprite.Rotate(Vector3.forward * rotatedAnimationSpeed * Time.deltaTime);
    }

    public void Configure(PlanetSettings planetSettings)
    {
        planetSprite.localScale = Vector3.one * planetSettings.PlanetScale;
        planetScale = planetSettings.PlanetScale;
        scaleAnimationPercent = planetSettings.ScaleAnimationPercent;

        float orbitScale = planetSettings.OrbitRadius / orbitCollider.radius;
        orbitCollider.transform.localScale = Vector3.one * orbitScale;

        orbitSpeed = planetSettings.NewOrbitSpeed;

        float calculatedRotationSpeed = Random.Range(planetSettings.MinRotationSpeed, planetSettings.MaxRotationSpeed);
        rotatedAnimationSpeed = 360f / calculatedRotationSpeed;

        scaleTween.Stop();
        float targetScale = planetScale * (1f + scaleAnimationPercent);
        scaleTween = Tween.Scale(planetSprite, endValue: Vector3.one * targetScale, duration: 1f, Ease.InOutSine, cycles: -1, CycleMode.Yoyo);
    }

    public void SetPlanetSprite(Sprite sprite)
    {
        planetSpriteRenderer.sprite = sprite;
    }

    public void SetDifficultyTint(float difficulty)
    {
        planetSpriteRenderer.color = Color.Lerp(baseColor, Color.red, difficulty);
    }

    public void PlayShockWaveEffect(Vector2 position)
    {
        orbitVisual.PlayWave(position);
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

public struct PlanetSettings
{
    public float PlanetScale { get; private set; }
    public float ScaleAnimationPercent { get; private set; }
    public float MinRotationSpeed { get; private set; }
    public float MaxRotationSpeed { get; private set; }
    public float OrbitRadius { get; private set; }
    public float NewOrbitSpeed { get; private set; }

    public PlanetSettings(float planetScale, float scaleAnimationPercent, float minRotationSpeed, float maxRotationSpeed, float orbitRadius, float newOrbitSpeed)
    {
        PlanetScale = planetScale;
        ScaleAnimationPercent = scaleAnimationPercent;
        MinRotationSpeed = minRotationSpeed;
        MaxRotationSpeed = maxRotationSpeed;
        OrbitRadius = orbitRadius;
        NewOrbitSpeed = newOrbitSpeed;
    }
}