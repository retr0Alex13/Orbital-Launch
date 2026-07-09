using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int, OrbitEntryType> OnScoreScoreChanged;

    public int TotalScore { get; private set; }

    [SerializeField]
    private OrbitEntryConfig orbitEntryConfig;

    private bool firstEntry = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        TotalScore = 0;
    }

    public void AwardOrbitEntry(OrbitEntryInfo orbitEntryInfo)
    {
        if (firstEntry)
        {
            firstEntry = false;
            return;
        }

        OrbitEntryType entryType = Evaluate(orbitEntryInfo, orbitEntryConfig);

        int points = GetPoints(entryType, orbitEntryConfig);

        TotalScore += points;

        OnScoreScoreChanged?.Invoke(TotalScore, entryType);
    }

    public OrbitEntryType Evaluate(OrbitEntryInfo orbitEntryInfo, OrbitEntryConfig orbitEntryConfig)
    {
        Vector2 directionToPlanet = (orbitEntryInfo.CapturedPlanetPos - orbitEntryInfo.CapturedPlayerPos).normalized;
        float dot = Vector2.Dot(orbitEntryInfo.CapturedVelocity.normalized, directionToPlanet);

        if (dot >= orbitEntryConfig.GoodThreshold)
            return OrbitEntryType.Good;

        if (dot <= orbitEntryConfig.NearMissThreshold)
            return OrbitEntryType.NearMiss;

        return OrbitEntryType.Perfect;
    }

    public int GetPoints(OrbitEntryType entryType, OrbitEntryConfig orbitEntryConfig) => entryType switch
    {
        OrbitEntryType.Good => orbitEntryConfig.GoodPoints,
        OrbitEntryType.Perfect => orbitEntryConfig.PerfectPoints,
        OrbitEntryType.NearMiss => orbitEntryConfig.NearMissPoints,
        _ => 0
    };

    public string GetLabel(OrbitEntryType entryType) => entryType switch
    {
        OrbitEntryType.Good => "GOOD!",
        OrbitEntryType.Perfect => "PERFECT!",
        OrbitEntryType.NearMiss => "NEAR MISS!",
        _ => ""
    };
}