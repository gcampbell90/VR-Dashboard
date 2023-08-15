using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Camera _camera;
    private Transform _transform; // Cache transform for better performance
    private Vector3 _lastCameraPosition;
    private const float updateThreshold = 0.5f; // Update if the camera has moved more than this threshold

    private void Awake()
    {
        _camera = Camera.main;
        _transform = transform;
        _lastCameraPosition = _camera.transform.position;
    }

    void LateUpdate()
    {
        if (Vector3.Distance(_lastCameraPosition, _camera.transform.position) > updateThreshold)
        {
            LookAtCamera();
            _lastCameraPosition = _camera.transform.position;
        }
    }

    private void LookAtCamera()
    {
        Vector3 direction = (_camera.transform.position - _transform.position).normalized;
        // We want rotation around Y-axis only, so we project the direction onto the world's XZ plane
        Vector3 lookDirection = Vector3.ProjectOnPlane(direction, Vector3.up);
        _transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
