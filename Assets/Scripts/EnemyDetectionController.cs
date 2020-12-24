using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionController : MonoBehaviour
{
    private SphereCollider detectionCollider;

    void Awake()
    {
        detectionCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // To Set the size of the Detection Radius as the Scene Starts
        detectionCollider.radius = GameManager.singleton.enemyDetectionRadius;
    }

    void OnTriggerEnter(Collider collider)
    {
        // To check if the disc is within the enemy detection area or not
        if (collider.gameObject.Equals(GameManager.singleton.Disc))
            // To transition the Enemy to the ChasingDisc state
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.ChasingDisc;
    }

    void OnTriggerExit(Collider collider)
    {
        // To indicate that the disc left the detection area of the enemy
        if (collider.gameObject.Equals(GameManager.singleton.Disc))
            // To transition the Enemy to the Roaming state
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Roaming;
    }
}
