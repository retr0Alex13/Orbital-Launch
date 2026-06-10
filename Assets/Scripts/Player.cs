using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private float orbitTransitionDuration = 0.4f;

    private Planet currentPlanet;

    private bool isTransitioning;
    private float transitionElapsed;
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
    }

    private void FixedUpdate()
    {
        if (currentPlanet == null)
            return;

        Vector2 directionToPlanet = currentPlanet.transform.position - transform.position;
        Vector2 normalizedDirection = directionToPlanet.normalized;
        playerRigidBody.AddForce(normalizedDirection * currentPlanet.GravityStrength, ForceMode2D.Force);

        if (isTransitioning)
        {
            transitionElapsed += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(transitionElapsed / orbitTransitionDuration);
            float smoothT = 1f - Mathf.Pow(1f - t, 1.5f);

            Vector2 tangent = new Vector2(-normalizedDirection.y, normalizedDirection.x);
            Vector2 targetVelocity = tangent * currentPlanet.OrbitSpeed;

            playerRigidBody.linearVelocity = Vector2.Lerp(velocityAtEntry, targetVelocity, smoothT);

            if (t >= 1f)
                isTransitioning = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Planet planet))
        {
            currentPlanet = planet;

            velocityAtEntry = playerRigidBody.linearVelocity;
            transitionElapsed = 0f;
            isTransitioning = true;
        }
    }
}