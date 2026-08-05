using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private int timesToRepeatTutorial = 2;
    [SerializeField] private int planetsToDisableLine = 4;
    [SerializeField] private float handOffsetY = 40f;
    [SerializeField] private Transform tutorialPanel;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform handContainer;
    [SerializeField] private OrbitTutorialScanner orbitTutorialScanner;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private VisitedPlanetsCounter visitedPlanets;

    [Header("Aim Restrictions")]
    [SerializeField] private TutorialLineEmitter tutorialAimLine;
    [SerializeField] private float allowedAimAngle = 15f;
    [SerializeField] private Color tutorialLineColor = Color.green;


    private int tutorialCounter;
    private Vector2 idealAimDirection;

    void Start()
    {
        bool isTutorialCompleted = PlayerPrefs.GetInt(Constants.IS_TUTORIAL_COMPLETED, 0) == 1;

        orbitTutorialScanner.enabled = !isTutorialCompleted;

        if (tutorialAimLine != null)
            tutorialAimLine.HideLine();

        if (!isTutorialCompleted)
        {
            orbitTutorialScanner.OnOrbitHitDetected += StopTime;
            playerController.OnPlayerLaunched += ReleaseTime;
            playerController.CanLaunch = false;
        }
    }

    private void ReleaseTime()
    {
        tutorialPanel.gameObject.SetActive(false);

        if (tutorialAimLine != null)
            tutorialAimLine.HideLine();

        playerController.LaunchValidator = null;

        Time.timeScale = 1f;
        tutorialCounter++;
    }

    private void StopTime(Vector2 hitDirection, float hitDistance)
    {
        if (playerController.IsTransitioning)
            return;

        if (tutorialCounter >= timesToRepeatTutorial)
        {
            orbitTutorialScanner.OnOrbitHitDetected -= StopTime;
            playerController.OnPlayerLaunched -= ReleaseTime;
            orbitTutorialScanner.enabled = false;
            PlayerPrefs.SetInt(Constants.IS_TUTORIAL_COMPLETED, 1);
            return;
        }

        idealAimDirection = hitDirection;
        playerController.LaunchValidator = ValidateTutorialAim;

        if (tutorialAimLine != null)
        {
            tutorialAimLine.ShowLine(playerController.transform.position, idealAimDirection, hitDistance, tutorialLineColor);
        }

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, playerController.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, null, out Vector2 localPoint);
        localPoint.y -= handOffsetY;
        handContainer.localPosition = localPoint;
        tutorialPanel.gameObject.SetActive(true);

        Time.timeScale = 0f;
        playerController.CanLaunch = true;
    }

    private bool ValidateTutorialAim(Vector2 playerAimDir)
    {
        float angleDifference = Vector2.Angle(idealAimDirection, playerAimDir);
        return angleDifference <= allowedAimAngle;
    }
}