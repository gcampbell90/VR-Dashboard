using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class FRForceGraphComputeShader : MonoBehaviour
{
    public ComputeShader graphCS;

    private float area = 100.0f; // Set a default area value; you might want to adjust this

    Dictionary<int, Vector3> nodes = new Dictionary<int, Vector3>();
    List<GameObject> nodeObjects = new List<GameObject>();
    List<Vector2Int> edges = new List<Vector2Int>();

    float maxDist;
    float curMaxDist;
    float k;

    Node[] nodeData;
    ComputeBuffer _nodeBuffer;
    ComputeBuffer _edgesBuffer;
    Vector2[] edgesData;

    int kernel_fruchtermanReingold;

    public IParsable networkGraphData;

    //Node visual properties
    public GameObject NodePrefab;
    public Material lineMaterial;
    private Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
    private Dictionary<string, (Color color, float scale)> tagProperties = new Dictionary<string, (Color, float)>{
        { "Country", (Color.blue, 5f) },
        { "Application", (Color.yellow, 2f) },
        { "Control", (Color.red, 1f) },
    };
    private Material nodeMat;

    public void Init()
    {
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        PopulateDataFromNetworkGraph();
        InitShader();

        StartCoroutine(RunShaderCoroutine());
    }


    void PopulateDataFromNetworkGraph()
    {
        for (int i = 0; i < networkGraphData.nodes.Count; i++)
        {
            var position = new Vector3(
                Random.Range(-area, area),
                Random.Range(-area, area),
                Random.Range(-area, area));

            nodes.Add(i, position);
            var nodeObj = Instantiate(NodePrefab, nodes[i], Quaternion.identity, transform);
            nodeObj.name = $"{networkGraphData.nodes[i].Name}";
            nodeObj.GetComponentInChildren<TextMeshPro>().text = nodeObj.name;
            nodeObjects.Add(nodeObj);

            SetVisuals(nodeObj, networkGraphData.nodes[i].Tag);
        }

        for (int i = 0; i < networkGraphData.edges.Count; i++)
        {
            edges.Add(new Vector2Int(networkGraphData.edges[i].source, networkGraphData.edges[i].target));
        }

        // Convert dictionaries and lists to arrays for compute shader
        nodeData = new Node[nodes.Count];
        int index = 0;
        foreach (var node in nodes)
        {
            nodeData[index] = new Node { pos = node.Value, isAnchored = 0f };
            index++;
        }

        edgesData = new Vector2[edges.Count];
        for (int i = 0; i < edges.Count; i++)
        {
            edgesData[i] = edges[i];
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

    void InitShader()
    {
        k = Mathf.Sqrt(area / nodeObjects.Count) * 5f;

        maxDist = Mathf.Sqrt(area / 10) * 5f;
        curMaxDist = maxDist;

        // mandatory variable 
        graphCS.SetInt("Iterations", 10);    
        graphCS.SetFloat("k", k);        
        graphCS.SetFloat("maxDist", maxDist);

        _edgesBuffer = new ComputeBuffer(edgesData.Length, sizeof(float) * 2);  // how many bytes a single element in the buffer is, >>  5 float variables in the struct -> 1x float3 1xbool  +1x float
        _edgesBuffer.SetData(edgesData);

        graphCS.SetBuffer(kernel_fruchtermanReingold, "edgeBuffer", _edgesBuffer);
    }

    private IEnumerator RunShaderCoroutine()
    {
        while (curMaxDist > 0.01)
        {
            curMaxDist *= .9f;

            graphCS.SetFloat("maxDist", curMaxDist);
            kernel_fruchtermanReingold = graphCS.FindKernel("FruchtermanReingold");

            _nodeBuffer = new ComputeBuffer(nodeData.Length, sizeof(float) * 3 + sizeof(uint) + sizeof(float));
            _nodeBuffer.SetData(nodeData);
            graphCS.SetBuffer(kernel_fruchtermanReingold, "nodeBuffer", _nodeBuffer);

            graphCS.Dispatch(kernel_fruchtermanReingold, nodeData.Length/64+1, 1, 1);

            _nodeBuffer.GetData(nodeData);

            for (int i = 0; i < nodeObjects.Count; i++)
            {
                nodeObjects[i].transform.localPosition = nodeData[i].pos;
            }

            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < nodeData.Length; i++)
        {
            nodeData[i].isAnchored = 0;
        }
        Debug.Log("Coroutine Finished");

        _nodeBuffer.Release();
        _edgesBuffer.Release();

    }

    public void MoveRandomNodeRecalulate()
    {
        int randNode = Random.Range(0, nodeObjects.Count);
        nodeObjects[randNode].transform.localPosition = new Vector3(Random.Range(0, 1000), Random.Range(0, 1000), Random.Range(0, 1000));
        nodes[randNode] = nodeObjects[randNode].transform.localPosition;
        nodeData[randNode].pos = nodeObjects[randNode].transform.localPosition;
        nodeData[randNode].isAnchored = 1f;
        InitShader();
        StartCoroutine(RunShaderCoroutine());
    }


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

        for (int i = 0; i < networkGraphData.edges.Count; i++)
        {
            var pos1 = nodeObjects[networkGraphData.edges[i].source].transform.localPosition;
            var pos2= nodeObjects[networkGraphData.edges[i].target].transform.localPosition;

            GL.Vertex3(pos1.x,pos1.y,pos1.z);
            GL.Vertex3(pos2.x,pos2.y,pos2.z);
        }

        GL.End();
        GL.PopMatrix();
    }


    private void OnDisable()
    {
        StopAllCoroutines();
        _edgesBuffer.Release();
        _nodeBuffer.Release();
    }

    struct Node
    {
        public Vector3 pos;
        public float _debug;
        public float isAnchored;
    }
}
