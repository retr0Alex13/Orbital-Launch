using UnityEngine;

[RequireComponent(typeof(Player))]
public class OrbitEntryScorer : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;

    private OrbitEntryInfo orbitEntryInfo;

    private void Awake()
    {
        player = GetComponent<Player>();
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

    private void HandleOrbitCapture()
    {
        Planet planet = player.CurrentPlanet;

        if (planet == null) return;

        orbitEntryInfo = new OrbitEntryInfo(rb.linearVelocity, transform.position, planet.transform.position);
        ScoreManager.Instance?.AwardOrbitEntry(orbitEntryInfo);
    }
}
