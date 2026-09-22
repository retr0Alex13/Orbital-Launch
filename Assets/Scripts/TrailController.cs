using UnityEngine;

public class TrailController : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer trailRenderer;
    private Rigidbody2D playerRigidBody;

    public void Initialize(Rigidbody2D rigidBody)
    {
        playerRigidBody = rigidBody;
    }

    void Update()
    {
        trailRenderer.time = Mathf.Abs(1f / playerRigidBody.linearVelocity.magnitude);
    }
}
