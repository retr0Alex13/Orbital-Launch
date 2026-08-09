using AudioSystem;
using UnityEngine;
public class PlayerEffectsFeedback : MonoBehaviour
{
    private ParticleSystem rocketThrust;
    private GameObject rocketExplosion;
    private TrailRenderer[] rocketTrails;
    private SpriteRenderer rocketSprite;
    private SoundData rocketThrustSound;
    private SoundData rocketLaunchSound;

    private SoundBuilder soundBuilder;
    private SoundEmitter engineSound;

    public void Initialize(ParticleSystem rocketThrust, GameObject rocketExplosion,
        TrailRenderer[] rocketTrails, SpriteRenderer rocketSprite,
        SoundData rocketThrustSound, SoundData rocketLaunchSound)
    {
        this.rocketThrust = rocketThrust;
        this.rocketExplosion = rocketExplosion;
        this.rocketTrails = rocketTrails;
        this.rocketSprite = rocketSprite;
        this.rocketThrustSound = rocketThrustSound;
        this.rocketLaunchSound = rocketLaunchSound;

        soundBuilder = SoundManager.Instance.CreateSoundBuilder().WithRandomPitch();
    }

    public void HandleLaunched()
    {
        soundBuilder.Play(rocketLaunchSound);
        SetThrustActive(true);
    }

    public void HandleCaptured(Planet planet)
    {
        soundBuilder.Play(rocketLaunchSound);
        SetThrustActive(false);
    }

    public void SetThrustActive(bool active)
    {
        if (active)
        {
            engineSound = soundBuilder.Play(rocketThrustSound);
            rocketThrust.Play(true);
        }
        else
        {
            engineSound?.Stop();
            rocketThrust.Stop(true);
        }
    }

    public void SetTrailsActive(bool active)
    {
        foreach (TrailRenderer trail in rocketTrails)
        {
            if (active)
            {
                trail.gameObject.SetActive(true);
            }
            else
            {
                trail.gameObject.SetActive(false);

            }
        }
    }

    public void SetSpriteActive(bool active)
    {
        if (active)
        {
            rocketSprite.enabled = true;
        }
        else
        {
            rocketSprite.enabled = false;
        }
    }

    public void SpawnExplosion()
    {
        Instantiate(rocketExplosion, transform.position, Quaternion.identity);
    }
}