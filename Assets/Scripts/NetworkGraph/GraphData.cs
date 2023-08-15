using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphData : IParsable
{
    public Dictionary<int, Node> nodes;
    public List<Edge> edges;
    Dictionary<int, Node> IParsable.nodes => nodes;
    List<Edge> IParsable.edges => edges;

    public GraphData()
    {
        nodes = new Dictionary<int, Node>();
        edges = new List<Edge>();
    }

    public void Parse(string data)
    {
        string m_comment;
        int m_directed;
        int m_id;
        string m_label = "";

        using (StringReader reader = new StringReader(data))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.StartsWith("comment")) m_comment = line.Split('"')[1];
                else if (line.StartsWith("directed")) m_directed = int.Parse(line.Split(' ')[1]);
                else if (line.StartsWith("id")) m_id = int.Parse(line.Split(' ')[1]);
                else if (line.StartsWith("label")) m_label = line.Split('"')[1];
                else if (line.StartsWith("node"))
                {
                    Node node = new Node();
                    while (!(line = reader.ReadLine().Trim()).StartsWith("]"))
                    {
                        if (line.StartsWith("id")) node.Id = int.Parse(line.Split(' ')[1]);
                        else if (line.StartsWith("label")) node.Name = line.Split('"')[1];
                        else if (line.StartsWith("tag")) node.Tag = line.Split('"')[1];
                    }
                    nodes[node.Id] = node;
                }
                else if (line.StartsWith("edge"))
                {
                    //Debug.Log("Edge parsing not yet implemented");
                    Edge edge = new Edge();
                    while (!(line = reader.ReadLine().Trim()).StartsWith("]"))
                    {
                        if (line.StartsWith("source")) edge.source = int.Parse(line.Split(' ')[1]);
                        else if (line.StartsWith("target")) edge.target = int.Parse(line.Split(' ')[1]);
                        else if (line.StartsWith("label")) edge.label = line.Split('"')[1];
                    }
                    //Debug.Log($"source: {edge.source} target:{edge.target} label: {m_label}");

                    // Now, update the connections in the source node
                    if (nodes.ContainsKey(edge.source))
                    {
                        Node sourceNode = nodes[edge.source];
                        if (sourceNode != null)
                        {
                            // Assuming connections is a List<int>. If it's an array, you might need to convert it to a List first.
                            sourceNode.Connections.Add(edge.target);
                        }
                    }
                 
                    edges.Add(edge);
                }
            }
        }
    }
}
