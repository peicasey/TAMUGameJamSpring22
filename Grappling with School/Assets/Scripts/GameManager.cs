using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{

    // VARIABLES
    public static GameManager instance;

    public GameObject player1;
    public Transform P1pos;

    public GameObject audioManager;

    public GameObject GameOverScreen;


    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        GameOverScreen.SetActive(true);
    }

    public void RetryLevel()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void LoadEndScreen()
    {
        Debug.Log("Go to level select screen -- doesn't exist yet");
        // SceneManager.LoadScene("LevelSelectScreen");
    }

    public void GiveUp()
    {
        Debug.Log("Go to level select screen -- doesn't exist yet");
        // SceneManager.LoadScene("LevelSelectScreen");
    }
}
