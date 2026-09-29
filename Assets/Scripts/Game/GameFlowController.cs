using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameView;
    [SerializeField] private GameObject gameOverView;

    private bool isTutorialCompleted;

    private void OnEnable()
    {
        player.OnPlayerLaunched += OnPlayerLaunched;
        player.OnPlayerDestroyed += OnPlayerDestroyed;
    }

    private void OnDisable()
    {
        player.OnPlayerLaunched -= OnPlayerLaunched;
        player.OnPlayerDestroyed -= OnPlayerDestroyed;
    }

    private void OnPlayerLaunched()
    {
        isTutorialCompleted = PlayerPrefs.GetInt(Constants.IS_TUTORIAL_COMPLETED_KEY, 0) == 1;

        if (!isTutorialCompleted)
        { 
            return; 
        }

        gameView.SetActive(true);
    }

    private void Start()
    {
        isTutorialCompleted = PlayerPrefs.GetInt(Constants.IS_TUTORIAL_COMPLETED_KEY, 0) == 1;

        bool skipMenu = GameManager.SkipMainMenu;
        GameManager.SkipMainMenu = false;

        player.ControlsBlocked = isTutorialCompleted;
        mainMenu.gameObject.SetActive(isTutorialCompleted);

        if (isTutorialCompleted)
        {
            gameView.SetActive(false);
        }

        if (isTutorialCompleted && skipMenu)
        {
            OnPlayButtonPressed();
        }
    }

    public void OnPlayButtonPressed()
    {
        mainMenu.gameObject.SetActive(false);
        gameView.SetActive(true);
        player.ControlsBlocked = false;
    }

    private void OnPlayerDestroyed()
    {
        gameView.SetActive(false);
        gameOverView.SetActive(true);
        player.ControlsBlocked = true;
    }
}
