using UnityEngine;

[CreateAssetMenu(fileName = "ComboConfig", menuName = "Config/ComboConfig")]
public class ComboConfig : ScriptableObject
{
    public int streakToActivate = 3;

    public float initialComboMultiplier = 1.5f;
    public float multiplierIncrementPerContinue = 0.5f;
    public float maxComboMultiplier = 5f;

    public float baseSpeedMultiplier = 1f;
    public float speedIncrementPerCombo = 0.15f;
    public float maxSpeedMultiplier = 2f;
}
