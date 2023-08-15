using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CSVData : IParsable
{
    public Dictionary<int, Node> nodes;
    public List<Edge> edges;
    Dictionary<int, Node> IParsable.nodes => nodes;
    List<Edge> IParsable.edges => edges;

    public CSVData()
    {
        nodes = new Dictionary<int, Node>();
        edges = new List<Edge>();
    }

    // This method takes a TextAsset and returns a dictionary of nodes.
    // The dictionary's key is the node's ID, and the value is the node itself.
    public void Parse(string dataFile)
    {
        string[] lines = dataFile.Split('\n'); // Split dataset into lines

        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Trim() == "") continue; // Skip empty lines

            string[] data = lines[i].Split(','); // Split line into data
            
            //Data verification
            if (data.Length < 5)
            {
                Debug.LogWarning($"Skipping malformed line: {lines[i]}");
                continue;
            }

            Node node = new Node();
            node.Id = int.Parse(data[0].Trim());
            node.Tag = data[1].Trim();
            string type = data[2].Trim();

            //Checking node data for connections from source
            string target = data[3].Trim();

            //Debug.Log($"Target string: '{target}', Length: {target.Length}");
            if (target.Length > 0)
            {
                string[] parts = target.Split('|');
                //Debug.Log($"Split into {parts.Length} parts: {string.Join(", ", parts)}");

                try
                {
          
                    node.Connections = parts.Select(int.Parse).ToList(); 

                    for (int j = 0; j < node.Connections.Count; j++)
                    {
                        Edge edge = new Edge();
                        edge.source = node.Id;
                        edge.target = node.Connections[j];
                        edges.Add(edge);
                        //Debug.Log($"{edge.source} {edge.target}");
                    }

                    //Debug.Log($"Parsed into {connections.Length} integers: {string.Join(", ", connections)}");
                }
                catch (FormatException ex)
                {
                    Debug.LogError($"Failed to parse string to int: {ex.Message}");
                }
            }
            else
            {
                Debug.Log("Target string is empty...skipping");
            }

            //int[] connections = data[3].Trim().Length > 0 ? data[3].Trim().Split('|').Select(int.Parse).ToArray() : Array.Empty<int>();
            int tmpConnectionCount;
            int? connectionCount = int.TryParse(data[4].Trim(), out tmpConnectionCount) ? tmpConnectionCount : (int?)null;

            nodes[node.Id] = node;
        }
    }
}
