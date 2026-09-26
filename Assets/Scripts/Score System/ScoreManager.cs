using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event EventHandler<ScoreEventArgs> OnScoreChanged;
    public event Action<float> OnComboSpeedMultiplierChanged;


    [SerializeField] private OrbitEntryConfig orbitEntryConfig;
    [SerializeField] private ComboConfig comboConfig;

    public int orbitCount;
    private int streakCount;
    private bool comboActive;
    private bool firstEntry = true;
    private float currentComboMultiplier = 1f;
    private float currentSpeedMultiplier;
    private OrbitEntryType? streakType;

    private Vector2? previousPlanetPos;
    private float lastOrbitDistance;

    public bool IsComboActive => comboActive;
    public float CurrentComboMultiplier => currentComboMultiplier;
    public float CurrentSpeedMultiplier => currentSpeedMultiplier;
    public float LastOrbitDistance => lastOrbitDistance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        orbitCount = 0;
        currentSpeedMultiplier = comboConfig != null ? comboConfig.baseSpeedMultiplier : 1f;
    }

    public float AwardOrbitEntry(OrbitEntryInfo orbitEntryInfo)
    {
        if (firstEntry)
        {
            firstEntry = false;
            previousPlanetPos = orbitEntryInfo.CapturedPlanetPos;
            return currentSpeedMultiplier;
        }

        OrbitEntryType entryType = Evaluate(orbitEntryInfo, orbitEntryConfig);
        UpdateCombo(entryType);
        orbitCount++;

        lastOrbitDistance = previousPlanetPos.HasValue
            ? Vector2.Distance(previousPlanetPos.Value, orbitEntryInfo.CapturedPlanetPos)
            : 0f;
        previousPlanetPos = orbitEntryInfo.CapturedPlanetPos;

        ScoreEventArgs scoreEventArgs = new ScoreEventArgs(
            orbitCount,
            lastOrbitDistance,
            currentComboMultiplier,
            comboActive,
            entryType);

        OnScoreChanged?.Invoke(this, scoreEventArgs);

        return currentSpeedMultiplier;
    }

    private void UpdateCombo(OrbitEntryType entryType)
    {
        if (streakType == entryType)
        {
            streakCount++;
        }
        else
        {
            if (comboActive)
                ResetCombo();

            streakType = entryType;
            streakCount = 1;
        }

        if (!comboActive && streakCount >= comboConfig.streakToActivate)
        {
            comboActive = true;
            currentComboMultiplier = comboConfig.initialComboMultiplier;
            currentSpeedMultiplier = Mathf.Min(
                comboConfig.baseSpeedMultiplier + comboConfig.speedIncrementPerCombo,
                comboConfig.maxSpeedMultiplier);

            OnComboSpeedMultiplierChanged?.Invoke(currentSpeedMultiplier);
        }
        else if (comboActive && streakCount > comboConfig.streakToActivate)
        {
            currentComboMultiplier = Mathf.Min(
                currentComboMultiplier + comboConfig.multiplierIncrementPerContinue,
                comboConfig.maxComboMultiplier);

            currentSpeedMultiplier = Mathf.Min(
                currentSpeedMultiplier + comboConfig.speedIncrementPerCombo,
                comboConfig.maxSpeedMultiplier);

            OnComboSpeedMultiplierChanged?.Invoke(currentSpeedMultiplier);
        }
    }

    private void ResetCombo()
    {
        comboActive = false;
        currentComboMultiplier = 1f;
        currentSpeedMultiplier = comboConfig.baseSpeedMultiplier;

        OnComboSpeedMultiplierChanged?.Invoke(currentSpeedMultiplier);
    }

    public void BreakCombo()
    {
        if (comboActive)
            ResetCombo();

        streakType = null;
        streakCount = 0;
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

    public string GetLabel(OrbitEntryType entryType) => entryType switch
    {
        OrbitEntryType.Good => "GOOD!",
        OrbitEntryType.Perfect => "PERFECT!",
        OrbitEntryType.NearMiss => "NEAR MISS!",
        _ => ""
    };
}

public class ScoreEventArgs : EventArgs
{
    public int OrbitCount { get; }
    public float DistanceTraveled { get; }
    public float CurrentComboMultiplier { get; }
    public bool ComboActive { get; }
    public OrbitEntryType OrbitEntry { get; }

    public ScoreEventArgs(int orbitCount, float distanceTraveled, float comboMultiplier, bool isComboActive,
        OrbitEntryType orbitEntry)
    {
        OrbitCount = orbitCount;
        DistanceTraveled = distanceTraveled;
        CurrentComboMultiplier = comboMultiplier;
        ComboActive = isComboActive;
        OrbitEntry = orbitEntry;
    }
}