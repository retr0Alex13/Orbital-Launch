using AudioSystem;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance { get; private set; }
    public static SoundEmitter BgmEmitter => bgmEmitter;

    [SerializeField] private SoundData buttonSound;
    [SerializeField] private SoundData purchaseSound;
    [SerializeField] private SoundData errorSound;
    [SerializeField] private SoundData backgroundMusic;

    private static SoundEmitter bgmEmitter;

    private SoundBuilder soundBuilder;

    private void Awake() => Instance = this;

    private void Start() => soundBuilder = SoundManager.Instance.CreateSoundBuilder();

    public void PlayButtonSound() => soundBuilder.Play(buttonSound);
    public void PlayPurchaseSound() => soundBuilder.Play(purchaseSound);
    public void PlayErrorSound() => soundBuilder.Play(errorSound);

    public void PlayBackgroundMusic()
    {
        bool alreadyPlaying = bgmEmitter != null
                              && bgmEmitter.gameObject.activeInHierarchy
                              && bgmEmitter.Data != null
                              && bgmEmitter.Data.clip == backgroundMusic.clip;
        if (alreadyPlaying) return;

        bgmEmitter = soundBuilder.Play(backgroundMusic);
    }
}