using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public bool GameStarted { get; private set; }
    public bool GameEnded { get; private set; }
    public bool GamePaused { get; private set; }
    // public bool GameWon { get; private set; }
    // public bool GameLost { get; private set; }

    public bool DiscCaught { get; private set; }
    public bool DiscCollidedOnce { get; private set; }
    public bool RepositionDisc { get; private set; }

    [Header("Player")]
    [Range(0, 1)]
    public float playerDragSpeed;

    [Header("Disc")]
    // [Range(0, 100)]
    public float discForce;
    public float discLerpMoveTime;

    [Range(0.4f, 1f)]
    public float discRepositionZDistance;

    [Header("Arena")]
    public float sideEdgeDistance;
    public float topBotEdgeDistance;
    public float topBotCenterDistance;

    [Header("Destructables")]
    public float destBrokenDelay;
    public float destExplosionForce;
    public float destExplosionRadius;
    public float destExplosionUpwardsMod;

    [Header("Debug")]
    public TextMeshProUGUI debugText;

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
        debugText.text = "Game Started: " + GameStarted + "\n" +
                         "Disc Caught: " + DiscCaught + "\n" +
                         "Disc Collided Once: " + DiscCollidedOnce + "\n" +
                         "Reposition Disc: " + RepositionDisc + "\n";
    }

    public void StartGame()
    {
        GameStarted = true;

        // Starting Conditions of the Disc
        setDiscCaught(true);
        setDiscCollidedOnce(false);
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

    // Setters
    public void setDiscCaught(bool status)
    {
        DiscCaught = status;
    }

    public void setDiscCollidedOnce(bool status)
    {
        DiscCollidedOnce = status;
    }

    public void setRepositionDisc(bool status)
    {
        RepositionDisc = status;
    }
}
