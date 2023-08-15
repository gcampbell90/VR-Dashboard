using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        UI_Controller.OnTriggerUI?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        UI_Controller.OnCloseUI?.Invoke();
    }

}
