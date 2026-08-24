using UnityEngine;

[System.Serializable]
public struct AimSettings
{
    [Header("Launch Speed")]
    public float minLaunchSpeed;
    public float maxLaunchSpeed;

    [Header("Drag")]
    [Tooltip("Pull distance (world units) along the pull axis required to reach full power/max launch speed.")]
    public float maxDragDistance;

    [Tooltip("Minimum pull distance (world units) required to actually launch on release. Shorter pulls cancel the aim.")]
    public float minDragDistanceToLaunch;

    public float deadzone;

    [Header("Smoothing")]
    public float smoothTime;
    public float powerSharpness;
    public float directionSharpness;

    public static AimSettings Default => new AimSettings
    {
        minLaunchSpeed = 2f,
        maxLaunchSpeed = 6f,
        maxDragDistance = 2.5f,
        minDragDistanceToLaunch = 0.3f,
        deadzone = 0.08f,
        smoothTime = 0.03f,
        powerSharpness = 20f,
        directionSharpness = 6f
    };
}

public readonly struct AimReleaseResult
{
    public readonly bool ShouldLaunch;
    public readonly Vector2 Direction;
    public readonly float Power;

    public AimReleaseResult(bool shouldLaunch, Vector2 direction, float power)
    {
        ShouldLaunch = shouldLaunch;
        Direction = direction;
        Power = power;
    }
}

public class PlayerAimHandler
{
    public bool IsAiming { get; private set; }
    public Vector2 AimDirection { get; private set; }
    public float AimPower { get; private set; }

    private readonly AimSettings settings;
    private readonly Camera aimCamera;
    private readonly Transform playerTransform;

    private Vector2 dragStartWorldPos;
    private Vector2 smoothedPointer;
    private Vector2 pointerVelocity;

    private Vector3 cameraPositionSnapshot;
    private float cameraOrthoSizeSnapshot;
    private float cameraAspectSnapshot;

    public PlayerAimHandler(AimSettings settings, Camera aimCamera, Transform playerTransform)
    {
        this.settings = settings;
        this.aimCamera = aimCamera;
        this.playerTransform = playerTransform;
    }

    public void BeginAim()
    {
        IsAiming = true;

        cameraPositionSnapshot = aimCamera.transform.position;
        cameraOrthoSizeSnapshot = aimCamera.orthographicSize;
        cameraAspectSnapshot = aimCamera.aspect;

        dragStartWorldPos = GetPointerWorldPosition();

        smoothedPointer = dragStartWorldPos;
        pointerVelocity = Vector2.zero;

        AimDirection = playerTransform.up;
        AimPower = 0f;
    }

    public void UpdateAim(float unscaledDeltaTime)
    {
        Vector2 rawPointer = GetPointerWorldPosition();

        smoothedPointer = Vector2.SmoothDamp(
            smoothedPointer,
            rawPointer,
            ref pointerVelocity,
            settings.smoothTime,
            Mathf.Infinity,
            unscaledDeltaTime);

        Vector2 dragVector = smoothedPointer - dragStartWorldPos;
        float dragDistance = dragVector.magnitude;

        float actualMaxDistance = settings.maxDragDistance + settings.deadzone;

        if (dragDistance > actualMaxDistance)
        {
            Vector2 direction = dragVector / dragDistance;
            dragStartWorldPos = smoothedPointer - direction * actualMaxDistance;

            dragVector = smoothedPointer - dragStartWorldPos;
            dragDistance = actualMaxDistance;
        }

        Vector2 targetDirection = dragDistance > 0.0001f
            ? dragVector / dragDistance
            : AimDirection;

        float effectiveDistance = Mathf.Max(0f, dragDistance - settings.deadzone);
        float targetPower = Mathf.Clamp01(effectiveDistance / settings.maxDragDistance);
        targetPower *= targetPower;

        float tPower = 1f - Mathf.Exp(-settings.powerSharpness * unscaledDeltaTime);
        float tDir = 1f - Mathf.Exp(-settings.directionSharpness * unscaledDeltaTime);

        AimDirection = Vector2.Lerp(AimDirection, targetDirection, tDir).normalized;
        AimPower = Mathf.Lerp(AimPower, targetPower, tPower);
    }

    public AimReleaseResult EndAim()
    {
        IsAiming = false;

        Vector2 dragVector = GetPointerWorldPosition() - dragStartWorldPos;
        float dragDistance = dragVector.magnitude;

        bool shouldLaunch = dragDistance >= settings.minDragDistanceToLaunch;
        return new AimReleaseResult(shouldLaunch, AimDirection, AimPower);
    }

    public void CancelPower()
    {
        AimPower = 0f;
    }

    private Vector2 GetPointerWorldPosition()
    {
        Vector3 viewportPos = aimCamera.ScreenToViewportPoint(Input.mousePosition);

        float halfHeight = cameraOrthoSizeSnapshot;
        float halfWidth = halfHeight * cameraAspectSnapshot;

        return new Vector2(
            cameraPositionSnapshot.x + (viewportPos.x - 0.5f) * halfWidth * 2f,
            cameraPositionSnapshot.y + (viewportPos.y - 0.5f) * halfHeight * 2f);
    }
}