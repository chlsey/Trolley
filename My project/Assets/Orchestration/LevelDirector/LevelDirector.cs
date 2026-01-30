using System;
using UnityEngine;

// Optional interface for StageSet scripts that want GameState at spawn time.
public interface IStageSetReceiver
{
    void Initialize(GameState state);
}

// Spawned by GameOrchestrator class to handle level lifecycle
// Uses a stage set prefab to load and deload level assets smoothly
// DON'T ADD THESE TO SCENES MANUALLY, SPAWNED BY GAME ORCHESTRATOR.
public class LevelDirector : MonoBehaviour
{
    public static LevelDirector Active { get; private set; }

    private GameObject _stageInstance;
    private Action<string> _onOutcome;
    private bool _hasEnded;
    private bool _hasRun;

    private void Awake()
    {
        if (Active != null && Active != this)
        {
            Debug.LogError("LevelDirector: another instance is already active.");
            Destroy(gameObject);
            return;
        }

        Active = this;
    }

    private void OnDestroy()
    {
        if (Active == this)
        {
            Active = null;
        }
    }

    public void Run(LevelNode node, GameState state, Transform stageRoot, Action<string> onOutcome)
    {
        if (_hasRun)
        {
            Debug.LogWarning("LevelDirector.Run called more than once; ignoring.");
            return;
        }

        if (Active != this)
        {
            Debug.LogWarning("LevelDirector.Run called on a non-active instance; ignoring.");
            return;
        }

        _hasRun = true;
        _onOutcome = onOutcome;

        if (node == null)
        {
            Debug.LogError("LevelDirector.Run called with a null LevelNode.");
            EndLevel("error_missing_node");
            return;
        }

        if (stageRoot == null)
        {
            Debug.LogError("LevelDirector.Run missing StageRoot.");
            EndLevel("error_missing_stage_root");
            return;
        }

        if (node.stageSetPrefab == null)
        {
            Debug.LogError($"LevelDirector.Run missing StageSet prefab on node '{node.id}'.");
            EndLevel("error_missing_stage_prefab");
            return;
        }

        _stageInstance = Instantiate(node.stageSetPrefab, stageRoot, false);
        _stageInstance.name = node.stageSetPrefab.name;

        // Initialize any components that opt-in to GameState.
        var behaviours = _stageInstance.GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            var receiver = behaviours[i] as IStageSetReceiver;
            if (receiver != null)
            {
                receiver.Initialize(state);
            }
        }
    }

    public void EndLevel(string outcomeId)
    {
        if (_hasEnded)
        {
            return;
        }

        _hasEnded = true;

        if (_stageInstance != null)
        {
            Destroy(_stageInstance);
            _stageInstance = null;
        }

        if (_onOutcome != null)
        {
            _onOutcome(outcomeId);
        }
        else
        {
            Debug.LogWarning("LevelDirector ended without an outcome callback.");
        }

        Destroy(gameObject);
    }
}
