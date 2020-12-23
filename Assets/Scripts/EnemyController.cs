using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // private Animator enemyAnimator;
    public GameObject Disc;
    private Vector3 discRepositionedPos;

    [Header("Enemy AI Movement")]
    public GameObject[] enemyPositions;

    private int positionIndex;
    private float idleTimer;

    void Awake()
    {
        // enemyAnimator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        positionIndex = 0;
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
        // To work only when the Game has started
        if (!GameManager.singleton.GameStarted)
            return;

        MoveEnemy();

        // LaunchDisc();

        // To reposition the Disc behind the Enemy when Caught
        if (GameManager.singleton.EnemyRepositionDisc)
            EnemyRepositionDisc();
    }

    // Late Update used mainly for Camera Calculations and Calculations that need to occur after movement has occured
    // Occurs after physics is applied 
    private void LateUpdate()
    {
        ConstraintPosition(gameObject, "Enemy");
    }

    // To Update the Position of Character with the Constraints
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

    private void MoveEnemy()
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
                // case GameManager.EnemyStateEnum.CaughtDisc:
                //     CaughtDiscState();
                //     break;
        }
    }

    private void IdleState()
    {
        if (idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
        }
        else
        {
            if (positionIndex < enemyPositions.Length)
            {
                // To check if the distance between the enemy and it's current target position is less than the position's radius
                if (Vector3.Distance(enemyPositions[positionIndex].transform.position, gameObject.transform.position) < GameManager.singleton.enemyPositionRadius)
                {
                    if (positionIndex + 1 == enemyPositions.Length)
                        positionIndex = 0;
                    else
                        positionIndex++;
                }
            }

            // To reset the timer
            idleTimer = 0;

            // To move to the next roaming position
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Roaming;
        }
    }

    private void RoamingState()
    {
        // Moving the Enemy from one position to the other
        transform.position = Vector3.MoveTowards(transform.position,
                                                 enemyPositions[positionIndex].transform.position,
                                                 GameManager.singleton.enemyMoveSpeed * Time.deltaTime);

        // To check if the distance between the enemy and it's current target position is less than the position's radius
        if (Vector3.Distance(transform.position, enemyPositions[positionIndex].transform.position) < GameManager.singleton.enemyPositionRadius)
        {
            // To start the idle timer and transition the enemy to the idle state
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Idle;
            idleTimer = GameManager.singleton.enemyIdleTime;
        }
    }

    private void ChasingDiscState()
    {
        // Moving the Enemy from one position to the other
        transform.position = Vector3.MoveTowards(transform.position,
                                                 Disc.transform.position,
                                                 GameManager.singleton.enemyMoveSpeed * Time.deltaTime);

    }

    private void LaunchDisc()
    {
        // To work only when the Game has started
        if (!GameManager.singleton.GameStarted)
            return;

        // To only run when the disc is caught and has already collided once with something
        if (Input.GetKeyUp(KeyCode.Mouse0) && GameManager.singleton.EnemyDiscCaught)
        {
            Disc.GetComponent<Rigidbody>().velocity = Vector3.Normalize(transform.position - Disc.transform.position) *
                                                        GameManager.singleton.discSpeed;

            Disc.layer = LayerMask.NameToLayer("Disc Launched");

            // To reset disc conditions
            GameManager.singleton.SetEnemyDiscCaught(false);
            GameManager.singleton.SetDiscCollidedOnce(false);
            GameManager.singleton.SetEnemyRepositionDisc(false);
            GameManager.singleton.bounceCount = 0;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Catching the Disc
        if (collider.gameObject.Equals(Disc))
        {
            GameManager.singleton.SetEnemyDiscCaught(true);

            // To Stop the Disc on Collision with the Enemy
            Disc.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // To indicate that the disc was last caught by the enemy
            Disc.tag = "Enemy Disc";

            // To reposition the disc on collision
            GameManager.singleton.lastEnemyPos = transform.position;

            if (GameManager.singleton.DiscCollidedOnce)
            {
                GameManager.singleton.SetEnemyRepositionDisc(true);
                GameManager.singleton.SetDiscCollidedOnce(false);
            }
        }
    }

    private void EnemyRepositionDisc()
    {
        discRepositionedPos = new Vector3(GameManager.singleton.lastEnemyPos.x,
                                          GameManager.singleton.lastEnemyPos.y,
                                          GameManager.singleton.lastEnemyPos.z + GameManager.singleton.discRepositionZDistance);

        // To reposition the Disc behind the Enemy when Caught
        Disc.transform.position = Vector3.Lerp(Disc.transform.position,
                                                discRepositionedPos,
                                                GameManager.singleton.discLerpMoveTime * Time.deltaTime);

        // To indicate the disc has repositioned
        if (Disc.transform.position == discRepositionedPos)
            GameManager.singleton.SetEnemyRepositionDisc(false);
    }
}
