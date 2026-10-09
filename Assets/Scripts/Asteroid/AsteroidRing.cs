using System.Collections.Generic;
using UnityEngine;

public class AsteroidRing
{
    public Planet Planet { get; }

    private readonly List<Asteroid> asteroids;
    public IReadOnlyList<Asteroid> Asteroids => asteroids;
    public bool IsEmpty => asteroids.Count == 0;

    public float InitialGapCenterDeg { get; }
    public float GapDegrees { get; }
    public float AngularSpeedDeg { get; }
    public int GapCount { get; }

    private readonly float spawnTime;

    // Center of the first gap. The others lie at +k * (360 / GapCount).
    public float CurrentGapCenterDeg =>
        InitialGapCenterDeg + AngularSpeedDeg * (Time.time - spawnTime);

    public float RingRadius { get; }

    public AsteroidRing(
        Planet planet,
        IReadOnlyList<Asteroid> asteroids,
        float initialGapCenterDeg,
        float gapDegrees,
        float angularSpeedDeg,
        float ringRadius,
        int gapCount = 1)
    {
        Planet = planet;
        this.asteroids = new List<Asteroid>(asteroids);
        InitialGapCenterDeg = initialGapCenterDeg;
        GapDegrees = gapDegrees;
        AngularSpeedDeg = angularSpeedDeg;
        RingRadius = ringRadius;
        GapCount = Mathf.Max(1, gapCount);
        spawnTime = Time.time;
    }

    public float GetGapCenterDeg(int index) =>
        CurrentGapCenterDeg + index * (360f / GapCount);

    public bool IsAngleInGap(float angleDeg)
    {
        float sector = 360f / GapCount;
        float rel = Mathf.Repeat(angleDeg - CurrentGapCenterDeg + sector * 0.5f, sector)
                    - sector * 0.5f;
        return Mathf.Abs(rel) <= GapDegrees * 0.5f;
    }

    public float NearestGapCenterDeg(float angleDeg)
    {
        float sector = 360f / GapCount;
        float rel = Mathf.Repeat(angleDeg - CurrentGapCenterDeg + sector * 0.5f, sector)
                    - sector * 0.5f;
        return angleDeg - rel;
    }

    public void RemoveAsteroid(Asteroid a) => asteroids.Remove(a);
}