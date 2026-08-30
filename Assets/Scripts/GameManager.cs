using AudioSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        bool isGameLoaded = PlayerPrefs.GetInt(Constants.IS_GAME_LOADED, 0) == 1;

        if (!isGameLoaded)
        {
            PlayerPrefs.SetInt(Constants.IS_GAME_LOADED, 1);
            PokiUnitySDK.Instance.gameLoadingFinished();
            PokiUnitySDK.Instance.init();
        }
    }

    public void RestartGame()
    {
        PokiUnitySDK.Instance.gameplayStop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SoundManager.Instance.StopAll();
    }

    public void RestartGameWithDelay(float delay)
    {
        Invoke(nameof(RestartGame), delay);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(Constants.IS_GAME_LOADED, 0);
    }
}
