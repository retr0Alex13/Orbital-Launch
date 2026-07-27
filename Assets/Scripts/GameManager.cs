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
}
