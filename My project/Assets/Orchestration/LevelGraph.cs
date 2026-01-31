using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Trolley/Level Graph")]
public class LevelGraph : ScriptableObject
{
    public LevelNode startNode;

    public LevelNode ResolveNext(LevelNode currentNode, string outcomeId, GameState state)
    {
        if (currentNode == null)
        {
            Debug.LogWarning("LevelGraph.ResolveNext called with null currentNode.");
            return null;
        }

        if (currentNode.outgoingEdges == null || currentNode.outgoingEdges.Count == 0)
        {
            Debug.LogWarning($"LevelGraph: Node '{currentNode.id}' has no outgoing edges.");
            return null;
        }

        for (int i = 0; i < currentNode.outgoingEdges.Count; i++)
        {
            var edge = currentNode.outgoingEdges[i];
            if (edge == null)
            {
                continue;
            }

            if (string.Equals(edge.outcomeId, outcomeId, StringComparison.Ordinal))
            {
                return edge.nextNode;
            }
        }

        Debug.LogWarning($"LevelGraph: No edge from node '{currentNode.id}' for outcome '{outcomeId}'.");
        return null;
    }
}
