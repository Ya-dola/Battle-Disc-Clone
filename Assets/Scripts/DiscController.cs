using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscController : MonoBehaviour
{
    private Rigidbody discRigBody;
    private Vector3 lastDiscVelocity;
    private Vector3 discReflection;

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
        // To Change the layer of the disc to a layer which collides with the Player and Enemy
        if (GameManager.singleton.DiscCollidedOnce)
            gameObject.layer = LayerMask.NameToLayer("Disc");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // To work only when the Game has started
        if (!GameManager.singleton.GameStarted)
            return;

        // To bounce when the disc collides with anything other than the Player
        if (collision.gameObject.tag != "Player")
        {
            // discRigBody.velocity = Vector3.Reflect(lastDiscVelocity.normalized, collision.contacts[0].normal) *
            //                         Mathf.Max(lastDiscVelocity.magnitude, GameManager.singleton.discBounceForce);

            // discRigBody.velocity = Vector3.Reflect(lastDiscVelocity.normalized, collision.GetContact(0).normal) *
            //                         Mathf.Max(lastDiscVelocity.magnitude, GameManager.singleton.discBounceForce);

            // To Achieve the Bounce Effect by the Disc
            GameManager.singleton.bounceCount++;

            discReflection = Vector3.Reflect(lastDiscVelocity.normalized, collision.GetContact(0).normal);

            // To Randomize the bounce every 'X' number of bounces where 'X' = 'bounceRandIterator'
            if (GameManager.singleton.bounceCount % GameManager.singleton.bounceRandIterator == 0)
            {
                discReflection = Vector3.Normalize(
                                    Vector3.Lerp(
                                        Vector3.Reflect(lastDiscVelocity.normalized, collision.GetContact(0).normal),
                                            // Direction of the Last Position where the Player and the Disc collided
                                            GameManager.singleton.lastPlayerPos.normalized - transform.position.normalized,
                                            GameManager.singleton.bounceBias));
            }

            discRigBody.velocity = discReflection * GameManager.singleton.discSpeed;
        }
    }
}
