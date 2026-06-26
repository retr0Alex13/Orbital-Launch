using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Planet planet;
    private float angle;
    private float orbitRadius;
    private float angularSpeed;

    public void Configure(Planet targetPlanet, float startAngle, float speed, float scale, float radius)
    {
        planet = targetPlanet;
        angle = startAngle;
        orbitRadius = radius;
        angularSpeed = speed;
        transform.localScale = Vector3.one * scale;
    }

    private void Update()
    {
        if (planet == null) return;

        angle += angularSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        transform.position = (Vector2)planet.transform.position
            + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
    }
}