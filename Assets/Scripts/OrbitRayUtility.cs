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
        out float hitDistance)
    {
        hitDistance = rayLength;
        bool found = false;

        int count = Physics2D.OverlapCircleNonAlloc(origin, rayLength, overlapBuffer, planetLayerMask);

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
        Vector2 toCenter = center - origin;
        float tClosest = Vector2.Dot(toCenter, direction);

        if (tClosest < 0f)
        {
            distance = 0f;
            return false;
        }

        Vector2 closestPoint = origin + direction * tClosest;
        float distToCenterSqr = (closestPoint - center).sqrMagnitude;
        float radiusSqr = radius * radius;

        if (distToCenterSqr > radiusSqr)
        {
            distance = 0f;
            return false;
        }

        float halfChord = Mathf.Sqrt(radiusSqr - distToCenterSqr);
        distance = tClosest - halfChord;

        if (distance < 0f)
            distance = tClosest + halfChord;

        return distance >= 0f;
    }
}