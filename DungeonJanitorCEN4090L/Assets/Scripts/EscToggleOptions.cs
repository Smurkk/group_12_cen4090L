using UnityEngine;
using UnityEngine.SceneManagement;

public class EstToggleOptions : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;

    public static bool GameIsPaused { get; private set; }  // 👈 global flag

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        GameIsPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        GameIsPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Optional: hook this to a “Quit to Main Menu” button
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Make sure time scale is normal
        SceneManager.LoadScene("MainMenu");
    }
}
