using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField] private ShopItemType type;
    [SerializeField] private int price;
    [SerializeField] private Sprite icon;

    public string ItemId => itemId;
    public string Name => itemName;
    public ShopItemType Type => type;
    public int Price => price;
    public Sprite Icon => icon;
}

public enum ShopItemType { RandomBox, RocketSkin, TrailSkin }