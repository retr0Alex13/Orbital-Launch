using AudioSystem;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance { get; private set; }

    [SerializeField] private SoundData buttonSound;
    [SerializeField] private SoundData purchaseSound;
    [SerializeField] private SoundData errorSound;

    private SoundBuilder soundBuilder;

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

    private void Start()
    {
        soundBuilder = SoundManager.Instance.CreateSoundBuilder();
    }

    public void PlayButtonSound()
    {
        soundBuilder.Play(buttonSound);
    }

    public void PlayPurchaseSound()
    {
        soundBuilder.Play(purchaseSound);
    }

    public void PlayErrorSound()
    {
        soundBuilder.Play(errorSound);
    }
}
