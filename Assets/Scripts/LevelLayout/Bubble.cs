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
	public GameObject popParticles;
	public AudioClip respawnClip;
	public GameObject respawnParticles;

	public void Start() {
		sr = GetComponent<SpriteRenderer>();
	}

	public void OnTriggerStay2D(Collider2D other) {
		// other should be player
		PlayerMovement player = other.GetComponent<PlayerMovement>();
		if (player != null && sr.enabled && !player.currentlyPerformingAirDodge) {
			player.transform.position = transform.position;
			if (changeYVelocity) {
				player.vert.vy = yVelocityFactor;
				player.initiatedJump = false;
			}
			if (changeXVelocity) {
				player.horiz.vx = xVelocityFactor;
			}

			StartCoroutine(DeactivateUntilRespawn(timeToRespawn));
		}
	}

	public IEnumerator DeactivateUntilRespawn(float time) {
		sr.enabled = false;
		if (popClip != null) {
			AudioSource.PlayClipAtPoint(popClip, Vector3.zero);
		}
		if (popParticles != null) {
			Instantiate(popParticles, transform.position, Quaternion.identity);
		}
		yield return new WaitForSeconds(time);
		if (respawnClip != null) {
			AudioSource.PlayClipAtPoint(respawnClip, Vector3.zero);
		}
		if (respawnParticles != null) {
			Instantiate(respawnParticles, transform.position, Quaternion.identity);
		}
		sr.enabled = true;
	}
}
