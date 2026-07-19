using AudioSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public event Action OnPlayerLaunched;
    public event Action<Planet> OnPlayerCaptured;

    public Planet CurrentPlanet => currentPlanet;

    [SerializeField]
    private float launchSpeed = 3f;

    [SerializeField]
    private float orbitTransitionDuration = 1f;

    [SerializeField]
    private float radiusCorrectionSpeed = 5f;

    [SerializeField]
    private ParticleSystem rocketThrust;

    [SerializeField]
    private Rigidbody2D playerRigidBody;

    [SerializeField]
    private SoundData rocketThrustSound;

    private float transitionElapsed;
    private float orbitDirection;

    private bool isTransitioning;

    private Planet currentPlanet;
    private Vector2 velocityAtEntry;

    private SoundBuilder soundBuilder;
    private SoundEmitter engineSound;

    private void Start()
    {
        soundBuilder = SoundManager.Instance.CreateSoundBuilder();

        playerRigidBody.linearVelocity = Vector2.right * launchSpeed;
        transform.up = playerRigidBody.linearVelocity.normalized;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LaunchFromOrbit();
        }
    }

    private void LaunchFromOrbit()
    {
        if (currentPlanet == null)
            return;

        currentPlanet = null;
        isTransitioning = false;

        playerRigidBody.AddForce(playerRigidBody.linearVelocity.normalized * launchSpeed, ForceMode2D.Impulse);

        OnPlayerLaunched?.Invoke();
        ToggleEffects(true);
    }

    private void FixedUpdate()
    {
        if (currentPlanet == null)
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
            ToggleEffects(false);
        }
        if (collision.TryGetComponent(out Asteroid asteroid))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void ToggleEffects(bool enable)
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
                ToggleEffects(true);
            }
        }
    }
}