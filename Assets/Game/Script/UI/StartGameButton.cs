using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public string sceneName = "SC_Level0";
    public void LoadFirstScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
