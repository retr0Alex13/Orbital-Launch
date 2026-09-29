using AudioSystem;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject shopMenu;

    private GameObject currentMenu;

    public void OpenInventory()
    {
        if (currentMenu == inventoryMenu)
        {
            CloseCurentMenu();
            return;
        }

        CloseCurentMenu();

        inventoryMenu.SetActive(true);
        currentMenu = inventoryMenu;
    }

    public void OpenShop()
    {
        if (currentMenu == shopMenu)
        {
            CloseCurentMenu();
            return;
        }

        CloseCurentMenu();

        shopMenu.SetActive(true);
        currentMenu = shopMenu;
    }

    public void CloseCurentMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            currentMenu = null;
        }
    }
}