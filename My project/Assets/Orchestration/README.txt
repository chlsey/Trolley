# Game Orchestration

Trolley's orchestration system for running the game start to finish.

## Classes

- GameOrchestrator (MonoBehaviour, persistent): Lives in the Core scene. Owns the main loop, holds the LevelGraph reference, and spawns LevelDirector per level.
- LevelDirector (MonoBehaviour, ephemeral): Runs exactly one level. Instantiates a StageSet prefab under StageRoot, waits for EndLevel to be called by the EndTrigger script, then destroys itself.
- LevelGraph (ScriptableObject): Data asset describing the directed graph (start node + edges).
- LevelNode (ScriptableObject): Datastructure that holds a singular level as a node in the graph. Uses a StageSet prefab to represent the graph. Holds outgoing edges to other nodes.
- LevelEdge (Serializable): Maps outcomeId -> next LevelNode.
- GameState: In-memory container for cross-level state. Use this to store data/gamestate that you want to persist through levels.
- EndTrigger (MonoBehaviour): Used to end a level and produce a level outcome. Example trigger placed in StageSet prefabs; calls LevelDirector.Active.EndLevel(outcomeId).

## Flow and Hierarchy

How are we changing scenes/levels during runtime and what does our game orchestration exist as in the scene?

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

**HOW TO ADD A NEW LEVEL TO THE GRAPH**

1) Create a StageSet prefab:
  A StageSet prefab is a containerized version of your level, which will load into the mainscene when a level starts.
   - In a scene, create an empty GameObject.
     - Hierarchy -> right-click -> Create Empty.
   - Parent all level-specific objects under it (tracks, splines, lever, victims, props).
     - Drag objects in Hierarchy onto the StageSet root.
   - Drag the root into the Project window to create the prefab.
     - Drag StageSet root from Hierarchy into Project window (e.g., Assets/StageSets/).
   - Delete the in-scene instance (the prefab is what gets spawned, we only use the in-scene instance to help build the prefab).
     - Select StageSet root in Hierarchy -> Delete.

2) Create a LevelNode asset:
  This is the representation of the level in the graph.
   - Project -> right-click -> Create -> Trolley -> Level Node.
   - On the LevelNode asset, assign its `Stage Set Prefab` field to the StageSet prefab you made in step 1.
     - Select the LevelNode asset -> Inspector -> Stage Set Prefab (drag your StageSet prefab from the Project window).
   - Set id (any label).

3) Wire edges:
  This is how you connect an existing level to the new LevelNode you created in steps 1 and 2.
   - When the previous level ends, something calls `EndTrigger.TriggerEnd()`, which reports an `outcomeId` string.
   - The graph looks at the LevelNode for the level that just ran, and matches that `outcomeId` against its Outgoing Edges.
   - The matching edge’s `Next Node` is what the orchestrator runs next.

  Steps to wire:
   1. Open the LevelNode asset you want to transition FROM.
   2. Decide which ending condition in that level should progress into the new level, then find the EndTrigger component and copy its `outcomeId`.
   3. Now add an Outgoing Edge to the LevelNode:
     - Outcome Id: Set its outcome id to the what you copied from the EndTrigger component. This will map that ending to this new level.
     - Next Node: Use the new LevelNode asset you created in step 2.
  
  So now, when ending conditions are met, and EndTrigger.TriggerEnd() is called 
   -> EndTrigger reports stored outcome id to Orchestrator 
   -> Orchestrator checks the current levels outgoing edges for a matching outcome id
   -> Finds an edge with matching outcome id, and loads the LevelNode asset that it contains.


**HOW DO I END A LEVEL?**
Levels end when something calls `LevelDirector.Active.EndLevel(outcomeId)`.

The common pattern is to use an EndTrigger component:
- Add EndTrigger to a GameObject in your StageSet prefab and set its `outcomeId` in the Inspector.
- When your gameplay/VO/animation logic decides the level is over, call `EndTrigger.TriggerEnd()`.
- In your scripts, keep a reference to the EndTrigger component (assign it in the Inspector) and call `endTrigger.TriggerEnd()` when you’re ready to end the level.
- Any script can call it: collision scripts, VO/sequence scripts, cinematics/timeline callbacks, UI buttons, animation events, etc.

Example script:
```csharp
using System.Collections;
using UnityEngine;

public class GenericSequence : MonoBehaviour
{
    [SerializeField] private EndTrigger endTrigger;

    private void Start()
    {
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        // all your level logic is here
        // ....
        // ....

        // all logic is done?
        endTrigger.TriggerEnd(); // ends the level + reports endTrigger.outcomeId
    }
}
```
**It is the level designer’s responsibility to make sure all level sequencing is complete before `EndTrigger.TriggerEnd()` is called.**
**You must ensure an EndTrigger component is implemneted for possible ending in a level, to ensure that the level orchestration system can progress**

So the lifecycle of a level should look like: Level starts -> level progresses -> level ending condition is met -> `EndTrigger.TriggerEnd()` is called.

Common gotchas:
- If you’re using collision-based ending and your trigger volume doesn’t fire (`OnTriggerEnter`), ensure the collider is a Trigger, and at least one of the two colliders has a Rigidbody.
- If the game can’t find a matching edge for the `outcomeId`, the orchestrator can’t progress to the next node (case-sensitive).
- Remember that prefabs are essentially containers. Objects inside the prefab should try to reference stuff inside the prefab and not be dependent on things outside the container.
