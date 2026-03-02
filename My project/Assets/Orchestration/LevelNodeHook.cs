using System.Collections;
using UnityEngine;

// abstract class, implement execute with your sequencing
// IMPLEMENT THIS AS A COROTUINE
public abstract class LevelNodeHook : ScriptableObject
{
    public abstract IEnumerator Execute(LevelNode node, GameState state);
}
