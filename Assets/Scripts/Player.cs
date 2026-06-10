using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D playerRigidBody;

    private Planet currentPlanet;

    private void FixedUpdate()
    {
        if (currentPlanet == null)
            return;

        Vector2 directionToPlanet = currentPlanet.transform.position - transform.position;
        Vector2 normalizedDirection = directionToPlanet.normalized;

        playerRigidBody.AddForce(normalizedDirection * currentPlanet.GravityStrength, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Planet planet))
        {
            currentPlanet = planet;

            Vector2 normalizedDirection = (currentPlanet.transform.position - transform.position).normalized;
            Vector2 tangent = new Vector2(-normalizedDirection.y, normalizedDirection.x);

            playerRigidBody.linearVelocity = tangent * currentPlanet.OrbitSpeed;
        }
    }
}
