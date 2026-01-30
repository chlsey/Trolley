using System.Collections;
using UnityEngine;

/*
Setup notes:
- Create a Core scene that persists and add GameOrchestrator to an empty GameObject.
- Assign a LevelGraph asset and the StageRoot transform (parent for StageSet prefabs).
- Player and theatre environment live in the Core scene and never get destroyed.

StageSet notes:
- Each level is a prefab with all per-level objects under its root.
- Place EndTrigger components at track ends and set outcomeId strings that match graph edges.
*/
public class GameOrchestrator : MonoBehaviour
{
    public LevelGraph graph;
    public Transform stageRoot;

    private GameState state;
    private LevelNode currentNode;

    private void Start()
    {
        state = new GameState();

        if (graph == null)
        {
            Debug.LogError("GameOrchestrator: missing LevelGraph reference.");
            return;
        }

        if (graph.startNode == null)
        {
            Debug.LogError("GameOrchestrator: graph has no startNode.");
            return;
        }

        if (stageRoot == null)
        {
            Debug.LogError("GameOrchestrator: missing StageRoot reference.");
            return;
        }

        currentNode = graph.startNode;
        StartCoroutine(RunLoop());
    }

    private IEnumerator RunLoop()
    {
        while (currentNode != null)
        {
            string outcomeId = null;
            bool outcomeReceived = false;

            var directorObject = new GameObject("LevelDirector");
            directorObject.transform.SetParent(transform);
            var director = directorObject.AddComponent<LevelDirector>();

            director.Run(currentNode, state, stageRoot, outcome =>
            {
                outcomeId = outcome;
                outcomeReceived = true;
            });

            // Wait for the level to report an outcome via the callback.
            yield return new WaitUntil(() => outcomeReceived);

            state.LevelsCompleted++;
            currentNode = graph.ResolveNext(currentNode, outcomeId, state);
            if (currentNode == null)
            {
                Debug.Log($"GameOrchestrator: no next node for outcome '{outcomeId}'.");
            }
        }
    }
}
