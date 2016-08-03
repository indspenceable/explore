using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour {
	public bool changeYVelocity;
	public float yVelocityFactor;
	public bool changeXVelocity;
	public float xVelocityFactor;

	public float timeToRespawn = 3f;
	private SpriteRenderer sr;

	public AudioClip popClip;
	public AudioClip respawnClip;

	public void Start() {
		sr = GetComponent<SpriteRenderer>();
	}

	public void OnTriggerStay2D(Collider2D other) {
		// other should be player
		PlayerMovement player = other.GetComponent<PlayerMovement>();
		if (player != null && sr.enabled) {
			player.transform.position = transform.position;
			if (changeYVelocity) {
				player.vy = yVelocityFactor;
				player.initiatedJump = false;
			}
			if (changeXVelocity) {
				player.vx = xVelocityFactor;
			}
			AudioSource.PlayClipAtPoint(popClip, Vector3.zero);
			StartCoroutine(DeactivateUntilRespawn(timeToRespawn));
		}
	}

	public IEnumerator DeactivateUntilRespawn(float time) {
		sr.enabled = false;
		float dt = 0f;
		while (dt < time) {
			yield return null;
			dt += Time.deltaTime;
		}
//		AudioSource.PlayClipAtPoint(respawnClip, Vector3.zero);
		sr.enabled = true;
	}
}
