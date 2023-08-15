using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DataStoryPanel : MonoBehaviour
{
    [SerializeField] Button buttonPrefab;
    [SerializeField] Transform storyPanel;
    [SerializeField] Transform toolsPanel;

    private List<GameObject> storyGraphs = new List<GameObject>();

    public void RegisterGraph(GameObject currentGraphObject)
    {
        Debug.Log($"Added {currentGraphObject} to Data Story");
        storyGraphs.Add(currentGraphObject);

        int currIndex = storyGraphs.Count;
        var storyButton = Instantiate(buttonPrefab, storyPanel);
        storyButton.onClick.AddListener(delegate { ViewStory(currIndex-1); });
        currentGraphObject.SetActive(false);
        storyButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Slide {currIndex}";
        AddTools(currIndex-1);
    }

    //void AddListener(Button button, int index)
    //{
    //    button.onClick.AddListener(delegate { ViewStory(index); });
    //}

    private void ViewStory(int v)
    {
        Debug.Log($"Viewing Story {v}");
        for (int i = 0; i < storyGraphs.Count; i++)
        {
            storyGraphs[i].SetActive(false);
        }
        storyGraphs[v].SetActive(true);
    }

    private void AddTools(int index)
    {
        Debug.Log($"Adding story options for story {index}");
        var deletestoryButton = Instantiate(buttonPrefab, toolsPanel);
        deletestoryButton.onClick.AddListener(delegate { DeleteStory(index); });
        deletestoryButton.GetComponentInChildren<TextMeshProUGUI>().text = "Delete Story";
    }

    private void DeleteStory(int v)
    {
        Debug.Log($"Deleting Story {v}");
        var selStory = storyGraphs[v];
        storyGraphs.Remove(selStory);
        Destroy(selStory);
        Destroy(toolsPanel.GetChild(v).gameObject);
        Destroy(storyPanel.GetChild(v).gameObject);
    }
}
