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

    // Fixed Update used mainly for Physics Calculations
    void FixedUpdate()
    {
        // To work only when the Game has started or has not ended or is not paused
        if (!GameManager.singleton.GameStarted || GameManager.singleton.GameEnded || GameManager.singleton.GamePaused)
            return;

        // To ensure the disc always moves at a constant speed when not caught by the Player or Enemy
        if (!GameManager.singleton.PlayerDiscCaught &
            !GameManager.singleton.PlayerRepositionDisc &
            !GameManager.singleton.EnemyDiscCaught &
            !GameManager.singleton.EnemyRepositionDisc &
            discRigBody.velocity.magnitude < GameManager.singleton.discSpeed - GameManager.singleton.discSpeedDiff)
        {
            if (transform.position.z < 0)
                discRigBody.velocity = new Vector3(Random.Range(-1f, 1f), 0, 1).normalized * GameManager.singleton.discSpeed;
            else
                discRigBody.velocity = new Vector3(Random.Range(-1f, 1f), 0, -1).normalized * GameManager.singleton.discSpeed;
        }

        // To ensure the disc stops when it is caught by the Player or Enemy
        if (GameManager.singleton.PlayerDiscCaught | GameManager.singleton.EnemyDiscCaught)
            discRigBody.velocity = Vector3.zero;
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

    void OnCollisionEnter(Collision collision)
    {
        // To work only when the Game has started
        if (!GameManager.singleton.GameStarted || GameManager.singleton.GameEnded)
            return;

        // To bounce when the disc collides with anything other than the Player or Enemy
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Enemy")
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

            // To Display the Effect when the Disc Collides
            GameObject discCollisionFade = Instantiate(GameManager.singleton.discCollisionFadePrefab,
                                                       transform.position,
                                                       transform.rotation);

            discCollisionFade.GetComponent<Animator>().Play("Disc Collision Fade Animation");

            Destroy(discCollisionFade, GameManager.singleton.discColFadeDestDelay);
        }
    }

    private void PlayDiscFade()
    {

    }
}
