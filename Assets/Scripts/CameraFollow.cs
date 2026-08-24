using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private PlanetSpawner planetSpawner;

    [SerializeField] private float positionSmoothTime = 0.2f;
    [SerializeField] private float sizeSmoothTime = 0.3f;
    [SerializeField] private float framingPadding = 2f;
    [SerializeField] private float minOrthographicSize = 5f;
    [SerializeField] private float maxOrthographicSize = 8f;

    [Header("Aim Offset")]
    [SerializeField] private float maxAimOffsetDistance = 3.5f;
    [SerializeField] private float aimOffsetSmoothTime = 0.25f;
    [SerializeField] private float aimOffsetReturnSmoothTime = 0.35f;
    [SerializeField, Range(0.1f, 0.95f)] private float orbitVisibleAreaFraction = 0.7f;

    private Camera cam;
    private Vector3 positionVelocity;
    private float sizeVelocity;

    private Vector3 lastFramedPosition;
    private float lastFramedSize;

    private Vector3 lastPlayerPosition;
    private Vector3 playerVelocityEstimate;

    private Vector3 currentAimOffset;
    private Vector3 aimOffsetVelocity;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        lastFramedPosition = transform.position;
        lastFramedSize = minOrthographicSize;
        lastPlayerPosition = player.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 currentPlayerPos = player.transform.position;
        if (Time.unscaledDeltaTime > 0f)
        {
            playerVelocityEstimate = (currentPlayerPos - lastPlayerPosition) / Time.unscaledDeltaTime;
        }

        lastPlayerPosition = currentPlayerPos;

        float dt = Time.unscaledDeltaTime;

        Vector3 baseTargetPosition;
        float targetSize;

        if (player.CurrentPlanet == null)
        {
            Vector3 predicted = player.transform.position + playerVelocityEstimate * positionSmoothTime;
            baseTargetPosition = new Vector3(predicted.x, predicted.y, transform.position.z);
            targetSize = minOrthographicSize;
        }
        else
        {
            (baseTargetPosition, targetSize) = ComputeOrbitFramingTarget();
        }

        Vector3 aimOffset = UpdateAimOffset(dt, baseTargetPosition);
        Vector3 finalTargetPosition = baseTargetPosition + aimOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position, finalTargetPosition, ref positionVelocity, positionSmoothTime, Mathf.Infinity, dt);

        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize, targetSize, ref sizeVelocity, sizeSmoothTime, Mathf.Infinity, dt);

        if (player.CurrentPlanet == null)
        {
            lastFramedPosition = transform.position;
            lastFramedSize = cam.orthographicSize;
        }
    }

    private (Vector3 position, float size) ComputeOrbitFramingTarget()
    {
        Planet current = player.CurrentPlanet;
        Planet next = planetSpawner.GetNextPlanetAfter(current);

        Vector3 targetPosition;
        float targetSize;

        if (next != null)
        {
            Vector2 a = current.transform.position;
            Vector2 b = next.transform.position;
            float rA = current.OrbitRadius;
            float rB = next.OrbitRadius;

            float minX = Mathf.Min(a.x - rA, b.x - rB);
            float maxX = Mathf.Max(a.x + rA, b.x + rB);
            float minY = Mathf.Min(a.y - rA, b.y - rB);
            float maxY = Mathf.Max(a.y + rA, b.y + rB);

            Vector3 midpoint = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, transform.position.z);
            targetPosition = midpoint;

            float halfWidth = (maxX - minX) * 0.5f;
            float halfHeight = (maxY - minY) * 0.5f;

            float sizeForHeight = halfHeight;
            float sizeForWidth = cam.aspect > 0f ? halfWidth / cam.aspect : halfWidth;

            targetSize = Mathf.Clamp(
                Mathf.Max(sizeForHeight, sizeForWidth) + framingPadding,
                minOrthographicSize,
                maxOrthographicSize);

            lastFramedPosition = targetPosition;
            lastFramedSize = targetSize;
        }
        else
        {
            targetPosition = lastFramedPosition;
            targetSize = lastFramedSize;
        }

        return (targetPosition, targetSize);
    }

    private Vector3 UpdateAimOffset(float dt, Vector3 baseCameraTarget)
    {
        Vector3 targetOffset = Vector3.zero;

        if (player.IsAiming)
        {
            float maxDistance = maxAimOffsetDistance;

            if (player.CurrentPlanet != null)
            {
                float visibilityMax = GetMaxOffsetKeepingPlayerVisible(player.AimDirection, baseCameraTarget);
                maxDistance = Mathf.Min(maxDistance, visibilityMax);
            }

            targetOffset = (Vector3)(player.AimDirection * player.AimPower * maxDistance);
        }

        float smoothTime = player.IsAiming ? aimOffsetSmoothTime : aimOffsetReturnSmoothTime;

        currentAimOffset = Vector3.SmoothDamp(
            currentAimOffset, targetOffset, ref aimOffsetVelocity, smoothTime, Mathf.Infinity, dt);

        return currentAimOffset;
    }

    private float GetMaxOffsetKeepingPlayerVisible(Vector2 direction, Vector3 baseCameraTarget)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return maxAimOffsetDistance;

        float halfHeight = cam.orthographicSize * orbitVisibleAreaFraction;
        float halfWidth = halfHeight * cam.aspect;

        Vector2 relativePlayerPos = (Vector2)player.transform.position - (Vector2)baseCameraTarget;

        float maxT = float.PositiveInfinity;

        if (Mathf.Abs(direction.x) > 0.0001f)
        {
            float axisMaxT = direction.x > 0f
                ? (relativePlayerPos.x + halfWidth) / direction.x
                : (relativePlayerPos.x - halfWidth) / direction.x;

            maxT = Mathf.Min(maxT, axisMaxT);
        }

        if (Mathf.Abs(direction.y) > 0.0001f)
        {
            float axisMaxT = direction.y > 0f
                ? (relativePlayerPos.y + halfHeight) / direction.y
                : (relativePlayerPos.y - halfHeight) / direction.y;

            maxT = Mathf.Min(maxT, axisMaxT);
        }

        return Mathf.Max(0f, maxT);
    }
}