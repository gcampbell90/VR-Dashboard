using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class FRForceGraph : MonoBehaviour
{

    //FR Graph properties
    public float area = 10.0f; //Size of node spawn area
    public float gravitationalConstant => 0.5f; //Strength nodes are attracted to center
    public float maxAttractionForce => area / 5; //Strength nodes pull eachother together
    public float maxRepulsionForce => area / 5; //Strength nodes push eachother apart
    public float damping = 0.9f;
    public int maxIterations = 1000;
    public int currentIteration = 0;
    public float convergenceThreshold = 0.5f;
    public float positionChangeThreshold = 1f;

    private int convergenceCounter = 0;
    private const int MAX_CONVERGENCE_COUNT = 10;

    //Cached data
    private Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    // Fetches a node by its key. Returns null if not found.
    private Node GetNode(int key)
    {
        nodes.TryGetValue(key, out Node node);
        return node;
    }

    List<Edge> tmpEdgesTest = new List<Edge>();
    private Dictionary<Node, Vector3> previousNodePositions = new Dictionary<Node, Vector3>();

    //Node visual properties
    public GameObject NodePrefab;
    private Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
    private Dictionary<string, (Color color, float scale)> tagProperties = new Dictionary<string, (Color, float)>{
        { "Country", (Color.blue, 1f) },
        { "Application", (Color.yellow, .5f) },
        { "Control", (Color.red, 0.25f) },
    };
    private Material nodeMat;
    public Material lineMaterial;


    private void Start()
    {
        nodeMat = NodePrefab.GetComponent<Renderer>().sharedMaterial;
    }

    #region FR Graph Methods
    public void CreateFRGraph(Transform transform, IParsable networkGraphData, UIController uiController, LookAtController lookatController)
    {
        //cache data
        nodes = networkGraphData.nodes;

        // Create nodes 
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = GetNode(i);
            node.Position = new Vector3(
                Random.Range(-area, area),
                Random.Range(-area, area),
                Random.Range(-area, area)) + transform.position;

            // Create node object
            node.nodeObject = Instantiate(NodePrefab, node.Position, Quaternion.identity, transform);

            SetVisuals(node.nodeObject, node.Tag);

            CreateButtons(uiController, node);

            var nodeTextObj = node.nodeObject.GetComponentInChildren<TextMeshPro>();
            nodeTextObj.text = node.Name;
        }


        //Set Edges
        //GenerateEdges(networkGraphData.nodes, networkGraphData.edges);

        for (int i = 0; i < networkGraphData.edges.Count; i++)
        {
            networkGraphData.edges[i].nodeA = GetNode(networkGraphData.edges[i].source);
            networkGraphData.edges[i].nodeB = GetNode(networkGraphData.edges[i].target);
        }
        tmpEdgesTest = networkGraphData.edges;

        StartCoroutine(GraphLayoutCoroutine());

    }

    internal void MoveRandomNodeRecalulate()
    {
        GetNode(0).nodeObject.transform.position = new Vector3(100, 100, 100);
        GetNode(0).Position = GetNode(0).nodeObject.transform.position;
        StartCoroutine(GraphLayoutCoroutine());
    }

    IEnumerator GraphLayoutCoroutine()
    {
        while (currentIteration < maxIterations)
        {
            PerformIteration();
            yield return new WaitForEndOfFrame();
        }
    }

    private void PerformIteration()
    {
        bool allNodesConverged = true;

        RepulseNodes();
        AttractNodes();

        Node node = null;
        for (int i = 0; i < nodes.Count; i++)
        {
            node = GetNode(i);
            Vector3 oldPosition = node.nodeObject.transform.localPosition;

            node.Position += CalculateGravitationalForce(node.Position);
            node.nodeObject.transform.localPosition = node.Position;

            if ((node.Position - oldPosition).magnitude > convergenceThreshold)
            {
                allNodesConverged = false;
            }
        }
        if (allNodesConverged)
        {
            convergenceCounter++;
            if (convergenceCounter >= MAX_CONVERGENCE_COUNT)
            {
                Debug.Log("NodesConverged- Stopping Coroutine");
                StopAllCoroutines();
                return;
            }
        }
        else
        {
            convergenceCounter = 0; // reset counter if any node moves significantly
        }

        currentIteration++;
    }

    void RepulseNodes()
    {
        Node nodeAEntry = null;
        Node nodeBEntry = null;

        for (int i = 0; i < nodes.Count; i++)
        {
            nodeAEntry = GetNode(i);
            for (int j = 0; j < nodes.Count; j++)
            {
                nodeBEntry = GetNode(j);
                //if (ShouldRecalculateRepulsion(nodeAEntry, nodeBEntry))
                //{
                Vector3 direction = nodeAEntry.Position - nodeBEntry.Position;
                float distance = direction.magnitude;

                if (distance < 0.01f) distance = 0.01f;

                Vector3 repulsionForce = (direction.normalized / distance) * (area / distance);
                repulsionForce = Vector3.ClampMagnitude(repulsionForce, maxRepulsionForce);
                repulsionForce *= damping;

                nodeAEntry.Position += repulsionForce;
                nodeBEntry.Position -= repulsionForce;

                previousNodePositions[nodeAEntry] = nodeAEntry.Position;
                previousNodePositions[nodeBEntry] = nodeBEntry.Position;
                //}
            }
        }
        //foreach (var nodeAEntry in nodes)
        //{
        //    foreach (var nodeBEntry in nodes)
        //    {
        //        if (nodeAEntry != nodeBEntry)
        //        {
        //            //if (ShouldRecalculateRepulsion(nodeAEntry, nodeBEntry))
        //            //{
        //            Vector3 direction = nodeAEntry.Position - nodeBEntry.Position;
        //            float distance = direction.magnitude;

        //            if (distance < 0.01f) distance = 0.01f;

        //            Vector3 repulsionForce = (direction.normalized / distance) * (area / distance);
        //            repulsionForce = Vector3.ClampMagnitude(repulsionForce, maxRepulsionForce);
        //            repulsionForce *= damping;

        //            nodeAEntry.Position += repulsionForce;
        //            nodeBEntry.Position -= repulsionForce;

        //            previousNodePositions[nodeAEntry] = nodeAEntry.Position;
        //            previousNodePositions[nodeBEntry] = nodeBEntry.Position;
        //            //}
        //        }
        //    }
        //}
    }
    void AttractNodes()
    {
        foreach (Edge edge in tmpEdgesTest)
        {
            Vector3 direction = edge.nodeA.Position - edge.nodeB.Position;
            float distance = direction.magnitude;

            if (distance < 0.01f) distance = 0.01f;

            Vector3 attractionForce = direction.normalized * (distance * distance / area);
            attractionForce = Vector3.ClampMagnitude(attractionForce, maxAttractionForce);
            attractionForce *= damping;

            edge.nodeA.Position -= attractionForce;
            edge.nodeB.Position += attractionForce;
        }
    }
    bool ShouldRecalculateRepulsion(Node nodeA, Node nodeB)
    {
        Vector3 previousPositionA, previousPositionB;

        if (previousNodePositions.TryGetValue(nodeA, out previousPositionA) && previousNodePositions.TryGetValue(nodeB, out previousPositionB))
        {
            float distanceChangeA = Vector3.Distance(nodeA.Position, previousPositionA);
            float distanceChangeB = Vector3.Distance(nodeB.Position, previousPositionB);

            return distanceChangeA > positionChangeThreshold || distanceChangeB > positionChangeThreshold;
        }

        return true;  // Default to true for the first frame or if the nodes aren't in the dictionary.
    }
    Vector3 CalculateGravitationalForce(Vector3 position)
    {
        Vector3 gravitationalForce = -position.normalized * gravitationalConstant;
        return gravitationalForce;
    }

    #region Line Creation Methods
    //Linerenderermethod
    void GenerateEdges(Dictionary<int, Node> nodes, List<Edge> edges)
    {
        Material m_lrMat = new Material(Shader.Find("Sprites/Default"));
        for (int i = 0; i < edges.Count; i++)
        {
            edges[i].nodeA = nodes[edges[i].source];
            edges[i].nodeB = nodes[edges[i].target];
        }

        foreach (Edge edge in edges)
        {
            GameObject lineObj = new GameObject("EdgeLine");
            lineObj.transform.SetParent(transform);
            edge.lineRenderer = lineObj.AddComponent<LineRenderer>();
            edge.lineRenderer.positionCount = 2;
            edge.lineRenderer.startWidth = 0.1f; // adjust as necessary
            edge.lineRenderer.endWidth = 0.1f;   // adjust as necessary
            edge.lineRenderer.sharedMaterial = m_lrMat;
            edge.lineRenderer.startColor = Color.blue;  // adjust as necessary
            edge.lineRenderer.endColor = Color.red;    // adjust as necessary
        }
    }

    //GL method
    void GenerateGLLine()
    {
        //Debug.Log("Creating Line");
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");

            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            lineMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 1);
        }
    }
    void OnRenderObject()
    {

        //Debug.Log("RenderingLines");
        GenerateGLLine();
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        // Set the material to be used for the next rendering
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        // Set drawing color
        GL.Color(new Color(1, 1, 1, 0.5f));

        // Define the lines
        foreach (var edge in tmpEdgesTest)
        {
            GL.Vertex3(edge.nodeA.Position.x, edge.nodeA.Position.y, edge.nodeA.Position.z);
            GL.Vertex3(edge.nodeB.Position.x, edge.nodeB.Position.y, edge.nodeB.Position.z);
        }

        GL.End();
        GL.PopMatrix();
    }
    #endregion

    #endregion

    #region Node Colours and Scaling

    private static void CreateButtons(UIController uiController, Node node)
    {
        //Send these 'source' nodes to UI controller
        if (node.Tag == "Country")
        {
            //    if (node.ConnectionCount > 0)
            uiController.CreateButton(node);
        }
    }
    private void SetVisuals(GameObject nodeObject, string tag)
    {
        if (!tagProperties.ContainsKey(tag))
        {
            tag = "Default";
        }

        var renderer = nodeObject.GetComponent<Renderer>();
        Color m_col = tagProperties[tag].color;
        float m_scale = tagProperties[tag].scale;

        Material mat = GetSharedMaterialForTag(tag);
        renderer.sharedMaterial = mat;

        mat.color = m_col;
        mat.SetColor("_EmissionColor", m_col);
        nodeObject.transform.localScale *= m_scale;
    }
    private Material GetSharedMaterialForTag(string tag)
    {
        if (materialCache.ContainsKey(tag))
        {
            return materialCache[tag];
        }

        if (nodeMat != null)
        {
            Material newMat = new Material(nodeMat);

            materialCache[tag] = newMat;
            return newMat;
        }
        else
        {
            Material newMat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));

            materialCache[tag] = newMat;
            return newMat;
        }

    }

    #endregion
}
