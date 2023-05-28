using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scatterplot : MonoBehaviour
{
    public GameObject dataPointPrefab;
    public GameObject xAxis;
    public GameObject yAxis;
    public GameObject zAxis;

    public int numPoints = 100;
    public float plotSize = 100f;
    TextMesh xLabelTextMesh, yLabelTextMesh, zLabelTextMesh;

    public DataConfigurationPanel dataconfigPanel;

    public enum Axis { x, y, z }

    //Sample data
    string[] workerNames = { "Oliver Turner", "Isabella Baker", "Mason Hill", "Sophia Walker", "Ethan Green", "Harper Cooper" };
    string[] engagementDates = { "15/09/2019", "21/06/2020", "10/05/2021", "07/12/2020", "16/08/2019", "12/05/2020" };
    string[] bands = { "Band 4", "Band 5", "Band 7", "Band 6", "Band 5", "Band 3" };
    string[] trusts = { "Southern Trust", "South Eastern Trust", "Western Trust", "Belfast Trust", "Northern Trust", "Southern Trust" };

    private void Start()
    {
        CreateScatterplot(engagementDates, bands, trusts);
    }

    public void CreateScatterplot(string[] engagementDates, string[] bands, string[] trusts)
    {
        CreateAxisLines();

        // Calculate the X-axis range based on the minimum and maximum engagement dates
        DateTime minDate = DateTime.MaxValue;
        DateTime maxDate = DateTime.MinValue;

        foreach (string date in engagementDates)
        {
            DateTime currentDate = DateTime.ParseExact(date, "dd/MM/yyyy", null);
            if (currentDate < minDate)
                minDate = currentDate;
            if (currentDate > maxDate)
                maxDate = currentDate;
        }

        // Calculate the range of the date values
        TimeSpan dateRange = maxDate - minDate;

        //float xPosition = 0f;
        //float yPosition = 0f;
        //float zPosition = 0f;

        GameObject[] dataPoint = new GameObject[engagementDates.Length];

        Vector3[] positionMatrix = new Vector3[engagementDates.Length];

        for (int i = 0; i < engagementDates.Length; i++)
        {
            dataPoint[i] = Instantiate(dataPointPrefab, Vector3.zero, Quaternion.identity, transform);
            dataPoint[i].name = "Data Point ";
            Renderer dataPointRenderer = dataPoint[i].GetComponent<Renderer>();
            dataPointRenderer.material.color = Color.blue;
            //Color.HSVToRGB((float)i / (engagementDates.Length - 1), 1f, 1f);
        }

        // Loop through each engagement date and create the data point positions
        for (int i = 0; i < engagementDates.Length; i++)
        {
            DateTime currentDate = DateTime.ParseExact(engagementDates[i], "dd/MM/yyyy", null);
            float normalizedX = (float)(currentDate - minDate).TotalDays / (float)dateRange.TotalDays;
            //xPosition = Mathf.Lerp(-plotSize / 2f, plotSize / 2f, normalizedX);
            //positionMatrix[i].x = xPosition;
            positionMatrix[i].x = GetGraphDataPointPosition(normalizedX);

        }

        //Loop Through Y Axis (Bands to test)
        // Step 1: Sort the bands array alphabetically
        Array.Sort(bands);

        // Step 2: Extract the distinct bands from the dataset
        HashSet<string> uniqueBands = new HashSet<string>(bands);


        // Step 3: Calculate the total number of distinct bands
        int totalDistinctBands = uniqueBands.Count;

        // Step 4: Create a dictionary to store the normalized Y-axis positions for each band
        Dictionary<string, float> bandPositions = new Dictionary<string, float>();

        // Step 5: Calculate the normalized Y-axis position for each distinct band
        for (int i = 0; i < totalDistinctBands; i++)
        {
            string band = uniqueBands.ElementAt(i);
            float normalizedY = (float)i / (totalDistinctBands - 1);
            bandPositions.Add(band, normalizedY);
        }

        // Step 6: Loop through each data point and assign the Y-axis position based on the band
        for (int i = 0; i < bands.Length; i++)
        {
            string currentBand = bands[i];
            float normalizedY = bandPositions[currentBand];
            positionMatrix[i].y = GetGraphDataPointPosition(normalizedY);
        }

        // Step 1: Sort the trusts array alphabetically
        Array.Sort(trusts);

        // Step 2: Extract the distinct trusts from the sorted array
        HashSet<string> uniqueTrusts = new HashSet<string>(trusts);

        // Step 3: Calculate the total number of distinct trusts
        int totalDistinctTrusts = uniqueTrusts.Count;

        // Step 4: Create a dictionary to store the normalized Z-axis positions for each trust
        Dictionary<string, float> trustPositions = new Dictionary<string, float>();

        // Step 5: Calculate the normalized Z-axis position for each distinct trust
        for (int i = 0; i < totalDistinctTrusts; i++)
        {
            string trust = uniqueTrusts.ElementAt(i);
            float normalizedZ = (float)i / (totalDistinctTrusts - 1);
            trustPositions.Add(trust, normalizedZ);
        }

        // Step 6: Loop through each data point and assign the Z-axis position based on the trust
        for (int i = 0; i < trusts.Length; i++)
        {
            string currentTrust = trusts[i];
            float normalizedZ = trustPositions[currentTrust];
            //zPosition = Mathf.Lerp(-plotSize / 2f, plotSize / 2f, normalizedZ);
            //positionMatrix[i].z = zPosition;
            positionMatrix[i].z = GetGraphDataPointPosition(normalizedZ);

        }


        for (int i = 0; i < positionMatrix.Length; i++)
        {
            dataPoint[i].transform.position = positionMatrix[i] + transform.position;
        }
        // Instantiate the data point object at the calculated position

        // Assign a unique color to each data point

        // Add a label or other visual representation to the data point if desired
        // You can attach text objects, sprites, or other game objects to represent additional information

    }

    private float GetGraphDataPointPosition(float normalisedAxisPos)
    {
        var graphPos = Mathf.Lerp((-plotSize * 0.8f) / 2f, (plotSize * 0.8f) / 2f, normalisedAxisPos);
        return graphPos;
    }

    private void CreateAxisLines()
    {
        // X-axis line
        xAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        float offset = plotSize / 2f;
        xAxis.transform.parent = transform;
        xAxis.transform.localScale = new Vector3(plotSize, 0.1f, 0.1f);
        xAxis.transform.localPosition = new Vector3(0f, -offset, -offset);
        xAxis.GetComponent<Renderer>().material.color = Color.red;

        // Add X-axis label
        GameObject xLabel = new GameObject("X Axis Label");
        xLabel.transform.parent = transform;
        xLabel.transform.localPosition = new Vector3(offset, -offset, -offset);
        xLabelTextMesh = xLabel.AddComponent<TextMesh>();
        SetLabel(xLabelTextMesh, AssignLabels(Axis.x));
        xLabelTextMesh.anchor = TextAnchor.MiddleLeft;
        xLabelTextMesh.fontSize = 14;
        xLabelTextMesh.color = Color.black;

        // Y-axis line
        yAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        yAxis.transform.parent = transform;
        yAxis.transform.localScale = new Vector3(0.1f, plotSize, 0.1f);
        yAxis.transform.localPosition = new Vector3(-offset, 0f, -offset);
        yAxis.GetComponent<Renderer>().material.color = Color.green;

        // Add Y-axis label
        GameObject yLabel = new GameObject("Y Axis Label");
        yLabel.transform.parent = transform;
        yLabel.transform.localPosition = new Vector3(-offset, offset, -offset);
        yLabelTextMesh = yLabel.AddComponent<TextMesh>();
        SetLabel(yLabelTextMesh, AssignLabels(Axis.y));
        yLabelTextMesh.anchor = TextAnchor.MiddleRight;
        yLabelTextMesh.fontSize = 14;
        yLabelTextMesh.color = Color.black;

        // Z-axis line
        zAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zAxis.transform.parent = transform;
        zAxis.transform.localScale = new Vector3(0.1f, 0.1f, plotSize);
        zAxis.transform.localPosition = new Vector3(-offset, -offset, 0f);
        zAxis.GetComponent<Renderer>().material.color = Color.blue;

        // Add Z-axis label
        GameObject zLabel = new GameObject("Z Axis Label");
        zLabel.transform.parent = transform;
        zLabel.transform.localPosition = new Vector3(-offset, -offset, offset);
        zLabelTextMesh = zLabel.AddComponent<TextMesh>();
        SetLabel(zLabelTextMesh, AssignLabels(Axis.z));
        zLabelTextMesh.anchor = TextAnchor.MiddleRight;
        zLabelTextMesh.fontSize = 14;
        zLabelTextMesh.color = Color.black;

    }

    public void UpdateAllLabels()
    {
        SetLabel(xLabelTextMesh, AssignLabels(Axis.x));
        SetLabel(yLabelTextMesh, AssignLabels(Axis.y));
        SetLabel(zLabelTextMesh, AssignLabels(Axis.z));
    }

    private void SetLabel(TextMesh labelTextMesh, string labelText)
    {
        labelTextMesh.text = labelText;
    }

    private string AssignLabels(Scatterplot.Axis axis)
    {
        var returnedVal = "";
        switch (axis)
        {
            case Axis.x:
                returnedVal = dataconfigPanel.xAxis != null ? dataconfigPanel.xAxis : "x";
                break;
            case Axis.y:
                returnedVal = dataconfigPanel.yAxis != null ? dataconfigPanel.yAxis : "y";
                break;
            case Axis.z:
                returnedVal = dataconfigPanel.zAxis != null ? dataconfigPanel.zAxis : "z";
                break;
            default:
                Debug.LogError("Not valid label axis");
                break;
        }
        //Debug.Log(returnedVal);

        return returnedVal;
    }
}
