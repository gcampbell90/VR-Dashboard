using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct RepulseJob : IJobParallelFor
{
    public NativeArray<Vector3> nodePositions;
    public float maxRepulsionForce;
    public float area;
    public float damping;

    public void Execute(int index)
    {
        Vector3 currentPosition = nodePositions[index];
        Vector3 totalRepulsion = Vector3.zero;

        for (int j = 0; j < nodePositions.Length; j++)
        {
            if (index != j)
            {
                Vector3 direction = currentPosition - nodePositions[j];
                float distance = direction.magnitude;

                if (distance < 0.01f) distance = 0.01f;

                Vector3 repulsionForce = (direction.normalized / distance) * (area / distance);
                repulsionForce = Vector3.ClampMagnitude(repulsionForce, maxRepulsionForce);
                repulsionForce *= damping;

                totalRepulsion += repulsionForce;
            }
        }

        nodePositions[index] += totalRepulsion;
    }
}

public struct AttractJob : IJob
{
    public NativeArray<Vector3> edgeNodeAPositions;
    public NativeArray<Vector3> edgeNodeBPositions;
    public float maxAttractionForce;
    public float area;
    public float damping;

    public void Execute()
    {
        for (int i = 0; i < edgeNodeAPositions.Length; i++)
        {
            Vector3 direction = edgeNodeAPositions[i] - edgeNodeBPositions[i];
            float distance = direction.magnitude;

            if (distance < 0.01f) distance = 0.01f;

            Vector3 attractionForce = direction.normalized * (distance * distance / area);
            attractionForce = Vector3.ClampMagnitude(attractionForce, maxAttractionForce);
            attractionForce *= damping;

            edgeNodeAPositions[i] -= attractionForce;
            edgeNodeBPositions[i] += attractionForce;
        }
    }
}
