using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextScene()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex;
        nextScene++;

        // return to start scene
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if (nextScene >= SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }
}
