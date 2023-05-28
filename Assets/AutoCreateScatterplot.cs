using UnityEngine;

public class AutoCreateScatterplot : MonoBehaviour
{
    public GameObject dataPointPrefab;
    public int numPoints = 100;
    public float plotSize = 10f;
    public DataConfigurationPanel dataconfigPanel;

    TextMesh xLabelTextMesh, yLabelTextMesh, zLabelTextMesh;

    private void Start()
    {
        //CreateScatterplot();
        CreateAxisLines();
    }

    private void CreateScatterplot()
    {
        for (int i = 0; i < numPoints; i++)
        {
            float x = Random.Range(-plotSize / 2f, plotSize / 2f);
            float y = Random.Range(-plotSize / 2f, plotSize / 2f);
            float z = Random.Range(-plotSize / 2f, plotSize / 2f);

            Vector3 position = new Vector3(x, y, z);
            GameObject dataPoint = Instantiate(dataPointPrefab, position, Quaternion.identity, transform);
            dataPoint.name = "Data Point " + i;

            Renderer dataPointRenderer = dataPoint.GetComponent<Renderer>();
            dataPointRenderer.material.color = Color.Lerp(Color.blue, Color.red, Random.value); // Random color

            // Optional: Add labels or other data representation to the data points
            // You can attach text objects, sprites, or other game objects to represent additional information
        }
    }

    private void CreateAxisLines()
    {
        // X-axis line
        GameObject xAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
        xLabelTextMesh.text = dataconfigPanel.xAxis;
        xLabelTextMesh.anchor = TextAnchor.MiddleLeft;
        xLabelTextMesh.fontSize = 14;
        xLabelTextMesh.color = Color.black;

        // Y-axis line
        GameObject yAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        yAxis.transform.parent = transform;
        yAxis.transform.localScale = new Vector3(0.1f, plotSize, 0.1f);
        yAxis.transform.localPosition = new Vector3(-offset, 0f, -offset);
        yAxis.GetComponent<Renderer>().material.color = Color.green;

        // Add Y-axis label
        GameObject yLabel = new GameObject("Y Axis Label");
        yLabel.transform.parent = transform;
        yLabel.transform.localPosition = new Vector3(-offset, offset, -offset);
        yLabelTextMesh = yLabel.AddComponent<TextMesh>();
        yLabelTextMesh.text = dataconfigPanel.yAxis;
        yLabelTextMesh.anchor = TextAnchor.MiddleRight;
        yLabelTextMesh.fontSize = 14;
        yLabelTextMesh.color = Color.black;

        // Z-axis line
        GameObject zAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zAxis.transform.parent = transform;
        zAxis.transform.localScale = new Vector3(0.1f, 0.1f, plotSize);
        zAxis.transform.localPosition = new Vector3(-offset, -offset, 0f);
        zAxis.GetComponent<Renderer>().material.color = Color.blue;

        // Add Z-axis label
        GameObject zLabel = new GameObject("Z Axis Label");
        zLabel.transform.parent = transform;
        zLabel.transform.localPosition = new Vector3(-offset, -offset, offset);
        zLabelTextMesh = zLabel.AddComponent<TextMesh>();
        zLabelTextMesh.text = dataconfigPanel.zAxis;
        zLabelTextMesh.anchor = TextAnchor.MiddleRight;
        zLabelTextMesh.fontSize = 14;
        zLabelTextMesh.color = Color.black;
    }

    public void UpdateLabels()
    {
        xLabelTextMesh.text = dataconfigPanel.xAxis;
        yLabelTextMesh.text = dataconfigPanel.yAxis;
        zLabelTextMesh.text = dataconfigPanel.zAxis;
    }
}
