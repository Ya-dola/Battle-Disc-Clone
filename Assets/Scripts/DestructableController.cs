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

        // TODO - Change it to destroy only when collides with the disc USE TAGS !!!

        if (!playerController)
            return;

        GameObject destBroken = Instantiate(DestBrokenPrefab, transform.position, transform.rotation);

        for (int i = 0; i < destBroken.transform.childCount; i++)
        {
            destBroken.transform.GetChild(i).GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        }

        Destroy(gameObject);

        Destroy(destBroken, GameManager.singleton.destBrokenDelay);
    }
}
