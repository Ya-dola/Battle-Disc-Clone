using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // private Touch screenTouch;
    // public float playerMoveSpeed;

    // private Rigidbody playerRigBody;
    // private Animator playerAnimator;
    private Vector2 lastMousePos;

    public GameObject LaunchIndicator;
    private Vector3 discRepositionedPos;

    void Awake()
    {
        // playerRigBody = GetComponent<Rigidbody>();
        // playerAnimator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Fixed Update used mainly for Physics Calculations
    void FixedUpdate()
    {
        // LaunchDisc();
    }

    // Update is called once per frame
    void Update()
    {
        DragPlayer();

        // To work only when the Game has started
        if (!GameManager.singleton.GameStarted)
            return;

        LaunchDisc();

        // To reposition the Disc behind the Player when Caught
        if (GameManager.singleton.PlayerRepositionDisc)
            PlayerRepositionDisc();
    }

    // Late Update used mainly for Camera Calculations and Calculations that need to occur after movement has occured
    // Occurs after physics is applied 
    private void LateUpdate()
    {
        // To Update the Position of the Player with the Constraints
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
            if (!GameManager.singleton.GameStarted)
                GameManager.singleton.StartGame();

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

    private void LaunchDisc()
    {
        // To only run when the disc is caught and has already collided once with any object besides the Player
        if (Input.GetKeyUp(KeyCode.Mouse0) && GameManager.singleton.PlayerDiscCaught)
        {
            GameManager.singleton.Disc.GetComponent<Rigidbody>().velocity = Vector3.Normalize(transform.position - GameManager.singleton.Disc.transform.position) *
                                                        GameManager.singleton.discSpeed;

            GameManager.singleton.Disc.layer = LayerMask.NameToLayer("Disc Launched");

            // To reset disc conditions
            GameManager.singleton.SetPlayerDiscCaught(false);
            GameManager.singleton.SetDiscCollidedOnce(false);
            GameManager.singleton.SetPlayerRepositionDisc(false);
            GameManager.singleton.bounceCount = 0;
            LaunchIndicator.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // Catching the Disc
        if (collider.gameObject.Equals(GameManager.singleton.Disc))
        {
            // To ensure that the Collision Effect does not occur more than once 
            if (GameManager.singleton.PlayerDiscCaught)
                return;

            // To indicate that the Player Caught the Disc
            GameManager.singleton.SetPlayerDiscCaught(true);
            LaunchIndicator.gameObject.SetActive(true);

            // To Stop the Disc on Collision with the Player
            GameManager.singleton.Disc.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // To indicate that the disc was last caught by the Player
            GameManager.singleton.Disc.tag = "Player Disc";
            GameManager.singleton.Disc.gameObject.GetComponent<Renderer>().material = GameManager.singleton.playerMaterial;
            GameManager.singleton.Disc.gameObject.GetComponentInChildren<TrailRenderer>().material = GameManager.singleton.playerMaterial;

            // To reposition the disc on collision
            GameManager.singleton.lastPlayerPos = transform.position;

            // To reposition the disc
            GameManager.singleton.SetPlayerRepositionDisc(true);
            GameManager.singleton.SetDiscCollidedOnce(false);
        }
    }

    private void PlayerRepositionDisc()
    {
        discRepositionedPos = new Vector3(GameManager.singleton.lastPlayerPos.x,
                                          GameManager.singleton.lastPlayerPos.y,
                                          GameManager.singleton.lastPlayerPos.z - GameManager.singleton.discRepositionZDistance);

        // To reposition the Disc behind the Player when Caught
        GameManager.singleton.Disc.transform.position = Vector3.Lerp(GameManager.singleton.Disc.transform.position,
                                                                     discRepositionedPos,
                                                                     GameManager.singleton.discLerpMoveTime * Time.fixedDeltaTime);

        // To indicate the disc has repositioned
        if (GameManager.singleton.Disc.transform.position == discRepositionedPos)
            GameManager.singleton.SetPlayerRepositionDisc(false);
    }
}
