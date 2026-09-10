using UnityEngine;

public class RocketSkinApplier : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rocketRenderer;
    [SerializeField] private RocketSkinSO defaultSkin;

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
        string equippedId = _inventory.GetEquippedRocketSkinId();
        var skin = FindSkinById(equippedId) ?? defaultSkin;

        if (skin != null)
            rocketRenderer.sprite = skin.RocketSprite;
    }

    private void HandleSkinEquipped(SkinItemSO skin)
    {
        if (skin is RocketSkinSO rocketSkin)
            rocketRenderer.sprite = rocketSkin.RocketSprite;
    }

    private RocketSkinSO FindSkinById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        foreach (var skin in _database.RocketSkins)
            if (skin.ItemId == id) return skin;
        return null;
    }
}