using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetHologramController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, 5 * Time.deltaTime);
    }
}
