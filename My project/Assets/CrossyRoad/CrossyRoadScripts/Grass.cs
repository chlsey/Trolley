using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour {
  [SerializeField] private Transform treePrefab;

  public HashSet<int> Init(float z) {
    transform.position = new Vector3(0, 0, z);
    HashSet<int> locations = new() { -6, 6 };
    int numTrees = Random.Range(1, 5);

    for (int i = 0; i < numTrees; i++) {
      Transform tree = Instantiate(treePrefab, transform);
      int xPos = Random.Range(-5, 6);
      tree.position = new Vector3(xPos, 1.2f, z);
      locations.Add(xPos);
    }

    return locations;
  }
}