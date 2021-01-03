using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscLineController : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;

    private LineRenderer discLineRenderer;
    private float charDistance;
    private float lineWidth;

    void Awake()
    {
        discLineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Runs only when the disc line is active
        if (!gameObject.activeSelf)
            return;

        discLineRenderer.startWidth = 0.5f;

        // To Update the Line with the Position of the Disc
        discLineRenderer.SetPosition(0, GameManager.singleton.Disc.transform.position);

        // To Update the Line with the Position of the Player when caught
        if (GameManager.singleton.PlayerDiscCaught)
        {
            discLineRenderer.SetPosition(1, Player.transform.position);

            charDistance = Vector3.Distance(GameManager.singleton.Disc.transform.position,
                                            Player.transform.position);
        }

        // To Update the Line with the Position of the Enemy when caught
        if (GameManager.singleton.EnemyDiscCaught)
        {
            discLineRenderer.SetPosition(1, Enemy.transform.position);

            charDistance = Vector3.Distance(GameManager.singleton.Disc.transform.position,
                                            Player.transform.position);
        }

        // To give the stretchy feeling to the line - Width is Inversly Proportional to Distance
        lineWidth = GameManager.singleton.discLineWidthInverseConst / charDistance;

        if (lineWidth > GameManager.singleton.discLineMinWidth &&
            lineWidth < GameManager.singleton.discLineMaxWidth)
            discLineRenderer.startWidth = lineWidth;
        else if (lineWidth < GameManager.singleton.discLineMinWidth)
            discLineRenderer.startWidth = GameManager.singleton.discLineMinWidth;
        else
            discLineRenderer.startWidth = GameManager.singleton.discLineMaxWidth;
    }
}
