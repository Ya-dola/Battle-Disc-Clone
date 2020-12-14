using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableController : MonoBehaviour
{
    public GameObject DestBrokenPrefab;

    private void OnCollisionEnter(Collision collision)
    {

        // To check if the player collided with the destructable
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

        // TODO - Change it to destroy only when collides with the disc

        if (!playerController)
            return;

        GameObject destBroken = Instantiate(DestBrokenPrefab, transform.position, transform.rotation);

        Destroy(gameObject);

        Destroy(destBroken, GameManager.singleton.destBrokenDelay);
    }
}
