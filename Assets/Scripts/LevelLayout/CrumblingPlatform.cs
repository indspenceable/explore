using UnityEngine;
using System.Collections;

public class CrumblingPlatform : MonoBehaviour, IPlatform {
	public float timeToCrumble = 0.5f;
	public float timeToRespawn = 3f;
	private SpriteRenderer sr;
	Collider2D collider2D_;

	public AudioClip crumbleClip;
	public AudioClip respawnClip;

	public GameObject startCrumbleParticles;
	public GameObject crumbleParticles;

	private bool crumbling = false;

	public void Start() {
		sr = GetComponent<SpriteRenderer>();
		collider2D_ = GetComponent<BoxCollider2D>();
	}

	public void NotifyPlayerIsOnTop() {
		StartCoroutine(WaitCrumbleAndRespawn(timeToCrumble, timeToRespawn));
	}

	public IEnumerator WaitCrumbleAndRespawn(float dtCrumble, float dtRespawn) {
		if (crumbling) {
			yield break;
		}
		crumbling = true;
		if (startCrumbleParticles != null) {
			Instantiate(startCrumbleParticles, transform.position, Quaternion.identity);
		}
		yield return new WaitForSeconds(dtCrumble);
		if (crumbleClip != null) {
			GameManager.instance.PlaySound(crumbleClip);
		}
		if (crumbleParticles != null) {
			Instantiate(crumbleParticles, transform.position, Quaternion.identity);
		}
		sr.enabled = false;
		collider2D_.enabled = false;
		yield return new WaitForSeconds(dtRespawn);
		if (respawnClip != null) {
			GameManager.instance.PlaySound(respawnClip);
		}
		sr.enabled = true;
		collider2D_.enabled = true;
		crumbling = false;
	}
}
