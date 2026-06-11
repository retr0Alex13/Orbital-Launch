using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float launchSpeed = 3f;

    [SerializeField]
    private float orbitTransitionDuration = 1f;

    [SerializeField]
    private float radiusCorrectionSpeed = 5f;

    [SerializeField] private Rigidbody2D playerRigidBody;

    private float transitionElapsed;
    private float orbitDirection;

    private bool isTransitioning;

    private Planet currentPlanet;
    private Vector2 velocityAtEntry;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            LaunchFromOrbit();
    }

    private void LaunchFromOrbit()
    {
        if (currentPlanet == null)
            return;

        currentPlanet = null;
        isTransitioning = false;

        playerRigidBody.AddForce(playerRigidBody.linearVelocity.normalized * launchSpeed, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (currentPlanet == null)
            return;

        Vector2 toPlanet = currentPlanet.transform.position - transform.position;
        float distanceToPlanet = toPlanet.magnitude;
        Vector2 directionToPlanet = toPlanet / distanceToPlanet;

        Vector2 tangent = new Vector2(-directionToPlanet.y, directionToPlanet.x) * orbitDirection;
        Vector2 targetVelocity = tangent * currentPlanet.OrbitSpeed;

        if (isTransitioning)
        {
            transitionElapsed += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(transitionElapsed / orbitTransitionDuration);
            float smoothT = 1f - Mathf.Pow(1f - t, 1.5f);

            playerRigidBody.linearVelocity = Vector2.Lerp(velocityAtEntry, targetVelocity, smoothT);

            if (t >= 1f)
                isTransitioning = false;
        }
        else
        {
            float radiusError = distanceToPlanet - currentPlanet.OrbitRadius;
            Vector2 correctionVelocity = directionToPlanet * (radiusError * radiusCorrectionSpeed);

            playerRigidBody.linearVelocity = targetVelocity + correctionVelocity;
        }
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
        }
    }
}