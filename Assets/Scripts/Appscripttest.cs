using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

public class Appscripttest : MonoBehaviour
{
    string url = "https://docs.google.com/spreadsheets/d/1ZkatHtNseT_KkjKfH3o03MgersAtimclTekvSlOpuHQ/export?format=csv";

    [SerializeField] private DatasetSelectionPanel datasetSelection;

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
            Debug.Log("Object created: " + myObject.ToString());
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
            
            Debug.Log($"CSVData: { lines[count]}");
            count++;
        }
        dataObject.HeaderData = rows[0].ToList();
        dataObject.RowData = rows;
 
        return dataObject;
    }
    //Debug.Log("Datasetcount " + count);
    //Debug.Log("Dataset sample " + CsvData[0]);
}
