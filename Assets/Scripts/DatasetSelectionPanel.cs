using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class DatasetSelectionPanel : Panel
{
    public Button browseButton;
    public Button loadCSVButton;
    public Button loadURLButton;
    public Button continueButton;
    public TextMeshProUGUI successMessageText;
    public TextMeshProUGUI csvHeadersText;
    public TextMeshProUGUI csvInfoText;

    private string filePath;
    string url = "https://docs.google.com/spreadsheets/d/1ZkatHtNseT_KkjKfH3o03MgersAtimclTekvSlOpuHQ/export?format=csv";

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
        loadCSVButton.onClick.AddListener(OnLoadButtonClick);
        loadURLButton.onClick.AddListener(OnLoadURLButtonClick);
        continueButton.onClick.AddListener(delegate { OnContinueButtonPressed?.Invoke(); });
    }

    private void OnLoadURLButtonClick()
    {
        InitialiseGoogleSheetFetch();
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
            GetDataSetInfo(CsvData);

            // Display success message
            DisplaySuccessMessage();

            //// Display CSV headers
            DisplayCSVHeaders();

            // Proceed to the next panel or perform further actions with the loaded data
            // For example, you can call a function like ActivateDataConfigurationPanel(csvData) to proceed to the data configuration panel
            // You would need to implement this function in the appropriate script for the next panel
            // Example: dashboardManager.ActivateDataConfigurationPanel(csvData);
        }
    }

    public List<string[]> LoadCSVData(string path)
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

    private void DisplayCSVHeaders()
    {

        ShowContinueButton();
    }

    private void GetDataSetInfo(List<string[]> data)
    {
        // Clear the existing headers
        csvHeadersText.text = "";

        int count = data.Count() - 1; //count of all rows minus header row to get total entries
        // Display the CSV headers
        if (count > 0)
        {
            string[] headers = data[0];

            //HeaderData = headers.ToList();
            string headersText = string.Join(", ", headers);
            HeaderData = headers.ToList();

            csvHeadersText.text = headersText;
        }

        csvInfoText.text = "";

        csvInfoText.text = "Datasetcount " + count;


        // Group data by a specific column
        //int columnToGroupByIndex = 0; // Replace with the index of the column you want to group by

        for (int i = 0; i < HeaderData.Count; i++)
        {

            var groupedData = data.GroupBy(row => row[i]);

            // Display the grouped categories and their counts
            csvInfoText.text += "\nGrouped Categories:\n" + HeaderData[i]+"\n";
            foreach (var group in groupedData)
            {
                string category = group.Key.ToString();
                int groupcount = group.Count();

                csvInfoText.text += "\nCategory: " + category + " | Count: " + groupcount + "\n";
            }
        }
        //Debug.Log("Datasetcount " + count);
        //Debug.Log("Dataset sample " + CsvData[0]);
    }

    private void ShowContinueButton()
    {
        continueButton.gameObject.SetActive(true);
    }

    public void InitialiseGoogleSheetFetch()
    {
        DownloadFile(url);

    }
    public void DownloadFile(string url)
    {
        StartCoroutine(DownloadFileCoroutine(url));
    }

    private IEnumerator DownloadFileCoroutine(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string downloadedData = Encoding.UTF8.GetString(webRequest.downloadHandler.data);

            // Process the downloaded data as needed (e.g., parse into an object)
            // Example:
            var myObject = ParseDownloadedData(downloadedData);

            // Do something with the created object
            // Example:
            Debug.Log("Data Object created: " + myObject.ToString());
            DisplaySuccessMessage();
            DisplayCSVHeaders();
            CsvData = myObject.RowData;
        }
        else
        {
            Debug.LogError("Download failed: " + webRequest.error);
        }

        webRequest.Dispose();
    }

    private DataObject ParseDownloadedData(string data)
    {
        List<string[]> rows = new List<string[]>();

        // Split the data by newlines to get individual lines
        string[] lines = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        DataObject dataObject = new DataObject();
        int count = 0;

        foreach (string line in lines)
        {
            // Split each line by commas to get individual fields
            string[] fields = line.Split(',');

            // Add the fields to the list of rows
            rows.Add(fields);

            //Debug.Log($"CSVData: { lines[count]}");
            count++;
        }
        dataObject.HeaderData = rows[0].ToList();
        dataObject.RowData = rows;
        GetDataSetInfo(rows);

        return dataObject;
    }
    //Debug.Log("Datasetcount " + count);
    //Debug.Log("Dataset sample " + CsvData[0]);
}

