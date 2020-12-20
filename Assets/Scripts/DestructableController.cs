using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableController : MonoBehaviour
{
    public GameObject Disc;
    public GameObject DestBrokenPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        // To check if the disc collided with the destructable
        if (!collision.gameObject.Equals(Disc))
            return;

        if (!GameManager.singleton.DiscCollidedOnce)
            GameManager.singleton.SetDiscCollidedOnce(true);

        if ((Disc.tag == "Player Disc" & gameObject.tag == "Enemy Dest") ||
            Disc.tag == "Enemy Disc" & gameObject.tag == "Player Dest")
        {

            // Debug.Log("Disc Collided with Destructable");

            GameObject destBroken = Instantiate(DestBrokenPrefab, transform.position, transform.rotation);

            for (int i = 0; i < destBroken.transform.childCount; i++)
            {
                // Assigning the Destructable's Material to the Broken Parts of the Destructable
                destBroken.transform.GetChild(i).GetComponent<Renderer>().material = GetComponent<Renderer>().material;

                // Giving an Upwards Explosive force to the Broken Parts
                destBroken.transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(GameManager.singleton.destExplosionForce,
                                                                                             transform.position,
                                                                                             GameManager.singleton.destExplosionRadius,
                                                                                             GameManager.singleton.destExplosionUpwardsMod);
            }

            Destroy(gameObject);

            Destroy(destBroken, GameManager.singleton.destBrokenDelay);
        }
    }
}
