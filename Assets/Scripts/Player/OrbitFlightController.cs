using UnityEngine;

[System.Serializable]
public struct OrbitTransitionSettings
{
    public float transitionDuration;
    public float radiusCorrectionSpeed;

    public static OrbitTransitionSettings Default => new OrbitTransitionSettings
    {
        transitionDuration = 1f,
        radiusCorrectionSpeed = 5f
    };
}

public class OrbitFlightController
{
    public bool IsTransitioning { get; private set; }

    private readonly OrbitTransitionSettings settings;

    private float transitionElapsed;
    private float orbitDirection;
    private Vector2 velocityAtEntry;

    public OrbitFlightController(OrbitTransitionSettings settings)
    {
        this.settings = settings;
    }

    public void BeginTransition(Vector2 entryVelocity, Vector2 playerPosition, Planet planet)
    {
        velocityAtEntry = entryVelocity;
        transitionElapsed = 0f;
        IsTransitioning = true;

        Vector2 toPlanet = (Vector2)planet.transform.position - playerPosition;
        float crossZ = entryVelocity.x * toPlanet.y - entryVelocity.y * toPlanet.x;
        orbitDirection = crossZ > 0 ? -1f : 1f;
    }

    public void RestartFromZeroVelocity()
    {
        velocityAtEntry = Vector2.zero;
        transitionElapsed = 0f;
    }

    public void ResetTransition()
    {
        IsTransitioning = false;
        transitionElapsed = 0f;
        velocityAtEntry = Vector2.zero;
    }

    public Vector2 CalculateOrbitVelocity(Vector2 playerPosition, Planet planet)
    {
        Vector2 toPlanet = (Vector2)planet.transform.position - playerPosition;
        float distanceToPlanet = toPlanet.magnitude;
        Vector2 directionToPlanet = toPlanet / distanceToPlanet;

        Vector2 tangent = new Vector2(-directionToPlanet.y, directionToPlanet.x) * orbitDirection;
        Vector2 tangentVelocity = tangent * planet.OrbitSpeed;

        float radiusError = distanceToPlanet - planet.OrbitRadius;
        Vector2 radialCorrection = directionToPlanet * (radiusError * settings.radiusCorrectionSpeed);

        return tangentVelocity + radialCorrection;
    }

    public Vector2 StepTransition(float fixedDeltaTime, Vector2 targetVelocity)
    {
        transitionElapsed += fixedDeltaTime;

        float t = Mathf.Clamp01(transitionElapsed / settings.transitionDuration);
        float smoothT = 1f - Mathf.Pow(1f - t, 1.5f);

        if (t >= 1f)
            IsTransitioning = false;

        return Vector2.Lerp(velocityAtEntry, targetVelocity, smoothT);
    }
}