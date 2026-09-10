using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Trail Skin")]
public class TrailSkinSO : SkinItemSO
{
    [SerializeField] private Gradient trailGradient;
    public Gradient TrailGradient => trailGradient;
}