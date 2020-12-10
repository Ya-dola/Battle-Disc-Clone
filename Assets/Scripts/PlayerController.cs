using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // private Touch screenTouch;
    // public float playerMoveSpeed;
    public float playerMoveForce;

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

        // Move the player ball according to the mouse when pressed
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

            // Vector3 force = new Vector3(deltaMousePos.x, 0, deltaMousePos.y) * playerMoveForce;
            // playerRigBody.AddForce(force);

            transform.position = new Vector3(
                    transform.position.x + deltaMousePos.x * playerMoveForce,
                    transform.position.y,
                    transform.position.z + deltaMousePos.y * playerMoveForce
                );
        }
        else
            lastMousePos = Vector2.zero;
    }
}

