using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkGraph_Spherical : MonoBehaviour
{
    public GameObject NodePrefab { get; set; }
    public float Scale { get; set; }
    public float LineWidth { get; set; }
    public float NodeSize { get; set; }

    public void CreateSphericalGraph(Transform transform, IParsable networkGraphData, UIController uiController, LookAtController lookatController)
    {
        Vector3 m_origin = transform.position;
        Vector3 m_scale = Vector3.one;
        // Create nodes in graph
        foreach (Node node in networkGraphData.nodes.Values)
        {
            node.Position = GetPosition(networkGraphData.nodes, out float _posOffset, node, ref m_origin);

            // Create node object
            node.nodeObject = Instantiate(NodePrefab, node.Position, Quaternion.identity, transform);

            //Manually removing the rb and collider for now.
            //TODO: update force-graph methods to add these instead, if necessary 
            var rb = node.nodeObject.GetComponent<Rigidbody>();
            var col = node.nodeObject.GetComponent<BoxCollider>();
            Destroy(rb);
            Destroy(col);


            SetVisuals(ref node.nodeObject, node.Tag);

            //Send these 'source' nodes to UI controller
            if (node.Tag == "Country")
            {
                //    if (node.ConnectionCount > 0)
                uiController.CreateButton(node);
            }
            var nodeTextObj = node.nodeObject.GetComponentInChildren<TextMeshPro>();
            nodeTextObj.text = node.Name;

            // Add TextObj transform to lookat controller
            lookatController.objectsToRotate.Add(nodeTextObj.gameObject.transform);
        }

        CreateLines(networkGraphData.nodes, networkGraphData.edges);
    }

    private void SetVisuals(ref GameObject nodeObject,string tag)
    {
        var mat = nodeObject.GetComponent<Renderer>().material;
        var m_col = new Color();
        float m_scale = 1;
        switch (tag)
        {
            case "Country":
                m_col = Color.blue;
                m_scale = 2;
                break;
            case "Application":
                m_col = Color.yellow;
                m_scale = 1;
                break;
            case "Control":
                m_col = Color.red;
                m_scale = 1;
                break;
            default:
                m_col = Color.magenta;
                break;

        }
        mat.color = m_col;
        mat.SetColor("_EmissionColor", m_col);
        nodeObject.transform.localScale *= (NodeSize * m_scale
           //+ node.Connections.Count
           );
    }

    private Vector3 GetPosition(Dictionary<int, Node> nodes, out float _posOffset, Node node, ref Vector3 _origin)
    {
        //set variable offset based on chosen rows/ids
        //calculate position based off if node is of type tag
        switch (node.Tag)
        {
            case "Country":
                _posOffset = 10f;
                break;
            case "Application":
                _posOffset = 5f;
                _origin = GetNodeOrigin(ref nodes, node.Id);
                break;
            case "Control":
                _posOffset = 1f;
                _origin = GetNodeOrigin(ref nodes, node.Id);
                break;
            default:
                _posOffset = 1f;
                break;
        }

        var nodePos = _origin + (Random.onUnitSphere * Scale * _posOffset);
        //Debug.Log($"{node.Tag}'s position is set as {nodePos}");

        return nodePos;
    }
    private Vector3 GetNodeOrigin(ref Dictionary<int, Node> nodes, int id)
    {
        foreach (KeyValuePair<int, Node> node in nodes)
        {
            if (node.Value.Connections == null) break;
            foreach (int integer in node.Value.Connections)
            {
                if (integer == id)
                {
                    //Debug.Log($"Setting position offset for {nodes[id].Tag} to parent {nodes[node.Key].Tag} at position {nodes[node.Key].Position}");
                    return nodes[node.Key].Position;
                }
            }
        }
        return Vector3.zero;
    }
    private void CreateLines(Dictionary<int, Node> nodes, List<Edge> edges)
    {
        foreach (var edge in edges)
        {
            GameObject lineObject = new GameObject(); // Create a new GameObject to hold the LineRenderer
            LineRenderer line = lineObject.AddComponent<LineRenderer>(); // Add a LineRenderer to the GameObject
            line.material = new Material(Shader.Find("Sprites/Default"));

            // Set properties for the LineRenderer
            line.positionCount = 2; // We have 2 positions: start and end
            line.startWidth = LineWidth; // Adjust as needed
            line.endWidth = LineWidth; // Adjust as needed
            line.useWorldSpace = true; // Use world space coordinates

            // Fetch the nodes that this edge connects
            Node sourceNode = nodes[edge.source];
            Node targetNode = nodes[edge.target];

            // Set the positions of the LineRenderer
            line.SetPosition(0, sourceNode.nodeObject.transform.position); // Start at the source node's position
            line.SetPosition(1, targetNode.nodeObject.transform.position); // End at the target node's position
        }
    }
}


