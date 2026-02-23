using UnityEngine;

// abstract class, implement execute with your sequencing
public abstract class LevelNodeHook : ScriptableObject
{
    public abstract void Execute(LevelNode node, GameState state);
}
