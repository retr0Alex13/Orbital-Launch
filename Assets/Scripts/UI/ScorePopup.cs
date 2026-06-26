using TMPro;
using UnityEngine;


public class ScorePopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI skillText;

    [SerializeField]
    private OrbitEntryConfig orbitEntryConfig;

    public void SetScore(OrbitEntryType entryType)
    {
        skillText.text = ScoreManager.Instance.GetLabel(entryType);
        scoreText.text = $"+{ScoreManager.Instance.GetPoints(entryType, orbitEntryConfig)}";
    }
}