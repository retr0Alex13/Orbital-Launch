using AudioSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public event Action OnPlayerLaunched;
    public event Action<Planet> OnPlayerCaptured;

    public Planet CurrentPlanet => currentPlanet;
    public bool CanLaunch { get; set; } = true;
    public bool IsAiming { get; set; }
    public Vector2 AimDirection { get; private set; }
    public float AimPower { get; private set; }

    [SerializeField]
    private float introSpeed = 5f;

    [SerializeField]
    private float orbitTransitionDuration = 1f;

    [SerializeField]
    private float radiusCorrectionSpeed = 5f;

    [Header("Slingshot Aiming")]
    [SerializeField]
    private float minLaunchSpeed = 2f;

    [SerializeField]
    private float maxLaunchSpeed = 6f;

    [Tooltip("Drag distance (world units) required to reach full power/max launch speed.")]
    [SerializeField]
    private float maxDragDistance = 2.5f;

    [Tooltip("Minimum drag distance (world units) required to actually launch on release. Shorter drags cancel the aim.")]
    [SerializeField]
    private float minDragDistanceToLaunch = 0.3f;

    [Header("Aim Smoothing")]
    [SerializeField]
    private float aimSmoothTime = 0.03f;

    [SerializeField]
    private float aimPowerSharpness = 20f;

    [SerializeField]
    private ParticleSystem rocketThrust;

    [SerializeField]
    private Rigidbody2D playerRigidBody;

    [SerializeField]
    private SoundData rocketThrustSound;

    [SerializeField]
    private SoundData rocketLaunchSound;

    private float transitionElapsed;
    private float orbitDirection;

    private bool isTransitioning;

    private Planet currentPlanet;
    private Vector2 velocityAtEntry;

    private SoundBuilder soundBuilder;
    private SoundEmitter engineSound;

    private Camera mainCamera;
    private Vector2 dragStartWorldPos;
    private Vector2 smoothedPointer;
    private Vector2 pointerVelocity;

    private void Start()
    {
        mainCamera = Camera.main;

        soundBuilder = SoundManager.Instance.CreateSoundBuilder();
        soundBuilder = soundBuilder.WithRandomPitch();

        playerRigidBody.linearVelocity = Vector2.right * introSpeed;
        transform.up = playerRigidBody.linearVelocity.normalized;
        ToggleThrustEffects(true);
    }

    private void Update()
    {
        if (currentPlanet == null || !CanLaunch)
            return;

        if (IsAiming)
        {
            if (Input.GetMouseButton(0))
            {
                UpdateAim();
            }
            else
            {
                // Covers both a normal mouse-up and any missed GetMouseButtonUp frame.
                EndAim();
            }
        }
        else if (Input.GetMouseButtonDown(0) && !isTransitioning)
        {
            // Only allow aiming once the player has settled into a stable orbit,
            // so we don't have to fight an in-progress orbit-entry transition.
            BeginAim();
        }
    }

    private Vector2 GetPointerWorldPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPos);
    }

    private void BeginAim()
    {
        IsAiming = true;
        dragStartWorldPos = GetPointerWorldPosition();

        smoothedPointer = dragStartWorldPos;
        pointerVelocity = Vector2.zero;

        AimDirection = transform.up;
        AimPower = 0f;

        playerRigidBody.linearVelocity = Vector2.zero;
    }

    private void UpdateAim()
    {
        // Smooth the pointer itself.
        Vector2 rawPointer = GetPointerWorldPosition();

        smoothedPointer = Vector2.SmoothDamp(
            smoothedPointer,
            rawPointer,
            ref pointerVelocity,
            aimSmoothTime,
            Mathf.Infinity,
            Time.unscaledDeltaTime);

        Vector2 dragVector = smoothedPointer - dragStartWorldPos;
        float dragDistance = dragVector.magnitude;

        // Smooth direction.
        if (dragDistance > 0.001f)
        {
            Vector2 targetDirection = (-dragVector).normalized;

            float t = 1f - Mathf.Exp(-aimPowerSharpness * Time.unscaledDeltaTime);
            AimDirection = Vector2.Lerp(AimDirection, targetDirection, t).normalized;
        }

        // Smooth power.
        float targetPower = Mathf.Clamp01(dragDistance / maxDragDistance);

        float powerT = 1f - Mathf.Exp(-aimPowerSharpness * Time.unscaledDeltaTime);
        AimPower = Mathf.Lerp(AimPower, targetPower, powerT);
    }

    private void EndAim()
    {
        IsAiming = false;

        Vector2 currentPointerWorldPos = GetPointerWorldPosition();
        float dragDistance = (currentPointerWorldPos - dragStartWorldPos).magnitude;

        if (dragDistance >= minDragDistanceToLaunch)
        {
            float launchSpeed = Mathf.Lerp(minLaunchSpeed, maxLaunchSpeed, AimPower);
            LaunchFromOrbit(AimDirection, launchSpeed);
        }
        else if (isTransitioning)
        {
            // Aim was cancelled mid-transition; restart the transition cleanly from the
            // zero velocity we froze at, instead of snapping from the stale pre-aim velocity.
            velocityAtEntry = Vector2.zero;
            transitionElapsed = 0f;
        }
        // Otherwise the aim is simply cancelled and orbit motion resumes next FixedUpdate.
    }

    private void LaunchFromOrbit(Vector2 direction, float speed)
    {
        if (currentPlanet == null)
            return;

        currentPlanet.PlayShockWaveEffect(transform.position);

        currentPlanet = null;
        isTransitioning = false;

        Vector2 launchDirection = direction.normalized;
        playerRigidBody.linearVelocity = launchDirection * speed;
        transform.up = launchDirection;

        OnPlayerLaunched?.Invoke();

        soundBuilder.Play(rocketLaunchSound);
        ToggleThrustEffects(true);
    }

    private void FixedUpdate()
    {
        if (currentPlanet == null || IsAiming)
            return;

        Vector2 targetVelocity = CalculateOrbitVelocity();

        if (isTransitioning)
        {
            ApplyTransition(targetVelocity);
        }
        else
        {
            playerRigidBody.linearVelocity = targetVelocity;
        }

        transform.up = playerRigidBody.linearVelocity.normalized;
    }

    private Vector2 CalculateOrbitVelocity()
    {
        Vector2 toPlanet = currentPlanet.transform.position - transform.position;
        float distanceToPlanet = toPlanet.magnitude;
        Vector2 directionToPlanet = toPlanet / distanceToPlanet;

        Vector2 tangent = new Vector2(-directionToPlanet.y, directionToPlanet.x) * orbitDirection;
        Vector2 tangentVelocity = tangent * currentPlanet.OrbitSpeed;

        float radiusError = distanceToPlanet - currentPlanet.OrbitRadius;
        Vector2 radialCorrection = directionToPlanet * (radiusError * radiusCorrectionSpeed);

        return tangentVelocity + radialCorrection;
    }

    private void ApplyTransition(Vector2 targetVelocity)
    {
        transitionElapsed += Time.fixedDeltaTime;

        float t = Mathf.Clamp01(transitionElapsed / orbitTransitionDuration);
        float smoothT = 1f - Mathf.Pow(1f - t, 1.5f);

        playerRigidBody.linearVelocity = Vector2.Lerp(velocityAtEntry, targetVelocity, smoothT);

        if (t >= 1f)
            isTransitioning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Planet planet))
        {
            if (currentPlanet != null) return;

            currentPlanet = planet;
            velocityAtEntry = playerRigidBody.linearVelocity;
            transitionElapsed = 0f;
            isTransitioning = true;

            Vector2 toPlanet = currentPlanet.transform.position - transform.position;
            float crossZ = velocityAtEntry.x * toPlanet.y - velocityAtEntry.y * toPlanet.x;
            orbitDirection = crossZ > 0 ? -1f : 1f;

            OnPlayerCaptured?.Invoke(planet);
            ToggleThrustEffects(false);

            soundBuilder.Play(rocketLaunchSound);
            currentPlanet.PlayShockWaveEffect(transform.position);
        }
        if (collision.TryGetComponent(out Asteroid asteroid))
        {
            GameManager.Instance.RestartGame();
        }
    }

    private void ToggleThrustEffects(bool enable)
    {
        if (enable)
        {
            engineSound = soundBuilder.Play(rocketThrustSound);
            rocketThrust.Play(true);
        }
        else
        {
            engineSound?.Stop();
            rocketThrust.Stop(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentPlanet != null && collision.gameObject == currentPlanet.gameObject)
        {
            if (isTransitioning)
            {
                currentPlanet = null;
                isTransitioning = false;

                OnPlayerLaunched?.Invoke();
                ToggleThrustEffects(true);
            }
        }
    }
}