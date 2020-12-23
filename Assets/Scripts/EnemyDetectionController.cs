using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionController : MonoBehaviour
{
    public GameObject Disc;
    private SphereCollider detectionCollider;

    void Awake()
    {
        detectionCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        detectionCollider.radius = GameManager.singleton.enemyDetectionRadius;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        // To check if the disc is within the enemy detection area or not
        if (collider.gameObject.Equals(Disc))
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.ChasingDisc;
        else
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Roaming;
    }

    void OnTriggerExit(Collider collider)
    {
        // To indicate that the disc left the detection area of the enemy
        if (collider.gameObject.Equals(Disc))
            GameManager.singleton.enemyState = GameManager.EnemyStateEnum.Roaming;
    }
}
