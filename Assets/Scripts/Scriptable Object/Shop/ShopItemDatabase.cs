using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item Database")]
public class ShopItemDatabase : ScriptableObject
{
    [SerializeField] private RandomBoxSO[] boxes;
    [SerializeField] private RocketSkinSO[] rocketSkins;
    [SerializeField] private TrailSkinSO[] trailSkins;

    public RandomBoxSO[] Boxes => boxes;
    public RocketSkinSO[] RocketSkins => rocketSkins;
    public TrailSkinSO[] TrailSkins => trailSkins;

    public IEnumerable<SkinItemSO> AllSkins => rocketSkins.Cast<SkinItemSO>().Concat(trailSkins);
}