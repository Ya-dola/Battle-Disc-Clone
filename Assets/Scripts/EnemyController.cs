﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // private Animator enemyAnimator;
    public GameObject Disc;
    private Vector3 discRepositionedPos;
    private NavMeshAgent enemyNavMeshAgent;

    [Header("Enemy AI Movement")]
    public GameObject[] enemyPositions;

    private int positionIndex;

    void Awake()
    {
        // enemyAnimator = GetComponentInChildren<Animator>();
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        positionIndex = 0;
    }

    // Fixed Update used mainly for Physics Calculations
    void FixedUpdate()
    {
        // LaunchDisc();
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();

        // LaunchDisc();

        // // To reposition the Disc behind the Enemy when Caught
        // if (GameManager.singleton.RepositionDisc)
        //     RepositionDisc();
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
        // To work only when the Game has started
        if (!GameManager.singleton.GameStarted)
            return;

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

        // To Set the Destination of the Enemy
        enemyNavMeshAgent.SetDestination(enemyPositions[positionIndex].transform.position);
    }

    private void LaunchDisc()
    {
        // To work only when the Game has started
        if (!GameManager.singleton.GameStarted)
            return;

        // To only run when the disc is caught and has already collided once with something
        if (Input.GetKeyUp(KeyCode.Mouse0) && GameManager.singleton.DiscCaught)
        {
            Disc.GetComponent<Rigidbody>().velocity = Vector3.Normalize(transform.position - Disc.transform.position) *
                                                        GameManager.singleton.discSpeed;

            Disc.layer = LayerMask.NameToLayer("Disc Launched");

            // To reset disc conditions
            GameManager.singleton.SetDiscCaught(false);
            GameManager.singleton.SetDiscCollidedOnce(false);
            GameManager.singleton.SetRepositionDisc(false);
            GameManager.singleton.bounceCount = 0;
        }
    }

    // private void OnTriggerEnter(Collider collider)
    // {
    //     // Catching the Disc
    //     if (collider.gameObject.Equals(Disc))
    //     {
    //         GameManager.singleton.SetDiscCaught(true);

    //         // To Stop the Disc on Collision with the Enemy
    //         Disc.GetComponent<Rigidbody>().velocity = Vector3.zero;

    //         // To indicate that the disc was last caught by the enemy
    //         Disc.tag = "Enemy Disc";

    //         // To reposition the disc on collision
    //         GameManager.singleton.lastEnemyPos = transform.position;

    //         if (GameManager.singleton.DiscCollidedOnce)
    //         {
    //             GameManager.singleton.SetRepositionDisc(true);
    //             GameManager.singleton.SetDiscCollidedOnce(false);
    //         }
    //     }
    // }

    // TODO - Fix Bug of Destroying Destructable when repositioning
    private void RepositionDisc()
    {
        discRepositionedPos = new Vector3(GameManager.singleton.lastEnemyPos.x,
                                          GameManager.singleton.lastEnemyPos.y,
                                          GameManager.singleton.lastEnemyPos.z - GameManager.singleton.discRepositionZDistance);

        // To reposition the Disc behind the Enemy when Caught
        Disc.transform.position = Vector3.Lerp(Disc.transform.position,
                                                discRepositionedPos,
                                                GameManager.singleton.discLerpMoveTime * Time.deltaTime);

        // To indicate the disc has repositioned
        if (Disc.transform.position == discRepositionedPos)
            GameManager.singleton.SetRepositionDisc(false);
    }
}
