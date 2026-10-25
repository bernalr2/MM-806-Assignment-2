// VillageMenuController.cs
// Ryan James Bernal

// Purpose: Control the UI Menus in game

using UnityEngine;

public class VillageMenuController : MonoBehaviour
{
    private Vector3 playerStartPosition;

    [Header("Menu Object References")] 
    public GameObject tutorialMenu;
    public GameObject inGameMenu;
    public GameObject villageMenu;

    [Header("Player Reference")] 
    public GameObject playerReference;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStartPosition = playerReference.transform.position;
        ShowInGameMenu();
    }
    
    // Change each of the menus visibility depending on which is needed
    public void ToggleMenu(bool bIsTutorialMenuEnabled, bool bIsInGameMenuEnabled, bool bIsVillageMenuEnabled)
    {
        tutorialMenu.SetActive(bIsTutorialMenuEnabled);
        inGameMenu.SetActive(bIsInGameMenuEnabled);
        villageMenu.SetActive(bIsVillageMenuEnabled);
    }
    
    // Switch to the Tutorial Menu
    public void ShowTutorialMenu()
    {
        ToggleMenu(true, false, false);
    }

    // Switch to the In Game Menu
    public void ShowInGameMenu()
    {
        ToggleMenu(false, true, false);
    }

    // Switch to the Pause Menu
    public void ShowVillageMenu()
    {
        ToggleMenu(false, false, true);
    }

    // Teleport the user back to their original starting point
    public void ResetPlayerPosition()
    {
        playerReference.transform.position = playerStartPosition;
    }
}
