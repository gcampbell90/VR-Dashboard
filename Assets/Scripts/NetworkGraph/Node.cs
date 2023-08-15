using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public List<int> Connections = new List<int>();
    public string Type { get; }
    public Vector3 Position { get; set; }
    public int? ConnectionCount => Connections?.Count;
    public GameObject nodeObject;

    public Node() { }
}
