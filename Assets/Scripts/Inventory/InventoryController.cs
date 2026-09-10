using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private ShopItemView itemViewPrefab;
    [SerializeField] private Transform rocketSkinsContainer;
    [SerializeField] private Transform trailSkinsContainer;
    [SerializeField] private ShopItemDatabase database;

    private InventoryService _inventory;
    private readonly List<ShopItemView> _spawnedViews = new();

    public void Initialize(InventoryService inventory)
    {
        _inventory = inventory;
        _inventory.OnSkinAcquired += HandleSkinChanged;
        _inventory.OnSkinEquipped += HandleSkinChanged;

        Refresh();
    }

    private void OnEnable()
    {
        if (_inventory != null)
            Refresh();
    }

    private void OnDestroy()
    {
        if (_inventory == null) return;
        _inventory.OnSkinAcquired -= HandleSkinChanged;
        _inventory.OnSkinEquipped -= HandleSkinChanged;
    }

    private void HandleSkinChanged(SkinItemSO _) => Refresh();

    private void Refresh()
    {
        ClearViews();

        foreach (var skin in database.RocketSkins)
            SpawnItemView(skin, rocketSkinsContainer);

        foreach (var skin in database.TrailSkins)
            SpawnItemView(skin, trailSkinsContainer);
    }

    private void SpawnItemView(SkinItemSO skin, Transform container)
    {
        if (!_inventory.IsOwned(skin.ItemId))
            return;

        var view = Instantiate(itemViewPrefab, container);
        var state = _inventory.GetState(skin);
        view.Bind(skin, state, ShopItemViewMode.Inventory, OnItemClicked);
        _spawnedViews.Add(view);
    }

    private void OnItemClicked(ShopItemSO item)
    {
        if (item is SkinItemSO skin)
            _inventory.Equip(skin);
    }

    private void ClearViews()
    {
        foreach (var view in _spawnedViews)
            Destroy(view.gameObject);
        _spawnedViews.Clear();
    }
}