using System;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float Size { get; set; }
    public float Speed { get; set; }
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }

    private void Update()
    {
        // Calculate the new position of the sphere based on the current time and rotation speed
        transform.RotateAround(Origin, transform.up, Speed * Time.deltaTime );
    }
}

