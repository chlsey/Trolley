# Game Orchestration

Trolley's orchestration system for running the game start to finish.

## Classes

- GameOrchestrator (MonoBehaviour, persistent): Lives in the Core scene. Owns the main loop, holds the LevelGraph reference, and spawns LevelDirector per level.
- LevelDirector (MonoBehaviour, ephemeral): Runs exactly one level. Instantiates a StageSet prefab under StageRoot, waits for EndLevel, then destroys itself.
- LevelGraph (ScriptableObject): Data asset describing the directed graph (start node + edges).
- LevelNode (ScriptableObject): One node in the graph. Holds a StageSet prefab reference and outgoing edges.
- LevelEdge (Serializable): outcomeId -> next LevelNode.
- GameState (plain C# class): In-memory container for cross-level state.
- EndTrigger (MonoBehaviour): Example trigger placed in StageSet prefabs; calls LevelDirector.Active.EndLevel(outcomeId).



## Flow and Hierarchy

- Core scene (persistent):
  - GameOrchestrator
  - Player + Camera
  - Theatre environment
  - StageRoot (empty Transform at 0,0,0)
- Per level:
  - StageSet prefab instantiated under StageRoot
  - StageSet contains all level-specific objects (tracks, lever, victims, props, end triggers, etc.)

Flow:
1) GameOrchestrator.Start() -> sets currentNode = graph.startNode -> starts coroutine.
2) Orchestrator spawns LevelDirector and calls Run(node, state, stageRoot, onOutcome).
3) LevelDirector instantiates the StageSet prefab and waits.
4) An EndTrigger (or other script) calls LevelDirector.Active.EndLevel("outcome_id").
5) LevelDirector destroys StageSet, invokes callback, destroys itself.
6) Orchestrator resolves next node and repeats.






## How Do I Use This For Development? 

### One Time Set Up

I've already set this up for Trolley so you probably won't have to worry about it.

- Core scene:
  - Add an empty GameObject and attach GameOrchestrator.
    - Hierarchy -> right-click -> Create Empty, then Inspector -> Add Component -> GameOrchestrator.
  - Create an empty GameObject named StageRoot and keep it at (0,0,0).
    - Hierarchy -> right-click -> Create Empty, rename to StageRoot, Inspector -> Transform -> Reset.
  - Assign StageRoot to GameOrchestrator.stageRoot.
    - Select GameOrchestrator -> Inspector -> GameOrchestrator -> Stage Root (drag StageRoot from Hierarchy).
  - Create a LevelGraph asset and assign it to GameOrchestrator.graph.
    - Project -> right-click -> Create -> Trolley -> Level Graph.
    - Select GameOrchestrator -> Inspector -> GameOrchestrator -> Graph (drag LevelGraph asset).
- Do NOT place LevelDirector in any scene (it is spawned at runtime).


### Level Development Workflow 

*(READ THIS IF YOU'RE ADDING NEW LEVELS)*

1) Create a StageSet prefab (your level):
   - In a scene, create an empty root GameObject (e.g., StageSet_Level01).
     - Hierarchy -> right-click -> Create Empty.
   - Parent all level-specific objects under it (tracks, splines, lever, victims, props).
     - Drag objects in Hierarchy onto the StageSet root.
   - Add EndTrigger(s) at track ends (Collider set to Is Trigger).
     - Select trigger object -> Inspector -> Add Component -> Box Collider (check Is Trigger).
     - Inspector -> Add Component -> EndTrigger -> set outcomeId.
   - Drag the root into the Project window to create the prefab.
     - Drag StageSet root from Hierarchy into Project window (e.g., Assets/StageSets/).
   - Delete the in-scene instance (the prefab is what gets spawned).
     - Select StageSet root in Hierarchy -> Delete.

2) Create a LevelNode asset:
   - Project -> right-click -> Create -> Trolley -> Level Node.
   - Assign stageSetPrefab to your StageSet prefab.
     - Select LevelNode asset -> Inspector -> Stage Set Prefab (drag prefab from Project).
   - Set id (any label).
     - Select LevelNode asset -> Inspector -> Id.

3) Wire edges:
   - On the LevelNode, add outgoingEdges.
     - Select LevelNode asset -> Inspector -> Outgoing Edges -> Size (increase) or + button.
   - outcomeId must exactly match EndTrigger.outcomeId (case-sensitive).
     - Select LevelNode asset -> Inspector -> Outgoing Edges -> Outcome Id.
   - nextNode points to another LevelNode asset.
     - Select LevelNode asset -> Inspector -> Outgoing Edges -> Next Node (drag LevelNode asset).

4) Set graph start node (once):
   - Open the LevelGraph asset and set startNode to your first LevelNode.
     - Select LevelGraph asset -> Inspector -> Start Node (drag LevelNode asset).

Common gotchas:
- If EndTrigger doesn’t fire, ensure trigger collider is Is Trigger, and at least one of the two colliders has a Rigidbody.
- Prefab references only persist to objects inside the prefab. Scene references outside the prefab will be lost.
