using AudioSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static bool SkipMainMenu { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void RestartGame()
    {
        PokiUnitySDK.Instance.gameplayStop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SoundManager.Instance.StopAll(AudioPlayer.BgmEmitter);
    }

    public void RestartGameWithoutMenu()
    {
        SkipMainMenu = true;
        RestartGame();
    }

    public void RestartGameWithDelay(float delay)
    {
        Invoke(nameof(RestartGame), delay);
    }

    public void RestartGameWithoutMenuWithDelay(float delay)
    {
        Invoke(nameof(RestartGameWithoutMenu), delay);
    }
}