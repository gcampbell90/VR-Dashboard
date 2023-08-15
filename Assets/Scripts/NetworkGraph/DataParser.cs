using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataParser
{
    public IParsable ParseTextAsset(TextAsset asset)
    {
        string content = asset.text;

        if (IsCSV(content))
        {
            var csvData = new CSVData();
            csvData.Parse(content);
            return csvData;
        }
        else if (IsGraphFormat(content))
        {
            var graph = new GraphData();
            graph.Parse(content);
            return graph;
        }
        return null;
    }

    public bool IsCSV(string content)
    {
        // Check for a line with at least one comma.
        var lines = content.Split('\n');
        foreach (var line in lines)
        {
            if (line.Contains(','))
            {
                return true;
            }
            else
            {
                break;
            }
        }
        return false;
    }
    public bool IsGraphFormat(string content)
    {
        var lines = content.Split('\n');
        foreach (var line in lines)
        {
            // Trim leading and trailing whitespaces.
            var trimmed = line.Trim();
            if (trimmed.StartsWith("graph") || trimmed.StartsWith("node") || trimmed.StartsWith("edge"))
            {
                return true;
            }
        }
        return false;
    }
}
