using UnityEngine;

public class GameServicesBootstrapper : MonoBehaviour
{
    public static InventoryService Inventory { get; private set; }
    public static ShopService Shop { get; private set; }
    public static ShopItemDatabase Database { get; private set; }

    [SerializeField] private PlayerCoinsController playerCoins;
    [SerializeField] private ShopItemDatabase database;
    [SerializeField] private ShopController shopController;
    [SerializeField] private InventoryController inventoryController;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Database = database;
        Inventory = new InventoryService(database);
        Shop = new ShopService(playerCoins, Inventory);

        shopController.Initialize(Shop, Inventory);
        inventoryController.Initialize(Inventory);
    }
}