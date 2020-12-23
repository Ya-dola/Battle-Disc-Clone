﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLineController : MonoBehaviour
{
    public GameObject Disc;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerExit(Collider collider)
    {
        // To check if the disc passes through the centre line of the Arena
        if (collider.gameObject.Equals(Disc))
            // To Change the layer of the disc to a layer which collides with the Player and Enemy
            Disc.gameObject.layer = LayerMask.NameToLayer("Disc");
    }
}
