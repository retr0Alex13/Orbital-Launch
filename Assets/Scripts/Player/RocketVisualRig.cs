using UnityEngine;

public class RocketVisualRig : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private TrailRenderer[] trails;
    [SerializeField] private TrailController[] trailControllers;
    [SerializeField] private ParticleSystem turbineParticles;

    public SpriteRenderer BodySprite => bodySprite;
    public TrailRenderer[] Trails => trails;
    public TrailController[] TrailControllers => trailControllers;
    public ParticleSystem TurbineParticles => turbineParticles;
}