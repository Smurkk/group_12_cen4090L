using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void OptionScreen()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void MenuScreen()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
