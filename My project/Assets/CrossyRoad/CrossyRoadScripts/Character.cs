using UnityEngine;

public class Character : MonoBehaviour {

public AudioSource audio;
public AudioClip deathSound;
[SerializeField] private GameManager gameManager;
[SerializeField] private GameObject character;
[SerializeField] private ParticleSystem deathParticles;

private void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.CompareTag("Vehicle") && character.activeSelf) {
      Kill(collision.GetContact(0).point);
    }
}

public void Kill(Vector3 collisionPoint) {
    audio.PlayOneShot(deathSound);
    character.SetActive(false);
    deathParticles.transform.position = collisionPoint;
    deathParticles.transform.LookAt(transform.position + Vector3.up);
    deathParticles.Play();
    gameManager.PlayerCollision();
    }

public void Reset() {
    character.SetActive(true);
    deathParticles.Clear();
    }
}