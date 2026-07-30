using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private int timesToRepeatTutorial = 2;

    [SerializeField]
    private int planetsToDisableLine = 4;

    [SerializeField]
    private Transform tutorialPanel;

    [SerializeField]
    private TrajectoryLineEmitter trajectoryLineEmitter;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private VisitedPlanetsCounter visitedPlanets;

    private int tutorialCounter;

    void Start()
    {
        if (!PlayerPrefs.HasKey(Constants.IS_FIRST_TIME_PLAYER))
        {
            PlayerPrefs.SetInt(Constants.IS_FIRST_TIME_PLAYER, 1);
        }

        if (PlayerPrefs.GetInt(Constants.IS_FIRST_TIME_PLAYER, 0) == 1)
        {
            trajectoryLineEmitter.OnOrbitHitDetected += StopTime;
            playerController.OnPlayerLaunched += ReleaseTime;
            playerController.CanLaunch = false;

            PlayerPrefs.SetInt(Constants.IS_FIRST_TIME_PLAYER, 0);
        }
    }

    private void ReleaseTime()
    {
        tutorialPanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
        tutorialCounter++;
    }

    private void StopTime()
    {
        if (tutorialCounter >= timesToRepeatTutorial)
        {
            trajectoryLineEmitter.OnOrbitHitDetected -= StopTime;
            playerController.OnPlayerLaunched -= ReleaseTime;
            return;
        }

        tutorialPanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
        playerController.CanLaunch = true;
    }
}
