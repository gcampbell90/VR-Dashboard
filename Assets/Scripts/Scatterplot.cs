using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

public class Scatterplot : MonoBehaviour
{
    public GameObject dataPointPrefab;
    public GameObject xAxis;
    public GameObject yAxis;
    public GameObject zAxis;

    private int swarmSize;
    public float plotSize = 1f;
    TextMesh xLabelTextMesh, yLabelTextMesh, zLabelTextMesh;

    public DataConfigurationPanel dataconfigPanel;

    public enum Axis { x, y, z }

    //Sample data
    //string[] workerNames = { "Oliver Turner", "Isabella Baker", "Mason Hill", "Sophia Walker", "Ethan Green", "Harper Cooper" };
    string[] engagementDates = { "15/09/2019", "21/06/2020", "10/05/2021", "07/12/2020", "16/08/2019", "12/05/2020" };
    string[] bands = { "Band 4", "Band 5", "Band 7", "Band 6", "Band 5", "Band 3" };
    string[] trusts = { "Southern Trust", "South Eastern Trust", "Western Trust", "Belfast Trust", "Northern Trust", "Southern Trust" };

    public delegate void CalculateNormalisedPosition();
    public CalculateNormalisedPosition calculateNormalPosition;

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(plotSize, plotSize, plotSize));
    }

    public void Initialise(Transform parent, DataConfigurationPanel dataconfigPanel)
    {
        transform.SetParent(parent, false);
        //CreateScatterplot(engagementDates, bands, trusts);

        this.dataconfigPanel = dataconfigPanel;
    }

    public void CreateScatterplot(string[] xAxis, string[] yAxis, string[] zAxis)
    {
        CreateAxisLines();
        swarmSize = xAxis.Length;
        GameObject[] swarmDataPoints = new GameObject[swarmSize];
        Vector3[] positionMatrix = new Vector3[swarmSize];

        Array.Sort(xAxis);
        Array.Sort(yAxis);
        Array.Sort(zAxis);

        //Step 1. Create Datapoint objects based on the size of the swarm

        for (int i = 0; i < swarmSize; i++)
        {
            var datapoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
            swarmDataPoints[i] = datapoint;
            datapoint.transform.SetParent(transform);
            datapoint.transform.rotation = Quaternion.identity;
            datapoint.transform.localScale = new Vector3(plotSize/10, plotSize / 10, plotSize / 10);
            //Instantiate(dataPointPrefab, Vector3.zero, Quaternion.identity, transform);
            swarmDataPoints[i].name = "Data Point ";
            Renderer dataPointRenderer = swarmDataPoints[i].GetComponent<Renderer>();

            //Eventually set by header/Category
            dataPointRenderer.material.color = Color.blue;
            //Color.HSVToRGB((float)i / (engagementDates.Length - 1), 1f, 1f); 
        }

        float xPos;
        float yPos;
        float zPos;

        //Step 2. Categorise data to determine how we calculate position
        //X-Axis
        if (IsNumeric(xAxis[0]))
        {
            Debug.Log("Dealing with numbers...calculating");

            // Numeric data, calculate position based on range
            float minValue = GetMinNumericValue(xAxis);
            float maxValue = GetMaxNumericValue(xAxis);

            for (int i = 0; i < swarmSize; i++)
            {
                float numericValue = float.Parse(xAxis[i]);
                xPos = GetGraphDataPointPosition(Mathf.InverseLerp(minValue, maxValue, numericValue));
                positionMatrix[i].x = xPos;
            }

        }
        else if (IsDate(xAxis[0]))
        {
            Debug.Log("Dealing with dates...calculating range");
            // Date data, calculate position based on range of dates
            DateTime minDate = GetMinDateValue(xAxis);
            DateTime maxDate = GetMaxDateValue(xAxis);

            TimeSpan dateRange = maxDate - minDate;

            for (int i = 0; i < swarmSize; i++)
            {
                DateTime dateValue = DateTime.ParseExact(xAxis[i], "dd/MM/yyyy", null);
                TimeSpan dateDifference = dateValue - minDate;
                xPos = GetGraphDataPointPosition((float)dateDifference.TotalDays / (float)dateRange.TotalDays);
                positionMatrix[i].x = xPos;
            }
        }
        else
        {
            Debug.Log("Dealing with categorys...creating hashset");

            // Category data, calculate position based on number of unique categories
            HashSet<string> categories = GetUniqueCategories(xAxis);
            for (int i = 0; i < swarmSize; i++)
            {
                int categoryIndex = GetCategoryIndex(categories, xAxis[i]);
                xPos = GetGraphDataPointPosition((float)categoryIndex / (float)(categories.Count - 1));
                positionMatrix[i].x = xPos;
            }
        }


        //Y-Axis
        if (IsNumeric(yAxis[0]))
        {
            Debug.Log("Dealing with numbers...calculating");

            // Numeric data, calculate position based on range
            float minValue = GetMinNumericValue(yAxis);
            float maxValue = GetMaxNumericValue(yAxis);

            for (int i = 0; i < swarmSize; i++)
            {
                float numericValue = float.Parse(yAxis[i]);
                yPos = GetGraphDataPointPosition(Mathf.InverseLerp(minValue, maxValue, numericValue));
                positionMatrix[i].y = yPos;
            }

        }
        else if (IsDate(yAxis[0]))
        {
            Debug.Log("Dealing with dates...calculating range");
            // Date data, calculate position based on range of dates
            DateTime minDate = GetMinDateValue(yAxis);
            DateTime maxDate = GetMaxDateValue(yAxis);

            TimeSpan dateRange = maxDate - minDate;

            for (int i = 0; i < swarmSize; i++)
            {
                DateTime dateValue = DateTime.ParseExact(yAxis[i], "dd/MM/yyyy", null);
                TimeSpan dateDifference = dateValue - minDate;
                yPos = GetGraphDataPointPosition((float)dateDifference.TotalDays / (float)dateRange.TotalDays);
                positionMatrix[i].y = yPos;
            }
        }
        else
        {
            Debug.Log("Dealing with categorys...creating hashset");

            // Category data, calculate position based on number of unique categories
            HashSet<string> categories = GetUniqueCategories(yAxis);
            for (int i = 0; i < swarmSize; i++)
            {
                int categoryIndex = GetCategoryIndex(categories, yAxis[i]);
                yPos = GetGraphDataPointPosition((float)categoryIndex / (float)(categories.Count - 1));
                positionMatrix[i].y = yPos;
            }
        }

        //Z-Axis
        if (IsNumeric(zAxis[0]))
        {
            Debug.Log("Dealing with numbers...calculating");

            // Numeric data, calculate position based on range
            float minValue = GetMinNumericValue(zAxis);
            float maxValue = GetMaxNumericValue(zAxis);

            for (int i = 0; i < swarmSize; i++)
            {
                float numericValue = float.Parse(zAxis[i]);
                zPos = GetGraphDataPointPosition(Mathf.InverseLerp(minValue, maxValue, numericValue));
                positionMatrix[i].z = zPos;
            }

        }
        else if (IsDate(zAxis[0]))
        {
            Debug.Log("Dealing with dates...calculating range");
            // Date data, calculate position based on range of dates
            DateTime minDate = GetMinDateValue(zAxis);
            DateTime maxDate = GetMaxDateValue(zAxis);

            TimeSpan dateRange = maxDate - minDate;

            for (int i = 0; i < swarmSize; i++)
            {
                DateTime dateValue = DateTime.ParseExact(zAxis[i], "dd/MM/yyyy", null);
                TimeSpan dateDifference = dateValue - minDate;
                zPos = GetGraphDataPointPosition((float)dateDifference.TotalDays / (float)dateRange.TotalDays);
                positionMatrix[i].z = zPos;
            }
        }
        else
        {
            Debug.Log("Dealing with categorys...creating hashset");

            // Category data, calculate position based on number of unique categories
            HashSet<string> categories = GetUniqueCategories(zAxis);
            for (int i = 0; i < swarmSize; i++)
            {
                int categoryIndex = GetCategoryIndex(categories, zAxis[i]);
                zPos = GetGraphDataPointPosition((float)categoryIndex / (float)(categories.Count - 1));
                positionMatrix[i].z = zPos;
            }
        }

        for (int i = 0; i < swarmSize; i++)
        {
            swarmDataPoints[i].transform.position = positionMatrix[i] + transform.position;
        }
        // Instantiate the data point object at the calculated position
        // Assign a unique color to each data point
        // Add a label or other visual representation to the data point if desired
        // You can attach text objects, sprites, or other game objects to represent additional information
        dataconfigPanel.SetToolActions(gameObject);
    }

    #region Scatterplot functions
    private void CreateAxisLines()
    {
        //length of lines will be based on plot axis, scaled by 100 to create 3d graph axes
        float mLineScale = plotSize / 100;
        float mLabelScale = plotSize / 100;
        Vector3 labelScale = new Vector3(mLabelScale, mLabelScale, mLabelScale);

        // X-axis line
        xAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        float offset = plotSize / 2f;
        xAxis.transform.parent = transform;
        xAxis.transform.localScale = new Vector3(plotSize, mLineScale, mLineScale);
        xAxis.transform.localPosition = new Vector3(0f, -offset, -offset);
        xAxis.GetComponent<Renderer>().material.color = Color.red;

        // Add X-axis label
        GameObject xLabel = new GameObject("X Axis Label");
        xLabel.transform.parent = transform;
        xLabel.transform.localPosition = new Vector3(offset, -offset, -offset);
        xLabelTextMesh = xLabel.AddComponent<TextMesh>();
        SetLabel(xLabelTextMesh, AssignLabels(Axis.x));
        xLabelTextMesh.anchor = TextAnchor.MiddleLeft;
        xLabelTextMesh.fontSize = 100;
        xLabelTextMesh.color = Color.black;
        xLabelTextMesh.transform.localScale = labelScale;


        // Y-axis line
        yAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        yAxis.transform.parent = transform;
        yAxis.transform.localScale = new Vector3(mLineScale, plotSize, mLineScale);
        yAxis.transform.localPosition = new Vector3(-offset, 0f, -offset);
        yAxis.GetComponent<Renderer>().material.color = Color.green;

        // Add Y-axis label
        GameObject yLabel = new GameObject("Y Axis Label");
        yLabel.transform.parent = transform;
        yLabel.transform.localPosition = new Vector3(-offset, offset, -offset);
        yLabelTextMesh = yLabel.AddComponent<TextMesh>();
        SetLabel(yLabelTextMesh, AssignLabels(Axis.y));
        yLabelTextMesh.anchor = TextAnchor.MiddleRight;
        yLabelTextMesh.fontSize = 100;
        yLabelTextMesh.color = Color.black;
        yLabelTextMesh.transform.localScale = labelScale;

        // Z-axis line
        zAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zAxis.transform.parent = transform;
        zAxis.transform.localScale = new Vector3(mLineScale, mLineScale, plotSize);
        zAxis.transform.localPosition = new Vector3(-offset, -offset, 0f);
        zAxis.GetComponent<Renderer>().material.color = Color.blue;

        // Add Z-axis label
        GameObject zLabel = new GameObject("Z Axis Label");
        zLabel.transform.parent = transform;
        zLabel.transform.localPosition = new Vector3(-offset, -offset, offset);
        zLabelTextMesh = zLabel.AddComponent<TextMesh>();
        SetLabel(zLabelTextMesh, AssignLabels(Axis.z));
        zLabelTextMesh.anchor = TextAnchor.MiddleRight;
        zLabelTextMesh.fontSize = 100;
        zLabelTextMesh.color = Color.black;
        zLabelTextMesh.transform.localScale = labelScale;


    }
    private float GetGraphDataPointPosition(float normalisedAxisPos)
    {
        var graphPos = Mathf.Lerp((-plotSize * 0.9f) / 2f, (plotSize * 0.9f) / 2f, normalisedAxisPos);
        return graphPos;
    }

    public bool IsNumeric(string value)
    {
        float result;
        return float.TryParse(value, out result);
    }

    public float GetMinNumericValue(string[] data)
    {
        float minValue = float.MaxValue;

        foreach (string value in data)
        {
            float numericValue;
            if (float.TryParse(value, out numericValue))
            {
                if (numericValue < minValue)
                    minValue = numericValue;
            }
        }

        return minValue;
    }

    public float GetMaxNumericValue(string[] data)
    {
        float maxValue = float.MinValue;

        foreach (string value in data)
        {
            float numericValue;
            if (float.TryParse(value, out numericValue))
            {
                if (numericValue > maxValue)
                    maxValue = numericValue;
            }
        }

        return maxValue;
    }

    public bool IsDate(string value)
    {
        DateTime dateValue;
        return DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out dateValue);
    }

    public DateTime GetMinDateValue(string[] data)
    {
        DateTime minDate = DateTime.MaxValue;

        foreach (string value in data)
        {
            DateTime dateValue;
            if (DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out dateValue))
            {
                if (dateValue < minDate)
                    minDate = dateValue;
            }
        }

        return minDate;
    }

    public DateTime GetMaxDateValue(string[] data)
    {
        DateTime maxDate = DateTime.MinValue;

        foreach (string value in data)
        {
            DateTime dateValue;
            if (DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out dateValue))
            {
                if (dateValue > maxDate)
                    maxDate = dateValue;
            }
        }

        return maxDate;
    }

    public HashSet<string> GetUniqueCategories(string[] data)
    {
        HashSet<string> categories = new HashSet<string>();

        foreach (string value in data)
        {
            categories.Add(value);
        }

        return categories;
    }

    public int GetCategoryIndex(HashSet<string> categories, string value)
    {
        string[] sortedCategories = categories.OrderBy(x => x).ToArray();
        return Array.IndexOf(sortedCategories, value);
    }

    #endregion

    #region Label Functions
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
    #endregion
}
