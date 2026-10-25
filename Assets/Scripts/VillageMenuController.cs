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
    
    public void ToggleMenu(bool bIsTutorialMenuEnabled, bool bIsInGameMenuEnabled, bool bIsVillageMenuEnabled)
    {
        tutorialMenu.SetActive(bIsTutorialMenuEnabled);
        inGameMenu.SetActive(bIsInGameMenuEnabled);
        villageMenu.SetActive(bIsVillageMenuEnabled);
    }
    
    public void ShowTutorialMenu()
    {
        ToggleMenu(true, false, false);
    }

    public void ShowInGameMenu()
    {
        ToggleMenu(false, true, false);
    }

    public void ShowVillageMenu()
    {
        ToggleMenu(false, false, true);
    }

    public void ResetPlayerPosition()
    {
        playerReference.transform.position = playerStartPosition;
    }
}
