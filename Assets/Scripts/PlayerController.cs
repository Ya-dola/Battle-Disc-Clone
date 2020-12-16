using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // private Touch screenTouch;
    // public float playerMoveSpeed;


    private Rigidbody playerRigBody;
    // private Animator playerAnimator;
    private Vector2 lastMousePos;

    void Awake()
    {
        playerRigBody = GetComponent<Rigidbody>();
        // playerAnimator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DragPlayer();
    }

    // Late Update used mainly for Camera Calculations and Calculations that need to occur after movement has occured
    // Occurs after physics is applied 
    private void LateUpdate()
    {
        ConstraintPosition(gameObject, "Player");

    }

    private void DragPlayer()
    {
        // float planeY = 0;
        // Transform draggingObject = transform;

        // Plane plane = new Plane(Vector3.up, Vector3.up * planeY); // ground plane

        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // float distance; // the distance from the ray origin to the ray intersection of the plane
        // if (plane.Raycast(ray, out distance))
        // {
        //     draggingObject.position = ray.GetPoint(distance); // distance along the ray
        // }

        // if (Input.touchCount > 0)
        // {
        //     screenTouch = Input.GetTouch(0);

        //     if (screenTouch.phase == TouchPhase.Moved)
        //     {
        //         transform.position = new Vector3(
        //             transform.position.x + screenTouch.deltaPosition.x * playerMoveSpeed,
        //             transform.position.y,
        //             transform.position.z + screenTouch.deltaPosition.y * playerMoveSpeed
        //         );
        //     }
        // }

        // Drag the Player accordingly when the mouse is pressed down
        Vector2 deltaMousePos = Vector2.zero;

        if (Input.GetMouseButton(0))
        {
            // Signals the Game has started and only runs it once if the game has already started
            // if (!GameManager.singleton.GameStarted)
            //     GameManager.singleton.StartGame();

            Vector2 currentMousePos = Input.mousePosition;

            if (lastMousePos == Vector2.zero)
                lastMousePos = currentMousePos;

            deltaMousePos = currentMousePos - lastMousePos;
            lastMousePos = currentMousePos;

            transform.position = new Vector3(
                    transform.position.x + deltaMousePos.x * GameManager.singleton.playerDragSpeed,
                    transform.position.y,
                    transform.position.z + deltaMousePos.y * GameManager.singleton.playerDragSpeed
                );
        }
        else
            lastMousePos = Vector2.zero;
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
}

