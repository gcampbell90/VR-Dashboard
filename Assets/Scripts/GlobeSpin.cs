using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeSpin : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(RotateGlobe());
    }

    private IEnumerator RotateGlobe()
    {
        while (true)
        {
            transform.rotation *= Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
            yield return null;
        }
    }
}
