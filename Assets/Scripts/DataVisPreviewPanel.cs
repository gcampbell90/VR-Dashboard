using UnityEngine;
using TMPro;

public class DataVisPreviewPanel : MonoBehaviour
{
    public GameObject axisBarPrefab;
    public TextMeshPro labelPrefab;

    private GameObject xAxis;
    private GameObject yAxis;
    private GameObject zAxis;

    private void Start()
    {
        // Create the x, y, and z axis bars
        xAxis = CreateAxisBar(Vector3.right, Color.red);
        yAxis = CreateAxisBar(Vector3.up, Color.green);
        zAxis = CreateAxisBar(Vector3.forward, Color.blue);

        // Set the axis labels
        SetAxisLabels();
    }

    private GameObject CreateAxisBar(Vector3 direction, Color color)
    {
        GameObject axisBar = Instantiate(axisBarPrefab, transform);
        axisBar.transform.localScale = new Vector3(0.1f, 0.1f, 1f); // Adjust the scale as needed
        axisBar.transform.position = Vector3.zero;
        axisBar.transform.rotation = Quaternion.LookRotation(direction);
        axisBar.GetComponent<Renderer>().material.color = color;

        return axisBar;
    }

    private void SetAxisLabels()
    {
        // Create and position the x-axis label
        GameObject xAxisLabel = Instantiate(labelPrefab.gameObject, transform);
        xAxisLabel.transform.position = xAxis.transform.position + new Vector3(0f, -0.5f, 0f);
        xAxisLabel.GetComponent<TextMeshPro>().text = "X-axis";

        // Create and position the y-axis label
        GameObject yAxisLabel = Instantiate(labelPrefab.gameObject, transform);
        yAxisLabel.transform.position = yAxis.transform.position + new Vector3(-0.5f, 0f, 0f);
        yAxisLabel.GetComponent<TextMeshPro>().text = "Y-axis";

        // Create and position the z-axis label
        GameObject zAxisLabel = Instantiate(labelPrefab.gameObject, transform);
        zAxisLabel.transform.position = zAxis.transform.position + new Vector3(0f, -0.5f, 0f);
        zAxisLabel.GetComponent<TextMeshPro>().text = "Z-axis";
    }
}
