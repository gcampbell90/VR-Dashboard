using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class NetworkGraphController : MonoBehaviour
{

    [Header("Enable ComputeShader Method")]
    [Tooltip("*Not Completed*")]
    [SerializeField] private bool UseComputeShader;

    [SerializeField] GameObject nodePrefab;
    [SerializeField] ComputeShader GraphShader;

    [SerializeField] private TextAsset _dataFile;
    public Button randomiseBtn;
 
    void Start()
    {
        DataParser parser = new DataParser();

        LookAtController lookatController = gameObject.AddComponent<LookAtController>();

        IParsable data = parser.ParseTextAsset(_dataFile);

        UIController uiController = GetComponent<UIController>();

        if (UseComputeShader)
        {
            var frGraph = gameObject.AddComponent<FRForceGraphComputeShader>();
            //var frGraph = gameObject.AddComponent<FRForceGraphComputeShader>();

            frGraph.graphCS = GraphShader;
            frGraph.NodePrefab = nodePrefab;
            frGraph.networkGraphData = data;

            frGraph.Init();
            randomiseBtn.onClick.AddListener(frGraph.MoveRandomNodeRecalulate);
        }
        //Fructerman-Reingold approach
        else if (!UseComputeShader)
        {
            //WorkingScript
            var frGraph = gameObject.AddComponent<FRForceGraph>();

            frGraph.NodePrefab = nodePrefab;
            frGraph.CreateFRGraph(transform, data, uiController, lookatController);
            randomiseBtn.onClick.AddListener(frGraph.MoveRandomNodeRecalulate);
        }

        //Prev physics based approach using springjoints
        //else if (ForceDirected == true)
        //{
        //    //Debug.Log("Not Yet Implemented");
        //    var forcedirectedGraph = gameObject.AddComponent<NetworkGraph_ForceDirected>();
        //    forcedirectedGraph.file = _dataFile;
        //    forcedirectedGraph.nodepf = nodePrefab;
        //    forcedirectedGraph.edgepf = edgePrefab;
        //    forcedirectedGraph.length = scale;
        //    forcedirectedGraph.width = scale;
        //    forcedirectedGraph.height = scale;
        //    forcedirectedGraph.Init();
        //}

    }
}
