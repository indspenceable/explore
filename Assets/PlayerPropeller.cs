using UnityEngine;
using System.Collections;

public class PlayerPropeller : MonoBehaviour {
	public bool changeYVelocity;
	public float yVelocityFactor;
	public bool changeXVelocity;
	public float xVelocityFactor;

	public float timeToRespawn = 3f;
	private SpriteRenderer sr;

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
		Debug.Log("Should be re-enableing.");
		sr.enabled = true;
	}
}
