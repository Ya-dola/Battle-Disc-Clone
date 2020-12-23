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

    public bool PlayerDiscCaught { get; private set; }
    public bool EnemyDiscCaught { get; private set; }
    public bool DiscCollidedOnce { get; private set; }
    public bool PlayerRepositionDisc { get; private set; }
    public bool EnemyRepositionDisc { get; private set; }

    public Vector3 lastEnemyPos { get; set; }

    [Header("Player")]
    [Range(0, 1)]
    public float playerDragSpeed;
    public Vector3 lastPlayerPos { get; set; }

    [Header("Enemy")]
    public float enemyIdleTime;
    public float enemyMoveSpeed;

    [Header("Enemy Radii")]
    public float enemyPositionRadius;
    public float enemyDetectionRadius;
    [HideInInspector]
    public enum EnemyStateEnum
    {
        Idle,
        Roaming,
        ChasingDisc,
        CaughtDisc
    }
    public EnemyStateEnum enemyState { get; set; }

    [Header("Disc")]
    // [Range(0, 100)]
    public float discSpeed;
    public float discLerpMoveTime;
    public int bounceRandIterator;
    [Range(0, 1)]
    public float bounceBias;
    public int bounceCount { get; set; }

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

    [Header("Materials")]
    public Material playerMaterial;
    public Material[] enemyMaterials;
    private Material enemyMaterial;

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
        // To Assign the Game Object's Materials according to their tag
        AssignGameObjMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text =
                         // "Game Started: " + GameStarted + "\n" +
                         "Player Disc Caught: " + PlayerDiscCaught + "\n" +
                         "Player Reposition Disc: " + PlayerRepositionDisc + "\n" +
                         "Disc Collided Once: " + DiscCollidedOnce + "\n" +
                         "Enemy Disc Caught: " + EnemyDiscCaught + "\n" +
                         "Enemy Reposition Disc: " + EnemyRepositionDisc + "\n" +
                         "Enemy State: " + enemyState + "\n"
                         //  "Bounce Count: " + bounceCount + "\n"
                         ;
    }

    public void StartGame()
    {
        GameStarted = true;

        // Starting Conditions of the Disc
        SetPlayerDiscCaught(true);
        SetDiscCollidedOnce(false);
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

    private void AssignGameObjMaterials()
    {
        // To choose the enemy material for the round
        enemyMaterial = enemyMaterials[Random.Range(0, enemyMaterials.Length)];
        // enemyMaterial = enemyMaterials[0];

        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Player Dest"))
            gameObj.GetComponent<Renderer>().material = playerMaterial;

        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Enemy Dest"))
            gameObj.GetComponent<Renderer>().material = enemyMaterial;

        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Enemy"))
            gameObj.GetComponentInChildren<Renderer>().material = enemyMaterial;
    }

    // Setters
    public void SetPlayerDiscCaught(bool status)
    {
        PlayerDiscCaught = status;
    }
    public void SetEnemyDiscCaught(bool status)
    {
        EnemyDiscCaught = status;
    }

    public void SetDiscCollidedOnce(bool status)
    {
        DiscCollidedOnce = status;
    }

    public void SetPlayerRepositionDisc(bool status)
    {
        PlayerRepositionDisc = status;
    }
    public void SetEnemyRepositionDisc(bool status)
    {
        EnemyRepositionDisc = status;
    }
}
