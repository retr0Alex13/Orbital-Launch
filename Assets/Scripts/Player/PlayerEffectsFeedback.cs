using AudioSystem;
using UnityEngine;

public class PlayerEffectsFeedback : MonoBehaviour
{
    private ParticleSystem rocketThrustVFX;
    private GameObject rocketExplosion;
    private TrailRenderer[] rocketTrails;
    private SpriteRenderer rocketSprite;
    private SoundData rocketThrustSound;
    private SoundData rocketLaunchSound;
    private SoundData rocketExplosionSound;

    private SoundBuilder soundBuilder;
    private SoundEmitter engineSound;

    private bool isThrustActive;
    private bool areTrailsActive = true;
    private bool isSpriteActive = true;

    public void Initialize(ParticleSystem rocketThrust, GameObject rocketExplosion,
        TrailRenderer[] rocketTrails, SpriteRenderer rocketSprite,
        SoundData rocketThrustSound, SoundData rocketLaunchSound, SoundData rocketExplosionSound)
    {
        this.rocketThrustVFX = rocketThrust;
        this.rocketExplosion = rocketExplosion;
        this.rocketTrails = rocketTrails;
        this.rocketSprite = rocketSprite;
        this.rocketThrustSound = rocketThrustSound;
        this.rocketLaunchSound = rocketLaunchSound;
        this.rocketExplosionSound = rocketExplosionSound;

        soundBuilder = SoundManager.Instance.CreateSoundBuilder().WithRandomPitch();
    }

    public void UpdateVisualReferences(ParticleSystem rocketThrust, TrailRenderer[] rocketTrails, SpriteRenderer rocketSprite)
    {
        this.rocketThrustVFX = rocketThrust;
        this.rocketTrails = rocketTrails;
        this.rocketSprite = rocketSprite;

        SetThrustActive(isThrustActive);
        SetTrailsActive(areTrailsActive);
        SetSpriteActive(isSpriteActive);
    }

    public void HandleLaunched()
    {
        engineSound?.Stop();
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
        isThrustActive = active;

        if (active)
        {
            engineSound = soundBuilder.Play(rocketThrustSound);
            rocketThrustVFX.Play(true);
        }
        else
        {
            engineSound?.Stop();
            rocketThrustVFX.Stop(true);
        }
    }

    public void PlayExplosionSFX()
    {
        soundBuilder.Play(rocketExplosionSound);
    }

    public void SetTrailsActive(bool active)
    {
        areTrailsActive = active;

        foreach (TrailRenderer trail in rocketTrails)
            trail.gameObject.SetActive(active);
    }

    public void SetSpriteActive(bool active)
    {
        isSpriteActive = active;
        rocketSprite.enabled = active;
    }

    public void SpawnExplosion()
    {
        Instantiate(rocketExplosion, transform.position, Quaternion.identity);
    }

    public void HandleCrashEffects()
    {
        SetSpriteActive(false);
        SetThrustActive(false);
        SetTrailsActive(false);
        PlayExplosionSFX();
        SpawnExplosion();
    }
}