using System;

public class ShopService
{
    private readonly PlayerCoinsController _coins;
    private readonly InventoryService _inventory;

    public int PlayerCoins => _coins.Coins;

    public event Action<SkinItemSO> OnPurchaseCompleted;
    public event Action<SkinItemSO> OnDuplicateReceived;
    public event Action<ShopItemSO> OnPurchaseFailed;

    public ShopService(PlayerCoinsController coins, InventoryService inventory)
    {
        _coins = coins;
        _inventory = inventory;
    }

    public void Purchase(ShopItemSO item)
    {
        if (_inventory.IsOwned(item.ItemId))
            return;

        if (!_coins.TrySpendCoins(item.Price))
        {
            OnPurchaseFailed?.Invoke(item);
            return;
        }

        GrantItem(item);
    }

    public void ClaimFreeBox(RandomBoxSO box)
    {
        if (_inventory.HasClaimedFreeBox)
            return;

        _inventory.MarkFreeBoxClaimed();
        GrantItem(box);
    }

    private void GrantItem(ShopItemSO item)
    {
        var granted = item is RandomBoxSO box ? box.RollRandomSkin(_inventory) : (SkinItemSO)item;

        if (_inventory.IsOwned(granted.ItemId))
        {
            _coins.AddCoins(granted.Price / 2);
            OnDuplicateReceived?.Invoke(granted);
            return;
        }

        _inventory.GrantSkin(granted);
        OnPurchaseCompleted?.Invoke(granted);
    }
}