using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator enemyAnimator;
    private Vector3 discRepositionedPos;

    private int positionIndex;
    private int tempPositionIndex;
    private float idleTimer;
    private float launchTimer;
    private Vector3 launchPos;
    private bool launchPosDecided;

    void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setting Default Values
        positionIndex = 0;
        idleTimer = 0;
        launchTimer = 0;
        launchPosDecided = false;
        GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Roaming;
    }

    // Fixed Update used mainly for Physics Calculations
    void FixedUpdate()
    {
        // LaunchDisc();
    }

    // Update is called once per frame
    void Update()
    {
        // To perform Updates on which animation should be playing for the Enemy
        AnimationUpdates();

        // To work only when the Game has started or has not ended or is not paused
        if (!GameManager.singleton.GameStarted || GameManager.singleton.GameEnded || GameManager.singleton.GamePaused)
            return;

        // Enemy AI Logic with regards to movement of the enemy
        EnemyLogic();

        // To reposition the Disc behind the Enemy when Caught
        if (GameManager.singleton.EnemyRepositionDisc)
            EnemyRepositionDisc();
    }

    // Late Update used mainly for Camera Calculations and Calculations that need to occur after movement has occured
    // Occurs after physics is applied 
    private void LateUpdate()
    {
        // To Update the Position of the Enemy with the Constraints
        ConstraintPosition(gameObject, "Enemy");
    }

    // To Update the Position of the Character with the Constraints
    private void ConstraintPosition(GameObject goCharacter, string character)
    {
        Vector3 goCharacterOldPos = goCharacter.transform.position;

        if (character == "Player")
        {
            // To make the Character not fall from the Arena through the Sides
            if (transform.position.x < -GameManager.singleton.sideEdgeDistance)
                goCharacterOldPos.x = -GameManager.singleton.sideEdgeDistance;
            else if (transform.position.x > GameManager.singleton.sideEdgeDistance)
                goCharacterOldPos.x = GameManager.singleton.sideEdgeDistance;

            // To make the Character not be able to leave the Arena through the Bottom
            if (transform.position.z < -GameManager.singleton.topBotEdgeDistance)
                goCharacterOldPos.z = -GameManager.singleton.topBotEdgeDistance;

            // To make the Character not be able to leave their half of the Arena
            if (transform.position.z > -GameManager.singleton.topBotCenterDistance)
                goCharacterOldPos.z = -GameManager.singleton.topBotCenterDistance;
        }
        else
        {
            // To make the Character not fall from the Arena through the Sides
            if (transform.position.x < -GameManager.singleton.sideEdgeDistance)
                goCharacterOldPos.x = -GameManager.singleton.sideEdgeDistance;
            else if (transform.position.x > GameManager.singleton.sideEdgeDistance)
                goCharacterOldPos.x = GameManager.singleton.sideEdgeDistance;

            // To make the Character not be able to leave the Arena through the Top
            if (transform.position.z > GameManager.singleton.topBotEdgeDistance)
                goCharacterOldPos.z = GameManager.singleton.topBotEdgeDistance;

            // To make the Character not be able to leave their half of the Arena
            if (transform.position.z < GameManager.singleton.topBotCenterDistance)
                goCharacterOldPos.z = GameManager.singleton.topBotCenterDistance;
        }

        goCharacter.transform.position = goCharacterOldPos;
    }

    private void EnemyLogic()
    {
        switch (GameManager.singleton.enemyState)
        {
            default:
            case GameManager.EnemyStateEnum.Idle:
                IdleState();
                break;

            case GameManager.EnemyStateEnum.Roaming:
                RoamingState();
                break;

            case GameManager.EnemyStateEnum.ChasingDisc:
                ChasingDiscState();
                break;

            case GameManager.EnemyStateEnum.CaughtDisc:
                CaughtDiscState();
                break;
        }

        if (GameManager.singleton.EnemyDiscCaught)
            // To Transition the Enemy to the CaughtDisc State
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.CaughtDisc;
    }

    private void IdleState()
    {
        if (idleTimer > 0)
            idleTimer -= Time.fixedDeltaTime;
        else
        {
            // To get the next position index randomly without staying at the same position again
            tempPositionIndex = Random.Range(0, GameManager.singleton.enemyPositions.Length);

            if (tempPositionIndex != positionIndex)
            {
                positionIndex = tempPositionIndex;

                // To reset the Idle timer
                idleTimer = 0;

                // To transition the Enemy to the Roaming state
                GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Roaming;
            }
        }
    }

    private void RoamingState()
    {
        // Moving the Enemy from it's current position to the other
        transform.position = Vector3.MoveTowards(transform.position,
                                                 GameManager.singleton.enemyPositions[positionIndex].transform.position,
                                                 GameManager.singleton.enemyMoveSpeed * Time.fixedDeltaTime);

        // To check if the distance between the enemy and it's current target position is less than the position's radius
        if (Vector3.Distance(transform.position,
                             GameManager.singleton.enemyPositions[positionIndex].transform.position) < GameManager.singleton.enemyPositionRadius)
        {
            // To start the idle timer and transition the Enemy to the Idle state
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Idle;
            idleTimer = GameManager.singleton.enemyIdleTime;
        }
    }

    private void ChasingDiscState()
    {
        if (!GameManager.singleton.EnemyDiscCaught)
            // Moving the Enemy from it's current position to the disc's position
            transform.position = Vector3.MoveTowards(transform.position,
                                                     GameManager.singleton.Disc.transform.position,
                                                     GameManager.singleton.enemyMoveSpeed * Time.fixedDeltaTime);
        else
            // To Transition the Enemy to the CaughtDisc State
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.CaughtDisc;

    }
    private void CaughtDiscState()
    {
        if (!launchPosDecided)
        {
            // Determining the Launch Position of the Enemy
            launchPos = new Vector3(transform.position.x + Random.Range(-GameManager.singleton.enemyLaunchVariance,
                                                                        GameManager.singleton.enemyLaunchVariance),
                                    transform.position.y,
                                    transform.position.z + Random.Range(0f, -GameManager.singleton.enemyLaunchVariance));

            launchPosDecided = true;
            launchTimer = GameManager.singleton.enemyLaunchDelayTime;
        }

        // Moving the Enemy from it's current position to the it's launching position
        transform.position = Vector3.MoveTowards(transform.position,
                                                 launchPos,
                                                 GameManager.singleton.enemyMoveSpeed * Time.fixedDeltaTime);

        if (launchTimer > 0)
            launchTimer -= Time.fixedDeltaTime;
        else
        {
            GameManager.singleton.Disc.GetComponent<Rigidbody>().velocity = Vector3.Normalize(transform.position -
                                                                                              GameManager.singleton.Disc.transform.position) *
                                                                            GameManager.singleton.discSpeed;

            GameManager.singleton.Disc.layer = LayerMask.NameToLayer("Disc Launched");

            // To Display the Effect when the Disc is Launched by the Enemy
            GameManager.singleton.ShowDiscFadeEffect(GameManager.singleton.enemyColor);

            enemyAnimator.SetBool("DiscLaunched", true);

            // To reset disc conditions
            GameManager.singleton.SetEnemyDiscCaught(false);
            GameManager.singleton.SetDiscCollidedOnce(false);
            GameManager.singleton.SetEnemyRepositionDisc(false);
            GameManager.singleton.bounceCount = 0;

            // To reset the Enemy CaughtDisc conditions
            launchPosDecided = false;
            launchTimer = 0;

            // To transition the Enemy to the Roaming state
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Roaming;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // Catching the Disc
        if (collider.gameObject.Equals(GameManager.singleton.Disc))
        {
            GameManager.singleton.SetEnemyDiscCaught(true);

            // To Transition the Enemy to the CaughtDisc State
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.CaughtDisc;

            // To Stop the Disc on Collision with the Enemy
            GameManager.singleton.Disc.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // To indicate that the disc was last caught by the Enemy
            GameManager.singleton.Disc.tag = "Enemy Disc";
            GameManager.singleton.Disc.gameObject.GetComponent<Renderer>().material = GameManager.singleton.enemyMaterial;
            GameManager.singleton.Disc.gameObject.GetComponentInChildren<TrailRenderer>().material = GameManager.singleton.enemyMaterial;

            // To Display the Effects when the Disc is Caught by the Enemy
            GameManager.singleton.ShowDiscFadeEffect(GameManager.singleton.enemyColor);
            GameManager.singleton.Disc.gameObject.GetComponentInChildren<ParticleSystem>().Play();

            // To reposition the disc on collision with the Enemy
            GameManager.singleton.lastEnemyPos = transform.position;

            // To reposition the disc
            GameManager.singleton.SetEnemyRepositionDisc(true);
            GameManager.singleton.SetDiscCollidedOnce(false);
        }
    }

    private void EnemyRepositionDisc()
    {
        discRepositionedPos = new Vector3(GameManager.singleton.lastEnemyPos.x,
                                          GameManager.singleton.lastEnemyPos.y,
                                          GameManager.singleton.lastEnemyPos.z + GameManager.singleton.discRepositionZDistance);

        // To reposition the Disc behind the Enemy when Caught
        GameManager.singleton.Disc.transform.position = Vector3.Lerp(GameManager.singleton.Disc.transform.position,
                                                                     discRepositionedPos,
                                                                     GameManager.singleton.discRepositionSpeed * Time.fixedDeltaTime);

        // To indicate the disc has repositioned
        if (GameManager.singleton.Disc.transform.position == discRepositionedPos)
            GameManager.singleton.SetEnemyRepositionDisc(false);
    }

    // To perform Updates on which animation should be playing for the Player
    private void AnimationUpdates()
    {
        if (!enemyAnimator.GetBool("GameStarted"))
            enemyAnimator.SetBool("GameStarted", GameManager.singleton.GameStarted);

        if (enemyAnimator.GetBool("DiscLaunched") && !GameManager.singleton.EnemyDiscCaught)
            enemyAnimator.SetBool("DiscLaunched", false);

        if (GameManager.singleton.enemyState == GameManager.EnemyStateEnum.Idle)
            enemyAnimator.SetBool("CharacterMoving", false);

        if (GameManager.singleton.enemyState == GameManager.EnemyStateEnum.ChasingDisc ||
            GameManager.singleton.enemyState == GameManager.EnemyStateEnum.Roaming)
            enemyAnimator.SetBool("CharacterMoving", true);

    }
}
