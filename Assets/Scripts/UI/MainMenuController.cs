using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject shopMenu;

    private GameObject currentMenu;

    public void OpenInventory()
    {
        CloseCurentMenu();

        inventoryMenu.SetActive(true);
        currentMenu = inventoryMenu;
    }

    public void OpenShop()
    {
        CloseCurentMenu();

        shopMenu.SetActive(true);
        currentMenu = shopMenu;
    }

    public void CloseCurentMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
        }
    }
}