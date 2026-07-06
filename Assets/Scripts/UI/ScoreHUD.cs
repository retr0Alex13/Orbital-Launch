using TMPro;
using UnityEngine;

public class ScoreHUD : MonoBehaviour
{
    [SerializeField]
    private ScorePopup popupPrefab;

    [SerializeField]
    private TextMeshProUGUI scoreLabel;

    [SerializeField]
    private Player player;

    private void Start()
    {
        if (ScoreManager.Instance == null)
            return;

       ScoreManager.Instance.OnScoreScoreChanged += UpdateDisplay;
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.OnScoreScoreChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int totalScore, OrbitEntryType orbitEntryType)
    {
        scoreLabel.text = totalScore.ToString("N0");
        popupPrefab.SetScore(orbitEntryType);
    }
}
