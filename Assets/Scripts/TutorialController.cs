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
    private RectTransform canvas;

    [SerializeField]
    private RectTransform handContainer;

    [SerializeField]
    private OrbitTutorialScanner orbitTutorialScanner;

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

        bool isFirstTimePlayer = PlayerPrefs.GetInt(Constants.IS_FIRST_TIME_PLAYER, 0) == 1;

        orbitTutorialScanner.enabled = isFirstTimePlayer;

        if (isFirstTimePlayer)
        {
            orbitTutorialScanner.OnOrbitHitDetected += StopTime;
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
            orbitTutorialScanner.OnOrbitHitDetected -= StopTime;
            playerController.OnPlayerLaunched -= ReleaseTime;
            orbitTutorialScanner.enabled = false;
            return;
        }
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, playerController.transform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, null, out Vector2 localPoint);
        localPoint.y -= 10f;
        handContainer.localPosition = localPoint;
        tutorialPanel.gameObject.SetActive(true);

        Time.timeScale = 0f;
        playerController.CanLaunch = true;
    }
}