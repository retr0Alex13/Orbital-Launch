using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private ShopItemView itemViewPrefab;
    [SerializeField] private Transform boxesContainer;
    [SerializeField] private Transform skinsContainer;
    [SerializeField] private ShopItemDatabase database;
    [SerializeField] private TextMeshProUGUI playerCoins;

    private ShopService _shop;
    private InventoryService _inventory;
    private readonly List<ShopItemView> _spawnedViews = new();

    public void Initialize(ShopService shop, InventoryService inventory)
    {
        _shop = shop;
        _inventory = inventory;

        _shop.OnPurchaseCompleted += HandlePurchaseCompleted;
        _shop.OnPurchaseFailed += HandlePurchaseFailed;

        Refresh();
    }

    private void OnEnable()
    {
        if (_inventory != null)
            Refresh();
    }

    private void OnDestroy()
    {
        if (_shop == null) return;
        _shop.OnPurchaseCompleted -= HandlePurchaseCompleted;
        _shop.OnPurchaseFailed -= HandlePurchaseFailed;
    }

    private void HandlePurchaseCompleted(SkinItemSO _) => Refresh();

    private void Refresh()
    {
        playerCoins.text = _shop.PlayerCoins.ToString();
        ClearViews();

        foreach (var box in database.Boxes)
            SpawnItemView(box, boxesContainer);

        foreach (var skin in database.RocketSkins)
            SpawnItemView(skin, skinsContainer);

        foreach (var skin in database.TrailSkins)
            SpawnItemView(skin, skinsContainer);
    }

    private void SpawnItemView(ShopItemSO item, Transform container)
    {
        var view = Instantiate(itemViewPrefab, container);
        var state = _inventory.GetState(item);
        view.Bind(item, state, OnItemClicked);
        _spawnedViews.Add(view);
    }

    private void OnItemClicked(ShopItemSO item)
    {
        var state = _inventory.GetState(item);

        if (item is RandomBoxSO box && state.CanClaimFree)
        {
            _shop.ClaimFreeBox(box);
            return;
        }

        _shop.Purchase(item);
        playerCoins.text = _shop.PlayerCoins.ToString();
    }

    private void HandlePurchaseFailed(ShopItemSO item)
    {
        Debug.Log($"Недостатньо монет для {item.name}");
    }

    private void ClearViews()
    {
        foreach (var view in _spawnedViews)
            Destroy(view.gameObject);
        _spawnedViews.Clear();
    }
}