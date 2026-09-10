using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Random Box")]
public class RandomBoxSO : ShopItemSO
{
    [SerializeField] private bool isFreeEligible;
    [SerializeField] private SkinItemSO[] possibleSkins;
    [SerializeField] private float rareWeight = 70f;
    [SerializeField] private float epicWeight = 30f;

    public bool IsFreeEligible => isFreeEligible;

    public SkinItemSO RollRandomSkin(InventoryService inventory)
    {
        var available = possibleSkins.Where(s => !inventory.IsOwned(s.ItemId)).ToArray();

        if (available.Length == 0)
            available = possibleSkins;

        var weighted = new List<(SkinItemSO skin, float weight)>();
        foreach (var skin in available)
        {
            float weight = skin.Rarity == Rarity.Rare ? rareWeight : epicWeight;
            weighted.Add((skin, weight));
        }

        float totalWeight = weighted.Sum(w => w.weight);
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var (skin, weight) in weighted)
        {
            cumulative += weight;
            if (roll <= cumulative)
                return skin;
        }

        return weighted[^1].skin;
    }
}