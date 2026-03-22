using System.Collections;
using UnityEngine;

/// <summary>
/// Manages UI screen navigation and transitions.
/// Handles showing and hiding different screens in the UI system.
/// </summary>
public class UiManager : MonoBehaviour
{
    UiScreen currentScreen;

    /// <summary>
    /// Transitions from the current screen to a new screen.
    /// Hides the current screen and shows the specified screen.
    /// </summary>
    /// <param name="screen">The UI screen to show</param>
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
