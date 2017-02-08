using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Menu CurrentMenu;

    private bool firstFrame = true;

    private void LateUpdate()
    {
        if (firstFrame)
        {
            firstFrame = false;
            LoadDefaultMenu();
        }
    }

    private void LoadDefaultMenu()
    {
        ShowMenu(CurrentMenu);
    }
    
    /// <summary>
    /// Calls ShowMenu(null)
    /// </summary>
    /// <remarks>
    /// This is purely used because UnityEvents will throw
    /// NullReferenceExceptions if you send them an empty parameter
    /// </remarks>
    public void ClearMenu()
    {
        ShowMenu(null);
    }

    public void ShowMenu(Menu menu)
    {

        if (CurrentMenu != null)
        {
            CurrentMenu.IsOpen = false;
        }

        CurrentMenu = menu;

        if (menu != null)
        {
            CurrentMenu.IsOpen = true;
        }
    }
}
