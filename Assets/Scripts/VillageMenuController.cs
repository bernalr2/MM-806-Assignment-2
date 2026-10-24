using UnityEngine;

public class VillageMenuController : MonoBehaviour
{
    [Header("Menu Object References")]
    public GameObject VillageMenu;
    public GameObject MenuButton;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ToggleMenu(false);
    }

    public void ToggleMenu(bool bIsMenuEnabled)
    {
        VillageMenu.SetActive(bIsMenuEnabled);
        MenuButton.SetActive(!bIsMenuEnabled);
    }
}
