using System;
using System.Collections;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    public delegate void TriggerUI();
    public static TriggerUI OnTriggerUI;

    public delegate void CloseUI();
    public static TriggerUI OnCloseUI;

    private GameObject planet;
    [SerializeField]private GameObject _solarSystem;
    private void Awake()
    {
        planet = transform.GetChild(0).transform.GetChild(0).gameObject;
        Debug.Log(planet);

    }

    private void OnEnable()
    {
        OnTriggerUI += OpenUI;
        OnCloseUI += ExitUI;
    }

    private void OnDisable()
    {
        OnTriggerUI -= OpenUI;
        OnCloseUI -= ExitUI;
    }

    private void OpenUI()
    {
        planet.SetActive(true);
        _solarSystem.SetActive(true);
    }

    private void ExitUI()
    {
        planet.SetActive(false);
        _solarSystem.SetActive(false);
    }

}