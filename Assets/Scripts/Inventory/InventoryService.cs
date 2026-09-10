using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryService
{
    private readonly InventorySaveData _data;
    private readonly ShopItemDatabase _database;

    public event Action<SkinItemSO> OnSkinAcquired;
    public event Action<SkinItemSO> OnSkinEquipped;

    public bool HasClaimedFreeBox => _data.hasClaimedFreeBox;

    public InventoryService(ShopItemDatabase database)
    {
        _database = database;

        bool isFirstLaunch = !PlayerPrefs.HasKey(Constants.INVENTORY_SAVE_KEY);
        _data = Load();

        if (isFirstLaunch)
            GrantDefaultSkins();
    }

    public bool IsOwned(string itemId) => _data.ownedItemIds.Contains(itemId);

    public bool IsEquipped(SkinItemSO skin)
    {
        return skin switch
        {
            RocketSkinSO => _data.equippedRocketSkinId == skin.ItemId,
            TrailSkinSO => _data.equippedTrailSkinId == skin.ItemId,
            _ => false
        };
    }

    public ShopItemState GetState(ShopItemSO item)
    {
        bool owned = IsOwned(item.ItemId);
        bool equipped = item is SkinItemSO skin && IsEquipped(skin);
        bool canClaimFree = item is RandomBoxSO box && box.IsFreeEligible && !HasClaimedFreeBox;

        return new ShopItemState(owned, equipped, canClaimFree);
    }

    public void GrantSkin(SkinItemSO skin)
    {
        if (IsOwned(skin.ItemId))
            return;

        _data.ownedItemIds.Add(skin.ItemId);
        Save();
        OnSkinAcquired?.Invoke(skin);
    }

    public void MarkFreeBoxClaimed()
    {
        _data.hasClaimedFreeBox = true;
        Save();
    }

    public void Equip(SkinItemSO skin)
    {
        if (!IsOwned(skin.ItemId))
            return;

        switch (skin)
        {
            case RocketSkinSO:
                _data.equippedRocketSkinId = skin.ItemId;
                break;
            case TrailSkinSO:
                _data.equippedTrailSkinId = skin.ItemId;
                break;
        }

        Save();
        OnSkinEquipped?.Invoke(skin);
    }

    public string GetEquippedRocketSkinId() => _data.equippedRocketSkinId;
    public string GetEquippedTrailSkinId() => _data.equippedTrailSkinId;

    private void Save()
    {
        string json = JsonUtility.ToJson(_data);
        PlayerPrefs.SetString(Constants.INVENTORY_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private InventorySaveData Load()
    {
        if (!PlayerPrefs.HasKey(Constants.INVENTORY_SAVE_KEY))
            return new InventorySaveData();

        string json = PlayerPrefs.GetString(Constants.INVENTORY_SAVE_KEY);
        return JsonUtility.FromJson<InventorySaveData>(json);
    }

    private void GrantDefaultSkins()
    {
        foreach (var skin in _database.AllSkins)
        {
            if (skin.IsDefault && !IsOwned(skin.ItemId))
                _data.ownedItemIds.Add(skin.ItemId);
        }

        var defaultRocket = _database.RocketSkins.FirstOrDefault(s => s.IsDefault);
        var defaultTrail = _database.TrailSkins.FirstOrDefault(s => s.IsDefault);

        if (defaultRocket != null)
            _data.equippedRocketSkinId = defaultRocket.ItemId;

        if (defaultTrail != null)
            _data.equippedTrailSkinId = defaultTrail.ItemId;

        Save();
    }
}