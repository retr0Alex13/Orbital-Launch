using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BackgroundSpriteConfig
{
    public Sprite sprite;
    public bool useCustomScale;
    public float customMinScale;
    public float customMaxScale;
    public bool canDrift;
}

public class DynamicBackgroundSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BackgroundElement prefab;
    [SerializeField] private BackgroundSpriteConfig[] backgroundSprites;

    [Header("Global Spawn Settings")]
    [SerializeField] private int maxActiveObjects = 15;
    [SerializeField] private float spawnDistanceInterval = 4f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;

    [Header("Parallax Settings")]
    [SerializeField, Range(0f, 1f)] private float minParallax = 0.1f;
    [SerializeField, Range(0f, 1f)] private float maxParallax = 0.3f;

    [Header("Placement & Cleanup")]
    [SerializeField] private float safeDistancePadding = 2f;
    [SerializeField] private float spawnMargin = 3.5f;
    [SerializeField] private float despawnDistance = 45f;

    [Header("Drift Settings")]
    [SerializeField] private float minDriftSpeed = 0.05f;
    [SerializeField] private float maxDriftSpeed = 0.2f;

    private BackgroundElement[] pool;
    private List<BackgroundElement> activeObjects = new List<BackgroundElement>();
    private Vector3 lastSpawnCameraPos;

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        lastSpawnCameraPos = mainCamera.transform.position;

        pool = new BackgroundElement[maxActiveObjects];
        for (int i = 0; i < maxActiveObjects; i++)
        {
            pool[i] = Instantiate(prefab, transform);
            pool[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < maxActiveObjects; i++)
        {
            TrySpawn(true);
        }
    }

    private void Update()
    {
        CleanupDistantObjects();

        if (Vector3.Distance(mainCamera.transform.position, lastSpawnCameraPos) >= spawnDistanceInterval)
        {
            int missingObjects = maxActiveObjects - activeObjects.Count;
            int objectsToSpawn = Mathf.Min(missingObjects, 2);

            for (int i = 0; i < objectsToSpawn; i++)
            {
                TrySpawn(false);
            }

            lastSpawnCameraPos = mainCamera.transform.position;
        }
    }

    private void TrySpawn(bool initialSpawn)
    {
        if (backgroundSprites == null || backgroundSprites.Length == 0) return;

        BackgroundElement newObj = GetFreeObjectFromPool();
        if (newObj == null) return;

        for (int attempt = 0; attempt < 15; attempt++)
        {
            List<BackgroundSpriteConfig> availableConfigs = new List<BackgroundSpriteConfig>();

            foreach (var config in backgroundSprites)
            {
                bool isUsed = false;
                foreach (var activeObj in activeObjects)
                {
                    if (activeObj.CurrentSprite == config.sprite)
                    {
                        isUsed = true;
                        break;
                    }
                }

                if (!isUsed)
                {
                    availableConfigs.Add(config);
                }
            }

            if (availableConfigs.Count == 0)
            {
                availableConfigs.AddRange(backgroundSprites);
            }

            BackgroundSpriteConfig selectedConfig = availableConfigs[Random.Range(0, availableConfigs.Count)];

            float currentMinScale = selectedConfig.useCustomScale ? selectedConfig.customMinScale : minScale;
            float currentMaxScale = selectedConfig.useCustomScale ? selectedConfig.customMaxScale : maxScale;

            float scale = Random.Range(currentMinScale, currentMaxScale);
            Sprite randomSprite = selectedConfig.sprite;

            float assumedRadius = Mathf.Max(randomSprite.bounds.extents.x, randomSprite.bounds.extents.y) * scale;

            Vector2 spawnPos = GetSpawnPosition(assumedRadius, initialSpawn);

            if (IsPositionSafe(spawnPos, assumedRadius))
            {
                float scalePercent = Mathf.InverseLerp(currentMinScale, currentMaxScale, scale);
                float parallax = Mathf.Lerp(maxParallax, minParallax, scalePercent);

                Color tint = new Color(0.6f, 0.6f, 0.6f, 1f);

                Vector2 driftVelocity = Vector2.zero;
                if (selectedConfig.canDrift)
                {
                    float speed = Random.Range(minDriftSpeed, maxDriftSpeed);
                    driftVelocity = Random.insideUnitCircle.normalized * speed;
                }

                newObj.transform.position = spawnPos;
                newObj.gameObject.SetActive(true);
                newObj.Initialize(randomSprite, scale, parallax, mainCamera.transform, tint, driftVelocity);

                activeObjects.Add(newObj);
                break;
            }
        }
    }

    private Vector2 GetSpawnPosition(float objectRadius, bool initialSpawn)
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector2 camPos = mainCamera.transform.position;

        if (initialSpawn)
        {
            float randomX = Random.Range(-camWidth - spawnMargin, camWidth + spawnMargin);
            float randomY = Random.Range(-camHeight - spawnMargin, camHeight + spawnMargin);
            return camPos + new Vector2(randomX, randomY);
        }
        else
        {
            float spawnX = camWidth + objectRadius + spawnMargin;
            float spawnY = camHeight + objectRadius + spawnMargin;

            Vector2 dir = Random.insideUnitCircle.normalized;

            float absX = Mathf.Abs(dir.x);
            float absY = Mathf.Abs(dir.y);

            float multiplierX = absX > 0 ? spawnX / absX : 0;
            float multiplierY = absY > 0 ? spawnY / absY : 0;

            float multiplier = Mathf.Min(multiplierX, multiplierY);

            return camPos + (dir * multiplier);
        }
    }

    private bool IsPositionSafe(Vector2 position, float radius)
    {
        foreach (var obj in activeObjects)
        {
            float dist = Vector2.Distance(position, obj.transform.position);
            if (dist < (radius + obj.Radius + safeDistancePadding))
            {
                return false;
            }
        }
        return true;
    }

    private void CleanupDistantObjects()
    {
        Vector2 camPos = mainCamera.transform.position;
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            BackgroundElement obj = activeObjects[i];

            if (Vector2.Distance(camPos, obj.transform.position) > despawnDistance)
            {
                obj.gameObject.SetActive(false);
                activeObjects.RemoveAt(i);
            }
        }
    }

    private BackgroundElement GetFreeObjectFromPool()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
                return pool[i];
        }
        return null;
    }
}