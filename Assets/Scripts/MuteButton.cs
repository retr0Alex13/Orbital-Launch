using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite mutedImage;
    [SerializeField] private Sprite unmutedImage;

    private void Start()
    {
        bool soundState = PlayerPrefs.GetInt(Constants.SOUND_STATE_KEY, 1) == 1;
        buttonImage.sprite = soundState ? unmutedImage : mutedImage;
        AudioListener.volume = soundState ? 1 : 0;
    }

    public void ToggleSound()
    {
        bool soundState = PlayerPrefs.GetInt(Constants.SOUND_STATE_KEY, 1) == 0;
        PlayerPrefs.SetInt(Constants.SOUND_STATE_KEY, soundState ? 1 : 0);
        buttonImage.sprite = soundState ? unmutedImage : mutedImage;
        AudioListener.volume = soundState ? 1 : 0;
    }
}
