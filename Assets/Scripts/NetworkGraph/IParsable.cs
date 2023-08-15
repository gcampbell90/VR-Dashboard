using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParsable
{
    Dictionary<int, Node> nodes { get; }
    List<Edge> edges { get; }

    void Parse(string data);
}
