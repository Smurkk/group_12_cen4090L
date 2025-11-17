using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // 🔒 always a fresh game
        SaveSystem.LoadOnStart = false;
        Time.timeScale = 1f;
        EscToggleOptions.GameIsPaused = false;   // or your pause controller
        SceneManager.LoadSceneAsync(1);
    }

    public void ContinueGame()
    {
        if (SaveSystem.SaveExists())
        {
            SaveSystem.LoadOnStart = true;       // ✅ tell game to load save
            Time.timeScale = 1f;
            EscToggleOptions.GameIsPaused = false;
            SceneManager.LoadSceneAsync(1);
        }
        else
        {
            Debug.Log("Continue pressed, but no save file found.");
        }
    }

    public void MenuScreen()
    {
        // called from in-game (pause menu)
        SaveSystem.LoadOnStart = false;          // ✅ don't auto-load when we come back later
        EscToggleOptions.GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void OptionsScreen()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
