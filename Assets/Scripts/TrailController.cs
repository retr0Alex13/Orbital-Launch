using UnityEngine;

public class TrailController : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer trailRenderer;

    [SerializeField]
    private Rigidbody2D playerRigidBody;

    void Update()
    {
        trailRenderer.time = Mathf.Abs(1f / playerRigidBody.linearVelocity.magnitude);
    }
}
