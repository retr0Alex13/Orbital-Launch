using TMPro;
using UnityEngine;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreTitle;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text bestScore;
    [SerializeField] private GameObject[] newBestBadges;

    public void OnEnable()
    {
        var scoreManager = ScoreManager.Instance;

        scoreManager.CommitFinalScore();

        score.text = scoreManager.orbitCount.ToString("N0");
        bestScore.text = scoreManager.BestScore.ToString("N0");

        if (scoreManager.IsNewBest)
        {
            scoreTitle.text = "New Best";

            foreach (var badge in newBestBadges)
            {
                badge.SetActive(true);
            }
        }
    }
}