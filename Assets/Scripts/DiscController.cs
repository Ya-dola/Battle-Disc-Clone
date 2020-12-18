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

    }

    // Update is called once per frame
    void Update()
    {
        // To get the last velocity of the disc before a collision
        lastDiscVelocity = discRigBody.velocity;
    }

    void LateUpdate()
    {
        // To Change the layer of the disc to a layer which collides with the player
        if (GameManager.singleton.DiscCollidedOnce)
            gameObject.layer = LayerMask.NameToLayer("Disc");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            // discRigBody.velocity = Vector3.Reflect(lastDiscVelocity.normalized, collision.contacts[0].normal) *
            //                         Mathf.Max(lastDiscVelocity.magnitude, GameManager.singleton.discBounceForce);

            // discRigBody.velocity = Vector3.Reflect(lastDiscVelocity.normalized, collision.GetContact(0).normal) *
            //                         Mathf.Max(lastDiscVelocity.magnitude, GameManager.singleton.discBounceForce);

            // To Achieve the Bounce Effect by the Disc
            discRigBody.velocity = Vector3.Reflect(lastDiscVelocity.normalized, collision.GetContact(0).normal) *
                                   GameManager.singleton.discForce;
        }
    }
}
