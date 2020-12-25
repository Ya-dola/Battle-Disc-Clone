using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public bool GameStarted { get; private set; }
    public bool GameEnded { get; private set; }
    public bool GamePaused { get; private set; }
    
    public bool PlayerDiscCaught { get; private set; }
    public bool EnemyDiscCaught { get; private set; }
    public bool DiscCollidedOnce { get; private set; }
    public bool PlayerRepositionDisc { get; private set; }
    public bool EnemyRepositionDisc { get; private set; }

    public Vector3 lastEnemyPos { get; set; }

    [Header("Player")]
    [Range(0, 0.2f)]
    public float playerDragSpeed;
    public Vector3 lastPlayerPos { get; set; }

    public Vector3 playerStartingPos;
    public GameObject Player { get; set; }
    public GameObject launchIndicator;

    [Header("Enemy")]
    public float enemyMoveSpeed;
    public float enemyLaunchVariance;

    [Header("Enemy Time")]
    public float enemyIdleTime;
    public float enemyLaunchDelayTime;

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
    public GameObject[] enemyPositions { get; set; }

    [Header("Disc")]
    public GameObject discPrefab;

    public GameObject discBrokenPrefab;
    public Vector3 discStartingPos;
    public GameObject Disc { get; set; }
    [Range(0, 48f)]
    public float discSpeed;
    [Range(0, 12f)]
    public float discSpeedDiff;
    public float discRepositionSpeed;
    public int bounceRandIterator;
    [Range(0, 1)]
    public float bounceBias;
    public int bounceCount { get; set; }
    [Range(0.4f, 1f)]
    public float discRepositionZDistance;

    [Header("Camera")]
    public float camFovStart;
    public float camFovEnd;
    public float camFovTransitionSpeed;
    private bool camStartTransition;

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
    public Material enemyMaterial { get; set; }

    [Header("Level Management")]
    public string baseSceneName;
    private int sceneCounter;
    private int nextSceneInt;
    private int tempNextSceneInt;
    private int currentSceneInt;
    private AsyncOperation sceneLoader;

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Debug")]
    public TextMeshProUGUI debugText;

    void Awake()
    {
        // Creates a Single Instance of the Game Manager through out the entire game
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setting Default Values
        camStartTransition = false;
        sceneCounter = 0;

        LoadNextScene();
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text =
                          //  "Game Started: " + GameStarted + "\n" +
                          "Player Disc Caught: " + PlayerDiscCaught + "\n" +
                          "Player Reposition Disc: " + PlayerRepositionDisc + "\n" +
                          "Disc Collided Once: " + DiscCollidedOnce + "\n" +
                          "Enemy Disc Caught: " + EnemyDiscCaught + "\n" +
                          "Enemy Reposition Disc: " + EnemyRepositionDisc + "\n"
                         //  "Game Ended: " + GameEnded + "\n" +
                         //  "Enemy State: " + enemyState + "\n" +
                         //  "Enemy Dest Size: " + GameObject.FindGameObjectsWithTag("Enemy Dest").Length + "\n" +
                         //  "Enemy Positions Length: " + enemyPositions.Length + "\n" +
                         //  "Enemy Position 0: " + enemyPositions[0].transform.position + "\n" +
                         //  "Scene Counter: " + sceneCounter + "\n" +
                         //  "Next Scene Int: " + nextSceneInt + "\n" +
                         //  "Temp Next Scene Int: " + tempNextSceneInt + "\n" +
                         //  "Current Scene Int: " + currentSceneInt + "\n"
                         //  "Bounce Count: " + bounceCount + "\n"
                         ;

        // Camera Transition before Starting the Game
        if (camStartTransition)
        {
            if (Camera.main.fieldOfView > camFovEnd)
                Camera.main.fieldOfView -= camFovTransitionSpeed * Time.fixedDeltaTime;
            else
            {
                Camera.main.fieldOfView = camFovEnd;
                camStartTransition = false;

                // Starting the Game if the Camera has transitioned to zoomed in FOV
                StartGame();
            }
        }

        // To check if the current level has finished
        if (!GameEnded)
            LevelProgress();
    }

    public void StartCameraTransition()
    {
        // Camera Transition before Starting the Game
        if (Camera.main.fieldOfView == camFovStart)
            camStartTransition = true;

        // Starting the Game if the Camera has transitioned to zoomed in FOV
        if (Camera.main.fieldOfView == camFovEnd)
            if (!GameStarted)
                StartGame();
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

        pauseMenu.SetActive(GamePaused);

        if (GamePaused)
            StopTime();

        else
            StartTime();
    }

    private void EndGame(bool gameWon)
    {
        GameEnded = true;
        GameStarted = false;

        Disc.GetComponent<MeshRenderer>().enabled = false;

        ShowDestroyedDisc();

        if (gameWon)
        {
            // To unload the current scene if the game has ended
            if (GameEnded)
                SceneManager.UnloadSceneAsync(string.Concat(baseSceneName + " ", currentSceneInt + 1));

            // Display Next Loaded Scene
            LoadNextScene();
        }
        else
        {
            Debug.Log("Game Lost");
        }
    }

    private void LoadNextScene()
    {
        if (sceneCounter != 0)
            sceneLoader.allowSceneActivation = true;

        currentSceneInt = nextSceneInt;

        GameEnded = false;

        // To Initialise the Game Object's Materials according to their tag
        InitialiseGameObjMaterials();

        // To Initialise the Active Disc as the Disc for the Scene
        InitialiseSceneDisc();

        // To Initialise the Active Disc as the Player for the Scene
        InitialiseScenePlayer();

        // To Initialise the Active Enemy Positions for the Scene
        InitialiseSceneEnemyPositions();

        // To Determine the next scene's number
        if (sceneCounter < 4)
        {
            sceneCounter++;
            nextSceneInt = sceneCounter;

            BackgroundLoadNextScene();
        }
        else
        {
            do
            {
                tempNextSceneInt = Random.Range(0, 5);
            } while (tempNextSceneInt == currentSceneInt);

            sceneCounter++;
            nextSceneInt = tempNextSceneInt;

            BackgroundLoadNextScene();
        }
    }

    private void BackgroundLoadNextScene()
    {
        sceneLoader = SceneManager.LoadSceneAsync(string.Concat(baseSceneName + " ", nextSceneInt + 1), LoadSceneMode.Additive);
        sceneLoader.allowSceneActivation = false;
    }

    private void InitialiseGameObjMaterials()
    {
        // To choose the enemy material for the round according to the level number
        enemyMaterial = enemyMaterials[currentSceneInt];
        // enemyMaterial = enemyMaterials[Random.Range(0, enemyMaterials.Length)];

        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Player Dest"))
            gameObj.GetComponent<Renderer>().material = playerMaterial;

        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Enemy"))
            gameObj.GetComponentInChildren<Renderer>().material = enemyMaterial;
    }

    private void InitialiseSceneDisc()
    {
        if (GameObject.FindGameObjectWithTag("Player Disc") != null)
            Disc = GameObject.FindGameObjectWithTag("Player Disc");
        else if (GameObject.FindGameObjectWithTag("Enemy Disc") != null)
            Disc = GameObject.FindGameObjectWithTag("Enemy Disc");

        // Setting the Defaults for the Disc before the Next Scene Starts
        if (sceneCounter != 0)
        {
            Disc.tag = "Player Disc";
            Disc.transform.position = discStartingPos;
            Disc.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Disc.GetComponent<MeshRenderer>().enabled = true;
            SetPlayerDiscCaught(true);
            SetDiscCollidedOnce(false);
        }
    }

    private void InitialiseScenePlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
            Player = GameObject.FindGameObjectWithTag("Player");

        Player.transform.position = playerStartingPos;
        launchIndicator.SetActive(true);
    }

    private void InitialiseSceneEnemyPositions()
    {
        enemyPositions = GameObject.FindGameObjectsWithTag("Enemy Position");
    }

    private void LevelProgress()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy Dest").Length == 0)
            EndGame(true);
        if (GameObject.FindGameObjectsWithTag("Player Dest").Length == 0)
            EndGame(false);
    }

    private void ShowDestroyedDisc()
    {
        // To only break the disc if the game ended
        if (GameManager.singleton.GameEnded)
        {
            GameObject discBroken = Instantiate(discBrokenPrefab, Disc.transform.position, Disc.transform.rotation);

            for (int i = 0; i < discBroken.transform.childCount; i++)
            {
                // Assigning the Disc's Material to the Broken Parts of the Disc
                discBroken.transform.GetChild(i).GetComponent<Renderer>().material = Disc.GetComponent<Renderer>().material;

                // Giving an Upwards Explosive force to the Broken Parts
                discBroken.transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(GameManager.singleton.destExplosionForce,
                                                                                             Disc.transform.position,
                                                                                             GameManager.singleton.destExplosionRadius,
                                                                                             GameManager.singleton.destExplosionUpwardsMod);
            }

            Destroy(discBroken, GameManager.singleton.destBrokenDelay);
        }
    }

    private void StopTime()
    {
        Time.timeScale = 0f;
    }

    private void StartTime()
    {
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Base Scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        // Written to show as Application.Quit doesnt do anything in Editor
        // Debug.Log("Quit the Game !!!");

        Application.Quit();
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
