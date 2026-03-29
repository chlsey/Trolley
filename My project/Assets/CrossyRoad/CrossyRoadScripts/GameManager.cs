using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour {

public Animator anim;
[Header("Game objects")]
[SerializeField] private Transform character;
[SerializeField] private Transform characterModel;
[SerializeField] private Transform terrainHolder;
[SerializeField] private TMPro.TextMeshProUGUI scoreText;
[SerializeField] private TMPro.TextMeshProUGUI deathText;

[Header("Terrain objects")]
[SerializeField] private Grass grassPrefab;
[SerializeField] private Road roadPrefab;

[Header("Game parameters")]
[SerializeField] private float moveDuration = 0.2f;
[SerializeField] private int spawnDistance = 20;
public AudioClip Jump;
public AudioSource audioSource;
float horizontalInput;
float verticalInput;

enum GameState {
    Ready,
    Moving,
    Dead
}
private GameState gameState;
private Vector2Int characterPos;
private int spawnLocation;
private List<(float terrainHeight, HashSet<int> locations, GameObject obj)> obstacles = new();  private int score = 0;

void Awake() {
    NewLevel();
  }

private void NewLevel() {
    gameState = GameState.Ready;
    characterPos = new Vector2Int(0, -1);
    character.position = new Vector3(0, 0.2f, -1);
    character.GetComponent<Character>().Reset();
    score = 0;
    scoreText.text = "0";
    deathText.enabled = false;
    obstacles.Clear();
    foreach (Transform child in terrainHolder) {
      Destroy(child.gameObject);
    }
    spawnLocation = 0;
    for (int i = 0; i < spawnDistance; i++) {
      SpawnObstacle();
    }
  }

private void SpawnObstacle() {
float roadProbability = Mathf.Lerp(0.5f, 0.9f, spawnLocation / 250f);

    if (Random.value < roadProbability) {
      Road road = Instantiate(roadPrefab, terrainHolder);
      obstacles.Add((0.1f, road.Init(spawnLocation), road.gameObject));
      road.gameObject.name = $"{spawnLocation} - Road";
    } else {
      Grass grass = Instantiate(grassPrefab, terrainHolder);
      obstacles.Add((0.2f, grass.Init(spawnLocation), grass.gameObject));
      grass.gameObject.name = $"{spawnLocation} - Grass";
    }
    spawnLocation++;
  }

private bool InStartArea(Vector2Int location) {
    if ((location.y > -5) && (location.y < 0) && (location.x > -6) && (location.x < 6)) {
      return true;
    }
    return false;
  }


void Update() {
    if (gameState == GameState.Ready) {
      Vector2Int moveDirection = Vector2Int.zero;
      horizontalInput = Input.GetAxisRaw("Horizontal");
      verticalInput = Input.GetAxisRaw("Vertical");
      
      bool sideways = Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput);

      if (Keyboard.current.upArrowKey.wasPressedThisFrame || (!sideways && verticalInput > 0)) {
        character.localRotation = Quaternion.identity;
        moveDirection.y = 1;
      } else if (Keyboard.current.downArrowKey.wasPressedThisFrame || (!sideways && verticalInput < 0) ) {
        character.localRotation = Quaternion.Euler(0, 180, 0);
        moveDirection.y = -1;
      } else if (Keyboard.current.leftArrowKey.wasPressedThisFrame || (sideways && horizontalInput < 0)) {
        character.localRotation = Quaternion.Euler(0, -90, 0);
        moveDirection.x = -1;
      } else if (Keyboard.current.rightArrowKey.wasPressedThisFrame || (sideways && horizontalInput > 0)) {
        character.localRotation = Quaternion.Euler(0, 90, 0);
        moveDirection.x = 1;
      }


      if (moveDirection != Vector2Int.zero) {
        Vector2Int destination = characterPos + moveDirection;

        if (InStartArea(destination) || ((destination.y >= 0) && !obstacles[destination.y].locations.Contains(destination.x))) {

          characterPos = destination;
          StartCoroutine(MoveCharacter());
          if ((destination.y + 1) > score) {
            score = destination.y + 1;
            scoreText.text = $"{score}";
          }
        }
        while (obstacles.Count < (characterPos.y + spawnDistance)) {
          SpawnObstacle();
          int oldIndex = characterPos.y - spawnDistance;
          if ((oldIndex >= 0) && (obstacles[oldIndex].obj != null)) {
            Destroy(obstacles[oldIndex].obj);
          }
        }
        if (characterPos.y < (score - 10)) {
          character.GetComponent<Character>().Kill(character.transform.position + new Vector3(0, 0.2f, 0.5f));
        }
      }
    }
    if (gameState == GameState.Dead && (Keyboard.current.spaceKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))) {
      NewLevel();
    }
    Vector3 cameraPosition = new(character.position.x + 2, 4, character.position.z - 3);

    cameraPosition.x = Mathf.Clamp(cameraPosition.x, -1, 5);

    Camera.main.transform.position = cameraPosition;
  }

  private IEnumerator MoveCharacter() {
    audioSource.PlayOneShot(Jump);
    anim.SetTrigger("Jump");
    gameState = GameState.Moving;
    float elapsedTime = 0f;

    float yHeight = 0.2f;
    if (characterPos.y >= 0) {
      yHeight = obstacles[characterPos.y].terrainHeight;
    }

    Vector3 startPos = character.position;
    Vector3 endPos = new(characterPos.x, yHeight, characterPos.y);

    Quaternion startRotation = characterModel.localRotation;

    while (elapsedTime < moveDuration) {

      float percent = elapsedTime / moveDuration;
      Vector3 newPos = Vector3.Lerp(startPos, endPos, percent);
      newPos.y = yHeight + (0.5f * Mathf.Sin(Mathf.PI * percent));
      character.position = newPos;
      Vector3 rotation = characterModel.localRotation.eulerAngles;
      characterModel.localRotation = Quaternion.Euler(-5f * Mathf.PI * Mathf.Cos(Mathf.PI * percent), rotation.y, rotation.z);
      elapsedTime += Time.deltaTime;

      yield return null;
    }
    character.position = endPos;
    characterModel.localRotation = startRotation;
    if (gameState == GameState.Moving) {
      gameState = GameState.Ready;
    }
  }

  public async Task PlayerCollisionAsync() {
    gameState = GameState.Dead;
    deathText.enabled = true;
    var gamepad = Gamepad.current;

    if (gamepad != null)
    {
        // Set motor speeds (0.0 to 1.0)
        // Low frequency is heavy/thumpy, High frequency is sharp/buzzy
        gamepad.SetMotorSpeeds(0.8f, 0.2f);

        await Task.Delay(200);
        Debug.Log("hit victim, rumbled");
        // Reset motors to stop the vibration
        gamepad.ResetHaptics();
    }
  }
}