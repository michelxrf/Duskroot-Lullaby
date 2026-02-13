using System.Collections;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    UiScreen currentScreen;

    public void ShowScreen(UiScreen screen)
    {
        if (currentScreen != null)
        {
            currentScreen.Hide();
        }

        currentScreen = screen;
        currentScreen.Show();
    }
}
