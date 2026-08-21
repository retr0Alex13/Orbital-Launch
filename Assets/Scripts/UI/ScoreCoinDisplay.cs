using TMPro;
using UnityEngine;

public class ScoreCoinDisplay : MonoBehaviour
{
    [SerializeField] private ScorePopup popupPrefab;
    [SerializeField] private PlayerCoinsController playerCoinsController;

    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private TextMeshProUGUI coinsValue;

    [SerializeField] private RectTransform coinIcon;

    private void Start()
    {
        if (ScoreManager.Instance == null)
            return;

       ScoreManager.Instance.OnScoreScoreChanged += UpdateScoreDisplay;
       playerCoinsController.OnCoinCollected += UpdateCoinDisplay;
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.OnScoreScoreChanged -= UpdateScoreDisplay;
        playerCoinsController.OnCoinCollected -= UpdateCoinDisplay;
    }

    private void UpdateScoreDisplay(int totalScore, OrbitEntryType orbitEntryType)
    {
        scoreLabel.text = totalScore.ToString("N0");
        popupPrefab.SetScore(orbitEntryType);
    }

    private void UpdateCoinDisplay(int coinsAmount)
    {
        coinsValue.text = coinsAmount.ToString();
    }
}
