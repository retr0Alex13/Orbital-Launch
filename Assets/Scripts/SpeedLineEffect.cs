using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SpeedLinesEffect : MonoBehaviour
{
    [SerializeField] private Rigidbody2D target;
    [SerializeField] private PlayerController player;

    [Header("Scaling")]
    [SerializeField] private float ratePerSpeed = 3f;
    [SerializeField] private float particleSpeedMultiplier = 1.5f;

    [Header("Spawn position")]
    [SerializeField] private float spawnDistance = 8f;

    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.MainModule main;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        emission = ps.emission;
        main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
    }

    private void LateUpdate()
    {
        Vector2 vel = target.linearVelocity;
        float speed = vel.magnitude;

        bool active = player.CurrentPlanet == null && speed > 0.01f;
        emission.enabled = active;
        if (!active) return;

        emission.rateOverTime = speed * ratePerSpeed;
        main.startSpeed = speed * particleSpeedMultiplier;

        Vector2 dir = vel / speed;

        Vector2 spawnPos = target.position + dir * spawnDistance;
        transform.position = new Vector3(spawnPos.x, spawnPos.y, transform.position.z);

        Vector3 back = new Vector3(-dir.x, -dir.y, 0f);
        transform.rotation = Quaternion.LookRotation(back, Vector3.forward);
    }
}