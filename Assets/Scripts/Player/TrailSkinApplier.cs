using System;
using UnityEngine;

public class TrailSkinApplier : MonoBehaviour
{
    [SerializeField] private RocketSkinApplier rocketSkinApplier;
    [SerializeField] private TrailSkinSO defaultSkin;

    private InventoryService _inventory;
    private ShopItemDatabase _database;

    public void Initialize(InventoryService inventory, ShopItemDatabase database)
    {
        _inventory = inventory;
        _database = database;

        _inventory.OnSkinEquipped += HandleSkinEquipped;
        rocketSkinApplier.OnRigChanged += HandleRigChanged;

        // rig уже створений в RocketSkinApplier.Initialize() до нашої підписки — застосувати колір одразу
        ApplyToRig(rocketSkinApplier.CurrentRig);
    }

    private void OnDestroy()
    {
        if (_inventory != null)
            _inventory.OnSkinEquipped -= HandleSkinEquipped;
        if (rocketSkinApplier != null)
            rocketSkinApplier.OnRigChanged -= HandleRigChanged;
    }

    private void HandleRigChanged(RocketVisualRig rig) => ApplyToRig(rig);

    private void HandleSkinEquipped(SkinItemSO skin)
    {
        if (skin is TrailSkinSO trailSkin)
            ApplyGradient(trailSkin, rocketSkinApplier.CurrentRig);
    }

    private void ApplyToRig(RocketVisualRig rig)
    {
        string equippedId = _inventory.GetEquippedTrailSkinId();
        var skin = FindSkinById(equippedId) ?? defaultSkin;

        if (skin != null)
            ApplyGradient(skin, rig);
    }

    private void ApplyGradient(TrailSkinSO skin, RocketVisualRig rig)
    {
        if (rig == null) return;

        foreach (var trail in rig.Trails)
            trail.colorGradient = skin.TrailGradient;
    }

    private TrailSkinSO FindSkinById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        foreach (var skin in _database.TrailSkins)
            if (skin.ItemId == id) return skin;
        return null;
    }
}