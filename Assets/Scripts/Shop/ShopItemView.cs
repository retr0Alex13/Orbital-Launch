using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonLabel;
    [SerializeField] private GameObject ownedBadge;
    [SerializeField] private GameObject freeBadge;
    [SerializeField] private GameObject equippedBadge;

    private ShopItemSO _data;
    private Action<ShopItemSO> _onClick;

    public void Bind(ShopItemSO data, ShopItemState state, ShopItemViewMode mode, Action<ShopItemSO> onClick)
    {
        _data = data;
        _onClick = onClick;

        iconImage.sprite = data.Icon;
        rarityText.text = data.Rarity.ToString();

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _onClick?.Invoke(_data));

        if (mode == ShopItemViewMode.Shop)
            BindShopMode(data, state);
        else
            BindInventoryMode(state);
    }

    private void BindShopMode(ShopItemSO data, ShopItemState state)
    {
        equippedBadge.SetActive(false);

        ownedBadge.SetActive(state.IsOwned);
        freeBadge.SetActive(state.CanClaimFree);
        priceText.gameObject.SetActive(!state.IsOwned && !state.CanClaimFree);
        priceText.text = data.Price.ToString();

        actionButton.interactable = !state.IsOwned;
        actionButtonLabel.text = state.CanClaimFree ? "Free" : "Buy";
    }

    private void BindInventoryMode(ShopItemState state)
    {
        ownedBadge.SetActive(false);
        freeBadge.SetActive(false);
        priceText.gameObject.SetActive(false);

        equippedBadge.SetActive(state.IsEquipped);
        actionButton.interactable = !state.IsEquipped;
        actionButtonLabel.text = state.IsEquipped ? "Equipped" : "Equip";
    }
}