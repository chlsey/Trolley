using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trolley/Level Node")]
public class LevelNode : ScriptableObject
{
    public string id;
    public GameObject stageSetPrefab;
    public List<LevelEdge> outgoingEdges = new List<LevelEdge>();
}

[Serializable]
public class LevelEdge
{
    public string outcomeId;
    public LevelNode nextNode;
}
