using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text closeButtonLabel;

    private Action _onClose;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        SetVisible(false);
    }

    public void Show(RewardPopupData data, Action onClose = null)
    {
        _onClose = onClose;

        iconImage.sprite = data.Icon;
        titleText.text = data.Title;
        itemNameText.text = data.ItemName;
        closeButtonLabel.text = data.ButtonText;

        SetVisible(true);
    }

    private void Close()
    {
        SetVisible(false);
        var callback = _onClose;
        _onClose = null;
        callback?.Invoke();
    }

    private void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
}