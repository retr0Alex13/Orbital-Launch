using UnityEngine;

public class Planet : MonoBehaviour
{
    public float GravityStrength => gravityStrength;
    public float OrbitSpeed => orbitSpeed;
    public float OrbitRadius => orbitCollider.radius * orbitCollider.transform.lossyScale.x;

    [SerializeField]
    private float gravityStrength = 10f;

    [SerializeField]
    private float orbitSpeed = 5f;

    [SerializeField]
    private CircleCollider2D orbitCollider;
}