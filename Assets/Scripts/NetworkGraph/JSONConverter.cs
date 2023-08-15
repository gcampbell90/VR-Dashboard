using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class JSONConverter : MonoBehaviour
{
    public TextAsset inputJSON;

    private void Start()
    {
        ConvertAndSave("Assets/Resources/testOutput.txt");
    }

    public void ConvertAndSave(string outputPath)
    {
        InputFormat inputData = JsonUtility.FromJson<InputFormat>(inputJSON.ToString());
        StringBuilder output = new StringBuilder();

        output.AppendLine("graph");
        output.AppendLine("[");

        Dictionary<string, int> idMapping = new Dictionary<string, int>();
        int nodeId = 0;

        foreach (var node in inputData.nodes)
        {
            output.AppendLine("  node");
            output.AppendLine("  [");
            output.AppendLine($"    id {nodeId}");
            output.AppendLine($"    label \"{node.name}\"");
            output.AppendLine($"    tag \"{node.tag}\"");
            output.AppendLine("  ]");

            idMapping[node.id] = nodeId;
            nodeId++;
        }

        foreach (var link in inputData.links)
        {
            output.AppendLine("  edge");
            output.AppendLine("  [");
            output.AppendLine($"    source {idMapping[link.source]}");
            output.AppendLine($"    target {idMapping[link.target]}");
            output.AppendLine("  ]");
        }

        output.AppendLine("]");

        File.WriteAllText(outputPath, output.ToString());
    }
}


[System.Serializable]
public class InputNode
{
    public string id;
    public string name;
    public string colour;
    public string size;
    public string tag;
}

[System.Serializable]
public class InputLink
{
    public string source;
    public string target;
}

[System.Serializable]
public class InputFormat
{
    public List<InputNode> nodes;
    public List<InputLink> links;
}
