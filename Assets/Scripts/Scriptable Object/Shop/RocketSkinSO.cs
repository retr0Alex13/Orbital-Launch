using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Rocket Skin")]
public class RocketSkinSO : SkinItemSO
{
    [SerializeField] private Sprite rocketSprite;
    public Sprite RocketSprite => rocketSprite;
}