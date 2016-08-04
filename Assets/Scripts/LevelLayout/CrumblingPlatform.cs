using UnityEngine;
using System.Collections;

public class CrumblingPlatform : MonoBehaviour {
	public float timeToCrumble = 0.5f;
	public float timeToRespawn = 3f;
	private SpriteRenderer sr;
	Collider2D collider2D_;

	public AudioClip crumbleClip;
	public AudioClip respawnClip;

	public void Start() {
		sr = GetComponent<SpriteRenderer>();
		collider2D_ = GetComponent<BoxCollider2D>();
	}

	public void PlayerResting() {
		StartCoroutine(WaitCrumbleAndRespawn(timeToCrumble, timeToRespawn));
	}

	public IEnumerator WaitCrumbleAndRespawn(float dtCrumble, float dtRespawn) {
		yield return new WaitForSeconds(dtCrumble);
		if (crumbleClip != null) {
			AudioSource.PlayClipAtPoint(crumbleClip, Vector3.zero);
		}
		sr.enabled = false;
		collider2D_.enabled = false;
		yield return new WaitForSeconds(dtRespawn);
		if (respawnClip != null) {
			AudioSource.PlayClipAtPoint(respawnClip, Vector3.zero);
		}
		sr.enabled = true;
		collider2D_.enabled = true;
	}
}
