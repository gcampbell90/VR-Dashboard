using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile;
    private List<string[]> csvData;
    private List<string[]> headerData;

    private List<Locations>_locationData;

    public List<Locations> LocationData { get { return _locationData; } set { _locationData = value; } }

    private void Start()
    {
        _locationData = ParseCsv<Locations>(csvFile.text);
        foreach (var item in _locationData)
        {
            Debug.Log($"Location: {item.Region} Lat: {item.Latitude} Lng: {item.Longitude}");
        }
    }

    public List<T> ParseCsv<T>(string csvData)
    {
        List<T> objectsList = new List<T>();

        // Split the CSV data by lines
        string[] lines = csvData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        // Get the header line
        string headerLine = lines[0];
        string[] headers = headerLine.Split(',');

        // Iterate over the remaining lines and parse them into objects
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = line.Split(',');

            T obj = Activator.CreateInstance<T>();

            // Iterate over the fields and set the object properties using reflection
            for (int j = 0; j < fields.Length; j++)
            {
                string field = fields[j];

                PropertyInfo property = typeof(T).GetProperty(headers[j].Trim());

                if (property != null)
                {
                    object value = Convert.ChangeType(field.Trim(), property.PropertyType);
                    property.SetValue(obj, value);
                }
            }
            objectsList.Add(obj);
        }

        return objectsList;
    }
}

public class Locations
{
    public string Region { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}