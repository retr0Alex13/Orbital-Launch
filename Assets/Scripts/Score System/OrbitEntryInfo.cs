using UnityEngine;

public struct OrbitEntryInfo
{
    public OrbitEntryInfo(Vector2 capturedVelocity, Vector2 capturedPlayerPos, Vector2 capturedPlanetPos)
    {
        CapturedVelocity = capturedVelocity;
        CapturedPlayerPos = capturedPlayerPos;
        CapturedPlanetPos = capturedPlanetPos;
    }

    public Vector2 CapturedVelocity { get; private set; }
    public Vector2 CapturedPlayerPos { get; private set; }
    public Vector2 CapturedPlanetPos { get; private set; }

}

public enum OrbitEntryType
{
    Good,
    Perfect,
    NearMiss
}