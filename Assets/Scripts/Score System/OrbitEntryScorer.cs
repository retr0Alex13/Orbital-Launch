using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class OrbitEntryScorer : MonoBehaviour
{
    private PlayerController player;
    private Rigidbody2D rb;

    private OrbitEntryInfo orbitEntryInfo;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        player.OnPlayerCaptured += HandleOrbitCapture;
    }

    private void OnDisable()
    {
        player.OnPlayerCaptured -= HandleOrbitCapture;
    }

    private void HandleOrbitCapture(Planet planet)
    {
        if (planet == null)
            return;

        orbitEntryInfo = new OrbitEntryInfo(rb.linearVelocity, transform.position, planet.transform.position);
        ScoreManager.Instance?.AwardOrbitEntry(orbitEntryInfo);
    }
}
