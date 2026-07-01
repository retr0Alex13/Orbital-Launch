using System.Collections.Generic;
using UnityEngine;

public class AsteroidRing
{
    public Planet Planet { get; }
    public IReadOnlyList<Asteroid> Asteroids { get; }

    public float InitialGapCenterDeg { get; }
    public float GapDegrees { get; }
    public float AngularSpeedDeg { get; }

    private readonly float spawnTime;

    public float CurrentGapCenterDeg =>
        InitialGapCenterDeg + AngularSpeedDeg * (Time.time - spawnTime);

    public float RingRadius { get; }

    public AsteroidRing(
        Planet planet,
        IReadOnlyList<Asteroid> asteroids,
        float initialGapCenterDeg,
        float gapDegrees,
        float angularSpeedDeg,
        float ringRadius)          
    {
        Planet = planet;
        Asteroids = asteroids;
        InitialGapCenterDeg = initialGapCenterDeg;
        GapDegrees = gapDegrees;
        AngularSpeedDeg = angularSpeedDeg;
        RingRadius = ringRadius;
        spawnTime = Time.time;
    }
}