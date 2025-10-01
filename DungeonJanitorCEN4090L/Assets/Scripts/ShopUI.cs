using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    public void OpenProgressionTree()
    {
        SceneManager.LoadScene("ProgressionTreeScene");
    }
}
