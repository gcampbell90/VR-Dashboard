using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class DataConfigurationPanel : Panel
{
    public TMP_Dropdown xDropdown;
    public TMP_Dropdown yDropdown;
    public TMP_Dropdown zDropdown;
    public TMP_InputField titleInputField;
    public Button visualizeButton;
    public TextMeshProUGUI userMessageText;

    private List<string> columnHeaders;
    private List<string[]> csvData;

    public string xAxis { get; set; }
    public string yAxis { get; set; }
    public string zAxis { get; set; }
    public System.Action OnVisualisationButtonPress { get; internal set; }

    public DatasetSelectionPanel datasetSelectionPanel;

    public string[] workerNames;
    public string[] engagementDates;
    public string[] bands;
    public string[] trusts;

    public string[] xAxisData;
    public string[] yAxisData;
    public string[] zAxisData;

    public GameObject CurrentGraphObject { get; set; }

    #region Tools Panel
    [SerializeField] Button addToStoryButton;
    [SerializeField] Button deleteButton;
    public System.Action OnAddStoryButtonPress { get; internal set; }
    #endregion


    public override void Initialise()
    {
        Debug.Log("Initialised");
        xAxis = "xAxis";
        yAxis = "yAxis";
        zAxis = "zAxis";

        csvData = datasetSelectionPanel.CsvData;

        columnHeaders = datasetSelectionPanel.HeaderData;

        visualizeButton.onClick.RemoveAllListeners();

        visualizeButton.onClick.AddListener(OnVisualizeButtonClick);
        SetDropdownData(columnHeaders, csvData);
    }

    public void SetDropdownData(List<string> headers, List<string[]> data)
    {
        columnHeaders = headers;
        csvData = data;

        PopulateDropdowns();
    }

    private void PopulateDropdowns()
    {
        xDropdown.ClearOptions();
        yDropdown.ClearOptions();
        zDropdown.ClearOptions();

        // Populate the dropdown options with column headers
        xDropdown.AddOptions(columnHeaders);
        yDropdown.AddOptions(columnHeaders);
        zDropdown.AddOptions(columnHeaders);

        yDropdown.value = 1;
        zDropdown.value = 2;

    }

    private void OnVisualizeButtonClick()
    {
        // Get the selected axis mappings from the dropdowns
        xAxis = xDropdown.options[xDropdown.value].text;
        yAxis = yDropdown.options[yDropdown.value].text;
        zAxis = zDropdown.options[zDropdown.value].text;

        // Get the visualization title from the input field
        string visualizationTitle = titleInputField.text;

        // Retrieve the column data for the selected headers
        xAxisData = GetColumn(xAxis);
        yAxisData = GetColumn(yAxis);
        zAxisData = GetColumn(zAxis);

        engagementDates = xAxisData;
        bands = yAxisData;
        trusts = zAxisData;


        // Display user message
        DisplayUserMessage("Visualization created: " + visualizationTitle);

        OnVisualisationButtonPress?.Invoke();


    }

    public string[] GetColumn(string headerName)
    {
        // Find the index of the header
        int headerIndex = -1;
        string[] headers = csvData[0];

        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Equals(headerName, StringComparison.OrdinalIgnoreCase))
            {
                headerIndex = i;
                break;
            }
        }

        // If the header is found, extract the column data
        if (headerIndex != -1)
        {
            string[] columnData = new string[csvData.Count - 1]; // Exclude the header row

            for (int i = 1; i < csvData.Count; i++) // Start from index 1 to skip the header row
            {
                string[] row = csvData[i];
                columnData[i - 1] = row[headerIndex]; // Extract the value at the corresponding header index
            }

            return columnData;
        }

        // If the header is not found, return an empty array or handle the error as desired
        return new string[0];
    }

    public void SetToolActions(GameObject graph)
    {
        CurrentGraphObject = graph;

        addToStoryButton.onClick.AddListener(OnAddStoryButtonClick);
        deleteButton.onClick.AddListener(OnDeleteButtonClick);

        Debug.Log("Tool Actions Set");
    }

    private void OnDeleteButtonClick()
    {
        addToStoryButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();
        Destroy(CurrentGraphObject);
        Debug.Log("Delete Button Clicked");
    }

    private void OnAddStoryButtonClick()
    {
        //Invoke add to story method in dashboard manager to add graph view to story panel
        Debug.Log("Add Button Clicked");
        OnAddStoryButtonPress?.Invoke();
        //Cleanup
        Debug.Log("Cleanup creation panel");
        CurrentGraphObject.SetActive(false);
        addToStoryButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();
    }

    private void DisplayUserMessage(string message)
    {
        userMessageText.text = message;
    }
}

