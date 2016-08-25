using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerTakeDamage : MonoBehaviour {	
	public int currentHealth;
	public int maxHealth = 5;

	public float iframesDurations = 1f;
	public bool currentlyInIframes = false;

	public void Start() {
		currentHealth = maxHealth;
	}

	public void GetHit(int damage = 1) {
		if (!currentlyInIframes) {
			currentHealth -= damage;
			StartCoroutine(enableIFrames(iframesDurations));
			GetComponent<PlayerMovement>().KnockBack();
		}
	}
	public IEnumerator enableIFrames(float time) {
		currentlyInIframes = true;
		float dt = 0f;
		while (dt < time) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		currentlyInIframes = false;
	}
}
