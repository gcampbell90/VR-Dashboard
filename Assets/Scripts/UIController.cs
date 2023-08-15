using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Transform togglePanel;
    [SerializeField] Toggle buttonPrefab;
    List<GameObject> sourceGameobjects = new List<GameObject>();
    ToggleGroup toggleGroup;
    List<Toggle> toggles = new List<Toggle>();

    private void Start()
    {
        toggleGroup = togglePanel.GetComponent<ToggleGroup>();
    }

    public void CreateButton(Node node)
    {
   
            //Debug.Log(node.Id + " " + node.Tag);
            var m_toggle = Instantiate(buttonPrefab, togglePanel);
            m_toggle.group = toggleGroup;
            string btnMsg = $"Tag:{node.Name}";
            if (node.ConnectionCount > 0)
            {
                btnMsg += $"\nConnectionCount:{node.ConnectionCount}";
            }
            m_toggle.GetComponentInChildren<TextMeshProUGUI>().text = btnMsg;

            m_toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(m_toggle, node.nodeObject); });
            sourceGameobjects.Add(node.nodeObject);
            toggles.Add(m_toggle);
        
    }

    //Change method to search for tag instead
    private void OnToggleValueChanged(Toggle m_toggle, GameObject nodeObject)
    {
        ColorBlock cb = m_toggle.colors;

        cb.normalColor = m_toggle.isOn ? Color.green : Color.white;

        //nodeObject.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = m_toggle.isOn;
        FocusCamera.focusOnNode?.Invoke(nodeObject.transform);
    }
}
