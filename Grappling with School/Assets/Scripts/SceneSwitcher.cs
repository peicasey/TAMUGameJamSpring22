using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public void GotoMenuScene() {
        Debug.Log("Going to Menu Scene");
        SceneManager.LoadScene("MenuScreen"); 
    }

    public void GotToCredits() {
        Debug.Log("Going to Credits Scene");
        SceneManager.LoadScene("CreditsScreen");
    }

    public void GoToHowToPlay() {
        SceneManager.LoadScene("HowToScreen"); // might want to rename
    }


    public void GoToLevel1() {
        SceneManager.LoadScene(1); // might want to rename
    }

    public void GoToLevel2() {
        SceneManager.LoadScene(2); // might want to rename
    }

    public void GoToLevel3() {
        SceneManager.LoadScene(3); // might want to rename
    }


    public void GoToLevelSelect() {
        Debug.Log("Go to level select screen -- doesn't exist yet");
        SceneManager.LoadScene("LevelSelectScreen");
    }

    public void quitGame() {
        Application.Quit();
        Debug.Log("Exiting Game");
    }
}
