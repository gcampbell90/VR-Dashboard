using UnityEngine;

public class Simple3DChart : MonoBehaviour
{
    public GameObject barPrefab;
    public Vector3[] dataPoints; // Sample data points

    private void Start()
    {
        CreateBars();
    }

    private void CreateBars()
    {
        for (int i = 0; i < dataPoints.Length; i++)
        {
            Vector3 position = new Vector3(i, dataPoints[i].y / 2f, 0f); // Adjust the position based on the data
            GameObject bar = Instantiate(barPrefab, position, Quaternion.identity, transform);

            // Set the scale of the bar based on the data value
            Vector3 scale = new Vector3(0.8f, dataPoints[i].y, 0.8f); // Adjust the scale based on the data
            bar.transform.localScale = scale;

            // Set the color of the bar based on the data value
            Renderer barRenderer = bar.GetComponent<Renderer>();
            barRenderer.material.color = Color.Lerp(Color.blue, Color.red, dataPoints[i].y / 10f); // Adjust the color based on the data
        }
    }
}
