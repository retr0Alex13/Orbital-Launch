using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private MainMenuWindow mainMenu;
    [SerializeField] private GameObject gameView;

    private bool isTutorialCompleted;

    private void OnEnable()
    {
        player.OnPlayerLaunched += OnPlayerLaunched;
    }

    private void OnDisable()
    {
        player.OnPlayerLaunched -= OnPlayerLaunched;
    }

    private void OnPlayerLaunched()
    {
        isTutorialCompleted = PlayerPrefs.GetInt(Constants.IS_TUTORIAL_COMPLETED, 0) == 1;

        if (!isTutorialCompleted)
        { 
            return; 
        }

        gameView.SetActive(true);
    }

    private void Start()
    {
        isTutorialCompleted = PlayerPrefs.GetInt(Constants.IS_TUTORIAL_COMPLETED, 0) == 1;
        player.ControlsBlocked = isTutorialCompleted;
        mainMenu.gameObject.SetActive(isTutorialCompleted);

        if (isTutorialCompleted)
        {
            gameView.SetActive(false);
        }
    }

    public void OnPlayButtonPressed()
    {
        mainMenu.gameObject.SetActive(false);
        gameView.SetActive(true);
        player.ControlsBlocked = false;
    }
}
