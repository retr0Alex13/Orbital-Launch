using AudioSystem;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject shopMenu;

    private GameObject currentMenu;

    public void OpenInventory()
    {
        CloseCurentMenu();
        AudioPlayer.Instance.PlayButtonSound();

        inventoryMenu.SetActive(true);
        currentMenu = inventoryMenu;
    }

    public void OpenShop()
    {
        CloseCurentMenu();
        AudioPlayer.Instance.PlayButtonSound();

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