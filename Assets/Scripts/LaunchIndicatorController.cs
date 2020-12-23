using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchIndicatorController : MonoBehaviour
{
    public GameObject Player;
    public GameObject Disc;

    private Vector3 indicatorDirection;
    private float indicatorAngle;

    // Update is called once per frame
    void Update()
    {
        // Runs only when the Indicator is active
        if (gameObject.activeSelf)
        {
            indicatorDirection = Vector3.Normalize(Player.transform.position - Disc.transform.position);
            indicatorAngle = Mathf.Atan2(indicatorDirection.x, indicatorDirection.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.AngleAxis(indicatorAngle, Vector3.up);
        }
    }
}
