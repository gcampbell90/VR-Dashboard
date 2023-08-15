using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCamera : MonoBehaviour
{
    public delegate void FocusOnNode(Transform node);
    public static FocusOnNode focusOnNode;

    [SerializeField] float pos_Offset;

    private void OnEnable()
    {
        focusOnNode += FocusCameraOnNode;
    }

    private void FocusCameraOnNode(Transform node)
    {
        Vector3 newPos = node.position + new Vector3(0, 0, -pos_Offset);
        transform.position = newPos;
        transform.LookAt(node);
    }
}
