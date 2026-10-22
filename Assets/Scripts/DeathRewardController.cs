using System.Linq;
using UnityEngine;

public class DeathRewardController : MonoBehaviour
{
    [SerializeField] private ShopItemDatabase database;
    [SerializeField] private RewardPopup rewardPopup;
    [SerializeField] private Sprite coinIcon;

    [Header("Settings")]
    [SerializeField, Min(1)] private int rewardEveryNDeaths = 3;
    [SerializeField, Range(0f, 1f)] private float itemChance = 0.5f;
    [SerializeField] private Vector2Int coinsRange = new(20, 50);

    private InventoryService _inventory;
    private PlayerCoinsController _coins;

    public void Initialize(InventoryService inventory, PlayerCoinsController coins)
    {
        _inventory = inventory;
        _coins = coins;
    }

    public void OnPlayerDied()
    {
        int deaths = PlayerPrefs.GetInt(Constants.DEATH_COUNT_KEY, 0) + 1;
        PlayerPrefs.SetInt(Constants.DEATH_COUNT_KEY, deaths);
        PlayerPrefs.Save();

        if ((deaths - 1) % rewardEveryNDeaths != 0)
            return;

        GiveReward();
    }

    private void GiveReward()
    {
        var available = database.AllSkins
            .Where(s => !s.IsDefault && !_inventory.IsOwned(s.ItemId))
            .ToList();

        bool giveItem = available.Count > 0 && Random.value < itemChance;

        if (giveItem)
        {
            var skin = available[Random.Range(0, available.Count)];
            _inventory.GrantSkin(skin);
            AudioPlayer.Instance.PlayPurchaseSound();

            rewardPopup.Show(new RewardPopupData(
                skin.Icon, "New skin unlocked!", skin.Name, "Awesome!"));
        }
        else
        {
            // Або випав шанс на монети, або всі предмети вже зібрані
            int amount = Random.Range(coinsRange.x, coinsRange.y + 1);
            _coins.AddCoins(amount);

            rewardPopup.Show(new RewardPopupData(
                coinIcon, "You got coins!", $"+{amount}", "Nice!"));
        }
    }
}