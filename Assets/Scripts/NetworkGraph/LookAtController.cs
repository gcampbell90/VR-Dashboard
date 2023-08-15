using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtController : MonoBehaviour
{
    public List<Transform> objectsToRotate = new List<Transform>();
    private Camera _camera;
    private Vector3 _lastCameraPosition;
    private const float updateThreshold = 0.5f; // Update if the camera has moved more than this threshold

    private void Awake()
    {
        _camera = Camera.main;
        _lastCameraPosition = _camera.transform.position;

    }

    private void LateUpdate()
    {
        if (Vector3.Distance(_lastCameraPosition, _camera.transform.position) > updateThreshold)
        {
            foreach (Transform obj in objectsToRotate)
            {
                LookAtCamera(obj);
            }
            _lastCameraPosition = _camera.transform.position;
        }
    }

    private void LookAtCamera(Transform obj)
    {
        Vector3 direction = (_camera.transform.position - obj.position).normalized;
        // We want rotation around Y-axis only, so we project the direction onto the world's XZ plane
        Vector3 lookDirection = -Vector3.ProjectOnPlane(direction, Vector3.up);
        obj.rotation = Quaternion.LookRotation(lookDirection);
    }
}
