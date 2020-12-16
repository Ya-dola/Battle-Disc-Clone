using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscController : MonoBehaviour
{
    private Rigidbody discRigBody;
    private Vector3 lastDiscVelocity;

    void Awake()
    {
        discRigBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.singleton.setDiscCaught(true);
        GameManager.singleton.setDiscCollidedOnce(true);
    }

    // Update is called once per frame
    void Update()
    {
        lastDiscVelocity = discRigBody.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // To check if the disc collided with the Player
        if (collision.gameObject.tag == "Player" && GameManager.singleton.DiscCollidedOnce)
        {
            // Debug.Log("Disc Collided with Player");

            GameManager.singleton.setDiscCaught(true);
            GameManager.singleton.setDiscCollidedOnce(false);

            // discRigBody.velocity = Vector3.zero;
        }
        else
        {
            // discRigBody.velocity = Vector3.Reflect(lastDiscVelocity.normalized, collision.contacts[0].normal) *
            //                         Mathf.Max(lastDiscVelocity.magnitude, GameManager.singleton.discBounceForce);

            discRigBody.velocity = Vector3.Reflect(lastDiscVelocity.normalized, collision.GetContact(0).normal) *
                                    Mathf.Max(lastDiscVelocity.magnitude, GameManager.singleton.discBounceForce);
        }
    }
}
