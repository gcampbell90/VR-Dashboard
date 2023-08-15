using UnityEngine;

public class Edge
{
    public int source;
    public int target;
    public string label;
    public LineRenderer lineRenderer;
    
    public Node nodeA;
    public Node nodeB;

    public Edge()
    {
    }

    public Edge(Node a, Node b) { nodeA = a; nodeB = b; }
}