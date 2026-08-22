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

       ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
       playerCoinsController.OnCoinCollected += UpdateCoinDisplay;
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
        playerCoinsController.OnCoinCollected -= UpdateCoinDisplay;
    }

    private void UpdateScoreDisplay(object sender, ScoreEventArgs scoreEvent)
    {
        scoreLabel.text = scoreEvent.Score.ToString("N0");
        popupPrefab.SetScore(scoreEvent.OrbitEntry, scoreEvent.PointsAwarded, scoreEvent.ComboActive, scoreEvent.CurrentComboMultiplier);
    }

    private void UpdateCoinDisplay(int coinsAmount)
    {
        coinsValue.text = coinsAmount.ToString();
    }
}
