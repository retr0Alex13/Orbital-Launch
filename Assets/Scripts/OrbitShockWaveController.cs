using UnityEngine;

public class OrbitShockWaveController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer orbitRenderer;

    [Header("Wave")]
    [SerializeField]
    private float waveDuration = 0.5f;

    [SerializeField]
    private float waveMaxDistance = 1f;

    [SerializeField]
    private float waveStrength = 1f;

    private Material orbitMaterial;

    private float waveTimer;
    private bool isPlaying;

    private static readonly int RingSpawnPosition =
        Shader.PropertyToID("_RingSpawnPosition");

    private static readonly int WaveDistanceFromCenter =
        Shader.PropertyToID("_WaveDistanceFromCenter");

    private static readonly int ShockWaveStrength =
        Shader.PropertyToID("_ShockWaveStrength");

    private void Awake()
    {
        // Створюємо окремий Material Instance
        orbitMaterial = orbitRenderer.material;

        orbitMaterial.SetFloat(WaveDistanceFromCenter, 0f);
        orbitMaterial.SetFloat(ShockWaveStrength, 0f);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        waveTimer += Time.deltaTime;

        float t = waveTimer / waveDuration;

        if (t >= 1f)
        {
            t = 1f;
            isPlaying = false;
        }

        // Плавний рух хвилі від центру
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        float waveDistance =
            Mathf.Lerp(0f, waveMaxDistance, smoothT);

        orbitMaterial.SetFloat(
            WaveDistanceFromCenter,
            waveDistance);

        if (!isPlaying)
        {
            orbitMaterial.SetFloat(
                ShockWaveStrength,
                0f);
        }
    }

    public void PlayWave(Vector2 worldPosition)
    {
        Vector2 uvPosition = WorldToUV(worldPosition);

        orbitMaterial.SetVector(
            RingSpawnPosition,
            uvPosition
        );

        orbitMaterial.SetFloat(
            WaveDistanceFromCenter,
            0f
        );

        orbitMaterial.SetFloat(
            ShockWaveStrength,
            waveStrength
        );

        waveTimer = 0f;
        isPlaying = true;
    }

    private Vector2 WorldToUV(Vector2 worldPosition)
    {
        Vector3 localPosition = orbitRenderer.transform.InverseTransformPoint(worldPosition);
        Vector2 spriteSize = orbitRenderer.sprite.bounds.size;

        float u = localPosition.x / spriteSize.x + 0.5f;
        float v = localPosition.y / spriteSize.y + 0.5f;

        return new Vector2(u, v);
    }
}