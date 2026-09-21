using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Random Box")]
public class RandomBoxSO : ShopItemSO
{
    [SerializeField] private bool isFreeEligible;
    [SerializeField] private SkinItemSO[] possibleSkins;

    public bool IsFreeEligible => isFreeEligible;

    public SkinItemSO RollRandomSkin(InventoryService inventory)
    {
        var available = possibleSkins.Where(s => !inventory.IsOwned(s.ItemId)).ToArray();

        if (available.Length == 0)
            available = possibleSkins;

        int index = Random.Range(0, available.Length);
        return available[index];
    }
}