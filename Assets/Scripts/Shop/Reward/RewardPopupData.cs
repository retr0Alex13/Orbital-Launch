using UnityEngine;

public readonly struct RewardPopupData
{
    public readonly Sprite Icon;
    public readonly string Title;
    public readonly string ItemName;
    public readonly string ButtonText;

    public RewardPopupData(Sprite icon, string title, string itemName, string buttonText = "OK")
    {
        Icon = icon;
        Title = title;
        ItemName = itemName;
        ButtonText = buttonText;
    }
}