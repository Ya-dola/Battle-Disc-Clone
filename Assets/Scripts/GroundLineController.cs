using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLineController : MonoBehaviour
{
    void OnTriggerExit(Collider collider)
    {
        // To check if the disc passes through the centre line of the Arena
        if (collider.gameObject.Equals(GameManager.singleton.Disc))
            // To Change the layer of the disc to a layer which collides with the Player and Enemy
            GameManager.singleton.Disc.gameObject.layer = LayerMask.NameToLayer("Disc");
    }
}
