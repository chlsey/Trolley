using System.Collections;
using UnityEngine;

// Core orchestrator class
// Determines what level the player is currently on and spawns Level Directors to handle each level.
public class GameOrchestrator : MonoBehaviour
{
    public LevelGraph graph;
    public Transform stageRoot;

    private GameState state;
    private LevelNode currentNode;

    private void Start()
    {
        state = new GameState();
        state.IsCat = MenuController.IsCat;

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

            yield return StartCoroutine(director.Run(currentNode, state, stageRoot, outcome =>
            {
                outcomeId = outcome;
                outcomeReceived = true;
            }));

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
