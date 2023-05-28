using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class DatasetSelectionPanel : Panel
{
    public Button browseButton;
    public Button loadButton;
    public Button continueButton;
    public TextMeshProUGUI successMessageText;
    public TextMeshProUGUI csvHeadersText;

    private string filePath;
    public List<string[]> CsvData { get; set; }
    public List<string> HeaderData { get; set; }
    public System.Action OnContinueButtonPressed { get; internal set; }

    private void Awake()
    {
        continueButton.gameObject.SetActive(false);
    }

    public override void Initialise()
    {
        Debug.Log("Initialised");

        // Set up event handlers for the browse and load buttons
        browseButton.onClick.AddListener(OnBrowseButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
        continueButton.onClick.AddListener(delegate { OnContinueButtonPressed?.Invoke(); });
    }

    private void OnBrowseButtonClick()
    {
        // Open file dialog to select a CSV file
        string[] fileExtensions = new string[] { "csv" };
        string browsePath = UnityEditor.EditorUtility.OpenFilePanel("Select CSV file", "", "csv");

        // Set the selected file path
        if (!string.IsNullOrEmpty(browsePath))
        {
            filePath = browsePath;
        }
    }

    private void OnLoadButtonClick()
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            // Load CSV data from the selected file
            CsvData = LoadCSVData(filePath);

            // Display success message
            DisplaySuccessMessage();

            // Display CSV headers
            DisplayCSVHeaders(CsvData);

            // Proceed to the next panel or perform further actions with the loaded data
            // For example, you can call a function like ActivateDataConfigurationPanel(csvData) to proceed to the data configuration panel
            // You would need to implement this function in the appropriate script for the next panel
            // Example: dashboardManager.ActivateDataConfigurationPanel(csvData);
        }
    }

    private List<string[]> LoadCSVData(string path)
    {
        List<string[]> data = new List<string[]>();

        // Read the CSV file
        using (StreamReader reader = new StreamReader(path))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                data.Add(values);
            }
        }

        return data;
    }

    private void DisplaySuccessMessage()
    {
        // Set success message
        successMessageText.text = "CSV file loaded successfully!";
    }

    private void DisplayCSVHeaders(List<string[]> data)
    {
        // Clear the existing headers
        csvHeadersText.text = "";

        // Display the CSV headers
        if (data.Count > 0)
        {
            string[] headers = data[0];

            //HeaderData = headers.ToList();
            string headersText = string.Join(", ", headers);
            HeaderData = headers.ToList();

            //foreach (var item in HeaderData)
            //{
            //    Debug.Log(item);

            //}
            csvHeadersText.text = headersText;
        }
        ShowContinueButton();
    }

    private void ShowContinueButton()
    {
        continueButton.gameObject.SetActive(true);
    }

    //public void ShowPanel()
    //{
    //    this.gameObject.SetActive(true);
    //}

    //public void HidePanel()
    //{
    //    this.gameObject.SetActive(false);
    //}

    //public override void SetInteractable(bool interactable)
    //{
    //    continueButton.interactable = interactable;
    //}

    //public override void OnStepCompleted()
    //{
    //    throw new System.NotImplementedException();
    //}
}
