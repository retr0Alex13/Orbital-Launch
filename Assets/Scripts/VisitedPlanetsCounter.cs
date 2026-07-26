using UnityEngine;

public class VisitedPlanetsCounter : MonoBehaviour
{
    public int VisitedPlanets { get; private set; }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Planet planet))
        {
            VisitedPlanets++;
        }
    }
}
