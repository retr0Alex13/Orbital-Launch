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
    [SerializeField] private GameObject ownedBadge;
    [SerializeField] private GameObject freeBadge;

    private ShopItemSO _data;
    private Action<ShopItemSO> _onClick;

    public void Bind(ShopItemSO data, ShopItemState state, Action<ShopItemSO> onClick)
    {
        _data = data;
        _onClick = onClick;

        iconImage.sprite = data.Icon;
        priceText.gameObject.SetActive(!state.CanClaimFree);
        priceText.text = data.Price.ToString();
        rarityText.text = data.Rarity.ToString();

        ownedBadge.SetActive(state.IsOwned);
        freeBadge.SetActive(state.CanClaimFree);
        actionButton.interactable = !state.IsOwned;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _onClick?.Invoke(_data));
    }
}