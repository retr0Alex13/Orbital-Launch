using TMPro;
using UnityEngine;

public class ScoreCoinDisplay : MonoBehaviour
{
    [SerializeField] private ScorePopup popupPrefab;
    [SerializeField] private PlayerCoinsController playerCoinsController;

    [SerializeField] private TextMeshProUGUI scoreValue;
    [SerializeField] private TextMeshProUGUI coinsValue;

    [SerializeField] private RectTransform coinIcon;

    private void Start()
    {
        if (ScoreManager.Instance == null)
            return;

       ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
       playerCoinsController.OnCoinCollected += UpdateCoinDisplay;

        int coins = PlayerPrefs.GetInt(Constants.PLAYER_COINS_KEY, 0);
        UpdateCoinDisplay(coins);
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
        playerCoinsController.OnCoinCollected -= UpdateCoinDisplay;
    }

    private void UpdateScoreDisplay(object sender, ScoreEventArgs scoreEvent)
    {
        scoreValue.text = scoreEvent.Score.ToString("N0");
        popupPrefab.SetScore(scoreEvent.OrbitEntry, scoreEvent.PointsAwarded, scoreEvent.ComboActive, scoreEvent.CurrentComboMultiplier);
    }

    private void UpdateCoinDisplay(int coinsAmount)
    {
        coinsValue.text = coinsAmount.ToString();
    }
}
