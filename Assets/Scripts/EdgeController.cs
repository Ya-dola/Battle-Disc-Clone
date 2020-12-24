using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeController : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // To check if the disc collided with the Arena Edge
        if (!collision.gameObject.Equals(GameManager.singleton.Disc))
            return;

        if (!GameManager.singleton.DiscCollidedOnce)
            GameManager.singleton.SetDiscCollidedOnce(true);
    }
}
