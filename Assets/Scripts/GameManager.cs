using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public bool GameStarted { get; private set; }
    public bool GameEnded { get; private set; }
    public bool GamePaused { get; private set; }
    // public bool GameWon { get; private set; }
    // public bool GameLost { get; private set; }

    [Header("Arena")]
    public float sideEdgeDistance;
    public float topBotEdgeDistance;
    public float topBotCenterDistance;

    [Header("Destructables")]
    public float destBrokenDelay;

    void Awake()
    {
        // Creates a Single Instance of the game manager through out the entire game
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        GameStarted = true;
    }

    public void PauseOrResumeGame()
    {
        GamePaused = !GamePaused;

        // pauseMenu.SetActive(GamePaused);

        // if (GamePaused)
        // {
        //     Time.timeScale = 0f;

        //     // Pauses Player Running Sound in the background
        //     // playerController.playerRunningAudioSource.Pause();
        // }
        // else
        // {
        //     Time.timeScale = 1f;

        //     // Plays Player Running Sound in the background
        //     // playerController.playerRunningAudioSource.Play();
        // }
    }

    public void EndGame(bool gameWon)
    {
        GameEnded = true;
    }
}
