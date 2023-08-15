using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(BoxCollider))]
public class DoorController : MonoBehaviour
{
    private Transform _doorTransform;

    private Vector3 _doorPositionOpen = new Vector3(-1, 0, 0);
    private Vector3 _doorPositionClosed;

    Task ToggleDoorTask;
    CancellationTokenSource _cts = null;

    [SerializeField] bool canOpen = false;


    private void Start()
    {
        _doorTransform = transform.GetChild(2);
        _doorPositionClosed = _doorTransform.localPosition;
        _doorPositionOpen = _doorPositionOpen + _doorTransform.localPosition;

        //set up cancellation token for move task
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canOpen) return;

        Debug.Log("Enter Door");

        Trigger(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!canOpen) return;

        Debug.Log("Exit Door");

        Trigger(false);
    }

    private async void Trigger(bool openDoor)
    {
        if (ToggleDoorTask != null && !ToggleDoorTask.IsCompleted)
        {
            Debug.Log("Toggle Door running");
            _cts?.Cancel();
        }
        _cts = new CancellationTokenSource();

        try
        {
            await (ToggleDoorTask = ToggleDoor(openDoor, _cts.Token));
        }
        catch
        {
            Debug.Log("An error occurred");
        }
        finally
        {
            Debug.Log("Set Task");
            _cts?.Dispose();
            _cts = null;
        }
    }


    //IEnumerator ToggleDoor(bool openDoor)
    //{
    //    Debug.Log("Toggle Door Open:"+ openDoor);
    //    var timer = 0f;
    //    var dur = 1f;

    //    var endPos = openDoor ? _doorPositionOpen : _doorPositionClosed;
    //    var doorPos = _doorTransform.localPosition;

    //    while (timer < dur)
    //    {
    //        _doorTransform.localPosition = Vector3.Lerp(doorPos, endPos, timer);
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }
    //}

    private async Task ToggleDoor(bool openDoor, CancellationToken token)
    {
        var timer = 0f;
        var endPos = openDoor ? _doorPositionOpen : _doorPositionClosed;
        var doorPos = _doorTransform.localPosition;

        //normalized range of open and closed positon
        var normalisedMagnitude = Vector3.SqrMagnitude(doorPos - endPos);
        var dur = 1f;


        //update duration to be variable for when door is in diff postions the lerp should be different

        while (timer < dur)
        {
            if (token.IsCancellationRequested)
            {
                Debug.Log("Task Cancelled");
                return;
            }

            _doorTransform.localPosition = Vector3.Lerp(doorPos, endPos, timer/normalisedMagnitude);
            timer += Time.deltaTime;
            await Task.Yield();
        }

    }
}
