using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas levelsMenu;

    private void Start()
    {
        ActivateMainMenu();
    }

    public void ActivateMainMenu()
    {
        levelsMenu.gameObject.SetActive(false);
    }

    public void ActivateLevelsMenu()
    {
        levelsMenu.gameObject.SetActive(true);
    }
}
