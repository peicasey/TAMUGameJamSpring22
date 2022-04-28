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

    private int assignments;

    public MoveToNextLevel moveToNextLevel;

    public AudioSource audioPlayer;

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
        moveToNextLevel = FindObjectOfType<MoveToNextLevel>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameOverScreen)
        {
            GameOverScreen = GameObject.Find("GameOverScreen");
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        Instantiate<GameObject>(GameOverScreen);
    }

    public void RetryLevel()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void LoadEndScreen()
    {
        Debug.Log("Go to level select screen -- doesn't exist yet");
        SceneManager.LoadScene("GameOverScreen");
    }

    public void GiveUp()
    {
        Debug.Log("Go to level select screen -- doesn't exist yet");
        SceneManager.LoadScene("LevelSelectScreen");
    }


    public void AddAssignment()
    {
        assignments++;
    }

    public void RemoveAssignment()
    {
        assignments--;
        if (assignments == 0)
        {
            moveToNextLevel.OpenDoor();
            audioPlayer.Play();
        }
    }
}
