using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Switch to a different scene using the input name
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        if (SceneManager.GetSceneByName(sceneName).IsValid())
        {
            Debug.Log(sceneName + " has successfully loaded!");
        }
    }

    // Stop the game and return to desktop
    public void ExitGame()
    {
        Application.Quit();
    }
}
