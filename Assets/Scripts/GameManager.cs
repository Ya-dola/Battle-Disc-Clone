﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public GameObject[] alivePlayerDests { get; set; }

    [Header("Player")]
    [Range(0, 0.05f)]
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
    [Range(0.4f, 1f)]
    public float discRepositionZDistance;
    public int bounceRandIterator;
    [Range(0, 1)]
    public float bounceBias;
    public int bounceCount { get; set; }

    [Header("Effects")]
    public GameObject discFadePrefab;
    public float discFadeDestDelay;
    public GameObject discCollisionFadePrefab;
    public float discColFadeDestDelay;
    public ParticleSystem trianglePs;
    public ParticleSystem playerLaunchPs;
    public ParticleSystem playerCharacterLaunchPs;
    public ParticleSystem enemyLaunchPs;
    public ParticleSystem enemyCharacterLaunchPs;
    public float launchPsSpeed;
    public float cameraShakeAmount;
    public float cameraShakeAmountStep { get; set; }
    public float cameraShakeDuration;
    public float cameraShakeDurationStep { get; set; }
    public float cameraShakeStepFactor;
    public GameObject discLine;
    public float discLineWidthInverseConst;
    public float discLineMinWidth;
    public float discLineMaxWidth;

    [Header("Camera")]
    public float camFovStart;
    public float camFovEnd;
    public float camFovTransitionSpeed;
    private bool camStartTransition;
    private Vector3 cameraPosition;

    [Header("Arena")]
    public float sideEdgeDistance;
    public float topBotEdgeDistance;
    public float topBotCenterDistance;

    [Header("Destructables")]
    public float destBrokenDelay;
    public float destExplosionForce;
    public float destExplosionRadius;
    public float destExplosionUpwardsMod;
    public AudioClip destBrokenSound;

    [Range(0, 1)]
    public float destBrokenSoundVolume;

    [Header("Materials")]
    public Material playerMaterial;
    public Material playerEffectsMaterial;
    public Color playerColor { get; set; }
    public Material[] enemyMaterials;
    public Material[] enemyEffectsMaterials;
    public Material enemyMaterial { get; set; }
    public Material enemyEffectsMaterial { get; set; }
    public Color enemyColor { get; set; }

    [Header("Level Management")]
    public string baseSceneName;
    private int sceneCounter;
    private int nextSceneInt;
    private int tempNextSceneInt;
    private int currentSceneInt;
    private AsyncOperation sceneLoader;

    [Header("Menus")]
    public GameObject slideToMoveTMP;
    public TextMeshProUGUI levelText;
    public GameObject pauseMenu;
    public Slider sensitivitySlider;
    public GameObject gameLostMenu;
    public GameObject pauseButton;

    [Header("Debug")]
    public TextMeshProUGUI debugText;

    void Awake()
    {
        // Creates a Single Instance of the Game Manager through out the entire game
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        // To Start Level 1 as the Base Scene Loads
        SceneManager.LoadScene("Level 1", LoadSceneMode.Additive);

        // To cap the Unity Game View Frame Rate when Maximized
#if UNITY_EDITOR
        // VSync must be disabled
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setting Default Values
        camStartTransition = false;
        sceneCounter = 0;
        sensitivitySlider.value = playerDragSpeed;
        cameraPosition = Camera.main.transform.position;
        cameraShakeAmountStep = cameraShakeAmount / cameraShakeStepFactor;
        cameraShakeDurationStep = cameraShakeDuration / cameraShakeStepFactor;

        // To load the next scene
        LoadNextScene();
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text =
                        // "Game Started: " + GameStarted + "\n" +
                        // "Game Ended: " + GameEnded + "\n" +
                        // "Game Paused: " + GamePaused + "\n" +
                        // "Player Disc Caught: " + PlayerDiscCaught + "\n" +
                        //   "Player Reposition Disc: " + PlayerRepositionDisc + "\n" +
                        // "Disc Collided Once: " + DiscCollidedOnce + "\n" +
                        // "Enemy Disc Caught: " + EnemyDiscCaught + "\n" +
                        "Time Scale: " + Time.timeScale + "\n" +
                        //   "Enemy Reposition Disc: " + EnemyRepositionDisc + "\n" +
                        //  "Enemy State: " + enemyState + "\n" +
                        //  "Enemy Dest Size: " + GameObject.FindGameObjectsWithTag("Enemy Dest").Length + "\n" +
                        //  "Enemy Positions Length: " + enemyPositions.Length + "\n" +
                        //  "Enemy Position 0: " + enemyPositions[0].transform.position + "\n" +
                        // "Scene Counter: " + sceneCounter + "\n" +
                        // "Next Scene Int: " + nextSceneInt + "\n" +
                        // "Temp Next Scene Int: " + tempNextSceneInt + "\n" +
                        // "Current Scene Int: " + currentSceneInt + "\n" +
                        //  "Bounce Count: " + bounceCount + "\n"
                        "Player Drag Speed: " + playerDragSpeed + "\n"
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

        // To Update the sensitivity of Player's Drag Speed
        playerDragSpeed = sensitivitySlider.value;

        // To Update the level indicator text with the corresponding level
        levelText.text = "Level " + sceneCounter;

        // To Control the Visibility of some UI Elements
        UiElementsVisibility();

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
        discLine.SetActive(true);
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
            gameLostMenu.SetActive(true);
            StopTime();
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
        SceneManager.LoadScene("Base Scene");

        StartTime();
    }

    public void QuitGame()
    {
        // Written to show as Application.Quit doesnt do anything in Editor
        // Debug.Log("Quit the Game !!!");

        Application.Quit();
    }

    private void LoadNextScene()
    {
        // To Load the Scene that was loaded in the background
        if (sceneCounter != 0)
            sceneLoader.allowSceneActivation = true;

        currentSceneInt = nextSceneInt;

        GameEnded = false;

        // To Initialise the Game Objects Materials according to their tag
        InitialiseGameObjMaterials();

        // To Initialise the Active Disc as the Disc for the Scene
        InitialiseSceneDisc();

        // To Initialise the Active Player for the Scene
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

            // To load the next scene in the background
            BackgroundLoadNextScene();
        }
    }

    // To load the next scene in the background
    private void BackgroundLoadNextScene()
    {
        sceneLoader = SceneManager.LoadSceneAsync(string.Concat(baseSceneName + " ", nextSceneInt + 1), LoadSceneMode.Additive);
        sceneLoader.allowSceneActivation = false;
    }

    private void InitialiseGameObjMaterials()
    {
        // To choose the enemy material for the round according to the level number
        enemyMaterial = enemyMaterials[currentSceneInt];
        enemyEffectsMaterial = enemyEffectsMaterials[currentSceneInt];

        // To get the colors of the Materials used in the round
        playerColor = playerMaterial.color;
        enemyColor = enemyMaterial.color;

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

    // To Display the Effect when the Disc is Caught or Launched by a Character
    public void ShowDiscFadeEffect(Color characterColor)
    {
        GameObject discFade = Instantiate(discFadePrefab, Disc.transform.position, Disc.transform.rotation);

        discFade.GetComponent<Animator>().Play("Disc Fade Animation");

        discFade.GetComponent<SpriteRenderer>().color = characterColor;

        Destroy(discFade, discFadeDestDelay);
    }

    // To Shake the Camera
    public void ShakeCamera(float camShakeDuration, float camShakeAmt)
    {
        // To Stop Any Existing Camera Shakes
        StopAllCoroutines();
        StartCoroutine(ShakeCameraIEnum(camShakeDuration, camShakeAmt));
    }

    private IEnumerator ShakeCameraIEnum(float camShakeDuration, float camShakeAmt)
    {
        while (camShakeDuration > 0)
        {
            Camera.main.transform.position = cameraPosition + (Random.insideUnitSphere * camShakeAmt);

            camShakeDuration -= cameraShakeDurationStep;
            camShakeAmt -= cameraShakeAmountStep;

            yield return null;
        }

        // To reset the Camera Position after Shaking the Camera
        Camera.main.transform.position = cameraPosition;
    }

    private void UiElementsVisibility()
    {
        // To Hide the Slide to Move Text if not at the Starting Scene
        if (sceneCounter > 1 || GameStarted)
            slideToMoveTMP.SetActive(false);

        // To Hide the Pause Button and Level Indicator Text when the Game is Paused or Lost
        if (pauseMenu.activeSelf || gameLostMenu.activeSelf)
        {
            pauseButton.SetActive(false);
            levelText.gameObject.SetActive(false);
        }
        else if (GameStarted)
        {
            pauseButton.SetActive(true);
            levelText.gameObject.SetActive(true);
        }
    }

    // To Refresh the Player Destructables that are still alive
    public void UpdateAlivePlayerDests()
    {
        alivePlayerDests = GameObject.FindGameObjectsWithTag("Player Dest");
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
