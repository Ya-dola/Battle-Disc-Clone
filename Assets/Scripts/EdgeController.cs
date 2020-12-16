using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        // To check if the disc collided with the Arena Edge
        if (collision.gameObject.tag != "Disc")
            return;

        if (!GameManager.singleton.DiscCollidedOnce)
            GameManager.singleton.setDiscCollidedOnce(true);

        // Debug.Log("Disc Collided with Edge");
    }
}
