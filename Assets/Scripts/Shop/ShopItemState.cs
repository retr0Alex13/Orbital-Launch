public readonly struct ShopItemState
{
    public readonly bool IsOwned;
    public readonly bool IsEquipped;
    public readonly bool CanClaimFree;

    public ShopItemState(bool isOwned, bool isEquipped, bool canClaimFree)
    {
        IsOwned = isOwned;
        IsEquipped = isEquipped;
        CanClaimFree = canClaimFree;
    }
}