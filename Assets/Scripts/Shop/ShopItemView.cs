using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonLabel;
    [SerializeField] private GameObject equippedBadge;
    [SerializeField] private GameObject lockBadge;
    [SerializeField] private GameObject coinIcon;

    private ShopItemSO _data;
    private Action<ShopItemSO> _onClick;

    public void Bind(ShopItemSO data, ShopItemState state, ShopItemViewMode mode, Action<ShopItemSO> onClick)
    {
        _data = data;
        _onClick = onClick;

        iconImage.sprite = data.Icon;
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

        bool showPrice = !state.IsOwned && !state.CanClaimFree;

        lockBadge.SetActive(!state.IsOwned);
        coinIcon.SetActive(showPrice);
        priceText.gameObject.SetActive(showPrice);
        priceText.text = data.Price.ToString();

        actionButtonLabel.gameObject.SetActive(!showPrice);
        actionButton.interactable = !state.IsOwned;
        actionButtonLabel.text = state.IsOwned ? "Owned" : (state.CanClaimFree ? "Free" : "Buy");
    }

    private void BindInventoryMode(ShopItemState state)
    {
        lockBadge.SetActive(false);
        coinIcon.SetActive(false);
        priceText.gameObject.SetActive(false);

        equippedBadge.SetActive(state.IsEquipped);
        actionButtonLabel.gameObject.SetActive(true);
        actionButton.interactable = !state.IsEquipped;
        actionButtonLabel.text = state.IsEquipped ? "Equipped" : "Equip";
    }
}