using UnityEngine;

public class TrailSkinApplier : MonoBehaviour
{
    [SerializeField] private TrailRenderer[] trails;
    [SerializeField] private TrailSkinSO defaultSkin;

    private InventoryService _inventory;
    private ShopItemDatabase _database;

    public void Initialize(InventoryService inventory, ShopItemDatabase database)
    {
        _inventory = inventory;
        _database = database;

        _inventory.OnSkinEquipped += HandleSkinEquipped;
        ApplyCurrentSkin();
    }

    private void OnDestroy()
    {
        if (_inventory != null)
            _inventory.OnSkinEquipped -= HandleSkinEquipped;
    }

    private void ApplyCurrentSkin()
    {
        string equippedId = _inventory.GetEquippedTrailSkinId();
        var skin = FindSkinById(equippedId) ?? defaultSkin;

        if (skin != null)
        {
            foreach(var trail in trails)
            {
                trail.colorGradient = skin.TrailGradient;
            }
        }
    }

    private void HandleSkinEquipped(SkinItemSO skin)
    {
        if (skin is TrailSkinSO trailSkin)
        {
            foreach (var trail in trails)
            {
                trail.colorGradient = trailSkin.TrailGradient;
            }
        }
    }

    private TrailSkinSO FindSkinById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        foreach (var skin in _database.TrailSkins)
            if (skin.ItemId == id) return skin;
        return null;
    }
}