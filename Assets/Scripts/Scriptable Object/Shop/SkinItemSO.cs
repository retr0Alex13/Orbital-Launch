using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Skin Item")]
public class SkinItemSO : ShopItemSO
{
    [SerializeField] private bool isDefault;
    public bool IsDefault => isDefault;
}