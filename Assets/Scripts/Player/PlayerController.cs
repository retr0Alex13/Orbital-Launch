using AudioSystem;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerEffectsFeedback))]
public class PlayerController : MonoBehaviour
{
    public event Action OnPlayerLaunched;
    public event Action<Planet> OnPlayerCaptured;
    public event Action<Planet> OnPlanetLeft;
    public event Action OnPlayerDestroyed;

    public OrbitFlightController OrbitFlightController => orbitFlight;
    public Func<Vector2, bool> LaunchValidator { get; set; }

    public Planet CurrentPlanet { get; private set; }
    public bool CanLaunch { get; set; } = true;
    public bool IsAiming => aimHandler.IsAiming;
    public bool IsTransitioning => orbitFlight.IsTransitioning;
    public Vector2 AimDirection => aimHandler.AimDirection;
    public float AimPower => aimHandler.AimPower;

    [SerializeField]
    private float introSpeed = 5f;

    [Header("Aiming")]
    [SerializeField] private AimSettings aimSettings = AimSettings.Default;

    [Header("Orbit")]
    [SerializeField] private OrbitTransitionSettings orbitSettings = OrbitTransitionSettings.Default;

    [Header("Destruction")]
    [SerializeField] private RocketParts rocketParts;
    [SerializeField] private GameObject rocketExplosion;

    [SerializeField] private SpriteRenderer rocketSprite;
    [SerializeField] private TrailRenderer[] rocketTrails;

    [Header("Audio")]
    [SerializeField] private SoundData rocketThrustSound;
    [SerializeField] private SoundData rocketLaunchSound;
    [SerializeField] private SoundData rocketExplosionSound;
    [SerializeField] private ParticleSystem rocketThrust;

    private Rigidbody2D playerRigidBody;
    private Camera mainCamera;

    private PlayerAimHandler aimHandler;
    private OrbitFlightController orbitFlight;
    private PlayerEffectsFeedback feedback;

    private void Awake()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        feedback = GetComponent<PlayerEffectsFeedback>();
    }

    private void Start()
    {
        mainCamera = Camera.main;

        aimHandler = new PlayerAimHandler(aimSettings, mainCamera, transform);
        orbitFlight = new OrbitFlightController(orbitSettings);

        feedback.Initialize(rocketThrust, rocketExplosion, rocketTrails, rocketSprite,
            rocketThrustSound, rocketLaunchSound, rocketExplosionSound);

        OnPlayerLaunched += feedback.HandleLaunched;
        OnPlayerCaptured += feedback.HandleCaptured;

        playerRigidBody.linearVelocity = Vector2.right * introSpeed;
        transform.up = playerRigidBody.linearVelocity.normalized;
        OnPlayerLaunched?.Invoke();
    }

    private void OnDestroy()
    {
        OnPlayerLaunched -= feedback.HandleLaunched;
        OnPlayerCaptured -= feedback.HandleCaptured;
    }

    private void Update()
    {
        if (!CanLaunch)
            return;

        if (aimHandler.IsAiming)
        {
            if (Input.GetMouseButton(0))
                aimHandler.UpdateAim(Time.unscaledDeltaTime);
            else
                ResolveAimRelease();
        }
        else if (Input.GetMouseButtonDown(0) && !orbitFlight.IsTransitioning)
        {
            aimHandler.BeginAim();
            playerRigidBody.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (CurrentPlanet == null || aimHandler.IsAiming)
            return;

        Vector2 targetVelocity = orbitFlight.CalculateOrbitVelocity(transform.position, CurrentPlanet);

        playerRigidBody.linearVelocity = orbitFlight.IsTransitioning
            ? orbitFlight.StepTransition(Time.fixedDeltaTime, targetVelocity)
            : targetVelocity;

        transform.up = playerRigidBody.linearVelocity.normalized;
    }

    private void ResolveAimRelease()
    {
        AimReleaseResult release = aimHandler.EndAim();

        if (!release.ShouldLaunch)
        {
            if (orbitFlight.IsTransitioning)
                orbitFlight.RestartFromZeroVelocity();
            return;
        }

        if (LaunchValidator != null && !LaunchValidator(release.Direction))
        {
            aimHandler.CancelPower();
            return;
        }

        float launchSpeed = Mathf.Lerp(aimSettings.minLaunchSpeed, aimSettings.maxLaunchSpeed, release.Power);
        launchSpeed *= orbitFlight.SpeedMultiplier;

        LaunchFromOrbit(release.Direction, launchSpeed);
    }

    private void LaunchFromOrbit(Vector2 direction, float speed)
    {
        Planet previousPlanet = CurrentPlanet;

        if (previousPlanet != null)
        {
            previousPlanet.PlayShockWaveEffect(transform.position);
            CurrentPlanet = null;
            orbitFlight.ResetTransition();
        }

        Vector2 launchDirection = direction.normalized;
        playerRigidBody.linearVelocity = launchDirection * speed;
        transform.up = launchDirection;

        OnPlayerLaunched?.Invoke();

        if (previousPlanet != null)
            OnPlanetLeft?.Invoke(previousPlanet);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CurrentPlanet != null || !collision.TryGetComponent(out Planet planet))
            return;

        CurrentPlanet = planet;
        orbitFlight.BeginTransition(playerRigidBody.linearVelocity, transform.position, planet);

        OnPlayerCaptured?.Invoke(planet);
        planet.PlayShockWaveEffect(transform.position);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CurrentPlanet == null || collision.gameObject != CurrentPlanet.gameObject || !orbitFlight.IsTransitioning)
            return;

        Planet previousPlanet = CurrentPlanet;
        CurrentPlanet = null;
        orbitFlight.ResetTransition();
        OnPlayerLaunched?.Invoke();
        OnPlanetLeft?.Invoke(previousPlanet);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool hitAsteroid = collision.gameObject.TryGetComponent(out Asteroid _);
        bool hitPlanet = collision.transform.parent != null && collision.transform.parent.TryGetComponent(out Planet _);

        if (!hitAsteroid && !hitPlanet)
            return;

        float duration = rocketExplosion.GetComponent<ParticleSystem>().main.duration;
        GameManager.Instance.RestartGameWithDelay(duration);
        feedback.HandleCrashEffects();
        rocketParts.SpawnParts(transform.position);

        OnPlayerDestroyed?.Invoke();
    }
}