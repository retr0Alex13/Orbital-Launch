using TMPro;
using UnityEngine;
using PrimeTween;

public class ScorePopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField]  private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI comboLabel;
    [SerializeField] private TextMeshProUGUI comboMultiplierValue;

    [SerializeField] private OrbitEntryConfig orbitEntryConfig;
    [SerializeField] private RectTransform popupTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    private Vector2 startPosition;

    private void Awake()
    {
        startPosition = popupTransform.anchoredPosition;
    }

    public void SetScore(OrbitEntryType entryType, int pointsAwarded, bool isComboActive, float comboMultiplier)
    {
        popupTransform.anchoredPosition = startPosition;
        canvasGroup.alpha = 1;

        skillText.text = ScoreManager.Instance.GetLabel(entryType);

        if (isComboActive)
        {
            comboMultiplierValue.text = comboMultiplier.ToString();
            comboLabel.gameObject.SetActive(true);
            comboMultiplierValue.gameObject.SetActive(true);
        }

        scoreText.text = $"+{pointsAwarded}";

        AnimateAndHide();
    }

    private void AnimateAndHide()
    {
        Tween.UIAnchoredPositionY(popupTransform, endValue: startPosition.y + 100f, duration: 1f, ease: Ease.InOutSine);
        Tween.Alpha(canvasGroup, startValue: 1f, endValue: 0, duration: 1f, ease: Ease.InOutSine)
            .OnComplete(() => 
            {
                comboLabel.gameObject.SetActive(false);
                comboMultiplierValue.gameObject.SetActive(false);
            });
    }
}