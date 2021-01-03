using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscLineController : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;

    private LineRenderer discLineRenderer;

    void Awake()
    {
        discLineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Runs only when the disc line is active
        if (gameObject.activeSelf)
        {
            // To Update the Line with the Position of the Disc
            discLineRenderer.SetPosition(0, GameManager.singleton.Disc.transform.position);

            // To Update the Line with the Position of the Player when caught
            if (GameManager.singleton.PlayerDiscCaught)
                discLineRenderer.SetPosition(1, Player.transform.position);

            // To Update the Line with the Position of the Enemy when caught
            if (GameManager.singleton.EnemyDiscCaught)
                discLineRenderer.SetPosition(1, Enemy.transform.position);
        }
    }
}
