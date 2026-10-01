using UnityEngine;

public static class OrbitRayUtility
{
    public static bool TryFindNearestOrbitHit(
    Vector2 origin,
    Vector2 direction,
    float rayLength,
    Planet ignorePlanet,
    LayerMask planetLayerMask,
    Collider2D[] overlapBuffer,
    out float hitDistance,
    float searchPadding = 0f)
    {
        hitDistance = rayLength;
        bool found = false;

        ContactFilter2D filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = planetLayerMask
        };

        int count = Physics2D.OverlapCircle(origin, rayLength + searchPadding, filter, overlapBuffer);

        for (int i = 0; i < count; i++)
        {
            if (!overlapBuffer[i].TryGetComponent(out Planet planet))
                planet = overlapBuffer[i].GetComponentInParent<Planet>();

            if (planet == null || planet == ignorePlanet)
                continue;

            if (RayIntersectsCircle(origin, direction, planet.transform.position, planet.OrbitRadius, out float distance)
                && distance < hitDistance)
            {
                hitDistance = distance;
                found = true;
            }
        }

        return found;
    }

    public static bool RayIntersectsCircle(Vector2 origin, Vector2 direction, Vector2 center, float radius, out float distance)
    {
        Vector2 oc = origin - center;
        float b = Vector2.Dot(oc, direction);
        float c = oc.sqrMagnitude - radius * radius;
        float disc = b * b - c;

        distance = 0f;
        if (disc < 0f)
            return false;

        float s = Mathf.Sqrt(disc);
        float t = -b - s;          
        if (t < 0f)
            t = -b + s;

        if (t < 0f)
            return false;

        distance = t;
        return true;
    }
}