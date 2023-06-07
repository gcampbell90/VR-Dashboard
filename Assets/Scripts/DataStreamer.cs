using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataStreamer : MonoBehaviour
{
    public GameObject dataPointPrefab;
    public int chunkSize = 100;
    public float spawnInterval = 0.1f;

    private List<Transform> dataPointPool;
    private Queue<DataPoint> dataQueue;
    private bool isStreaming = false;

    private struct DataPoint
    {
        public Vector3 position;
        public Color color;
    }

    private void Start()
    {
        // Initialize data structures
        dataPointPool = new List<Transform>();
        dataQueue = new Queue<DataPoint>();

        // Start data streaming
        StartStreaming();
    }

    private void StartStreaming()
    {
        isStreaming = true;
        StartCoroutine(StreamData());
    }

    private void StopStreaming()
    {
        isStreaming = false;
        StopCoroutine(StreamData());
    }

    private IEnumerator StreamData()
    {
        while (isStreaming)
        {
            // Generate a chunk of data points
            for (int i = 0; i < chunkSize; i++)
            {
                // Generate random position and color
                Vector3 position = Random.insideUnitSphere;
                Color color = Random.ColorHSV();

                // Enqueue the data point
                dataQueue.Enqueue(new DataPoint { position = position, color = color });
            }

            // Spawn data points from the queue
            while (dataQueue.Count > 0)
            {
                // Retrieve a data point from the queue
                DataPoint dataPoint = dataQueue.Dequeue();

                // Spawn or reuse a data point object
                Transform dataPointTransform = GetOrCreateDataPoint();

                // Set position and color of the data point
                dataPointTransform.position = dataPoint.position;
                Renderer dataPointRenderer = dataPointTransform.GetComponent<Renderer>();
                dataPointRenderer.material.color = dataPoint.color;

                yield return new WaitForSeconds(spawnInterval);
            }

            // Wait for next chunk
            yield return null;
        }
    }

    private Transform GetOrCreateDataPoint()
    {
        Transform dataPointTransform = null;

        // Try to find a deactivated data point in the pool
        foreach (Transform dataPoint in dataPointPool)
        {
            if (!dataPoint.gameObject.activeSelf)
            {
                dataPointTransform = dataPoint;
                break;
            }
        }

        // If no deactivated data point is found, create a new one
        if (dataPointTransform == null)
        {
            GameObject newDataPoint = Instantiate(dataPointPrefab, transform);
            dataPointTransform = newDataPoint.transform;
            dataPointPool.Add(dataPointTransform);
        }

        // Activate the data point
        dataPointTransform.gameObject.SetActive(true);

        return dataPointTransform;
    }
}
