using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableController : MonoBehaviour
{
    public GameObject DestBrokenPrefab;

    void OnCollisionEnter(Collision collision)
    {
        // To check if the disc collided with the destructable
        if (!collision.gameObject.Equals(GameManager.singleton.Disc))
            return;

        // To indicate that the disc has already collided once if it already hasn't
        if (!GameManager.singleton.DiscCollidedOnce)
            GameManager.singleton.SetDiscCollidedOnce(true);

        // To reset the bounce count on disc collision with a destructable
        GameManager.singleton.bounceCount = 0;

        if ((GameManager.singleton.Disc.tag == "Player Disc" & gameObject.tag == "Enemy Dest") ||
            GameManager.singleton.Disc.tag == "Enemy Disc" & gameObject.tag == "Player Dest")
        {

            // Debug.Log("Disc Collided with Destructable");

            GameObject destBroken = Instantiate(DestBrokenPrefab, transform.position, transform.rotation);

            for (int i = 0; i < destBroken.transform.childCount; i++)
            {
                // To avoid affecting the Black Smoke Particles of Broken Destucables 
                if (destBroken.transform.GetChild(i).GetComponent<ParticleSystem>() == null)
                {
                    // Assigning the Destructable's Material to the Broken Parts of the Destructable
                    destBroken.transform.GetChild(i).GetComponent<Renderer>().material = GetComponent<Renderer>().material;

                    // Giving an Upwards Explosive force to the Broken Parts
                    destBroken.transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(GameManager.singleton.destExplosionForce,
                                                                                                 transform.position,
                                                                                                 GameManager.singleton.destExplosionRadius,
                                                                                                 GameManager.singleton.destExplosionUpwardsMod);
                }
            }

            Destroy(gameObject);

            // Plays the sound between the Camera's position and the Destructables's position
            AudioSource.PlayClipAtPoint(GameManager.singleton.destBrokenSound,
                                        0.9f * Camera.main.transform.position + 0.1f * transform.position,
                                        GameManager.singleton.destBrokenSoundVolume);

            Destroy(destBroken, GameManager.singleton.destBrokenDelay);
        }
    }
}
