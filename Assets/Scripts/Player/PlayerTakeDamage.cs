using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerTakeDamage : MonoBehaviour {	
	public int currentHealth;
	public int maxHealth = 5;

	public float iframesDurations = 1f;
	public bool currentlyInIframes = false;
	public bool ValidToGetHitNow = true;

	public healthmeter meter;
	public AudioClip hurt;

	public void Start() {
		ValidToGetHitNow = true;
		meter.gameObject.SetActive(true);
		currentHealth = maxHealth;
		meter.SetHealth(currentHealth);
	}

	public void GetHit(int damage = 1) {
		if (!ValidToGetHitNow) {
//			Debug.LogError("Got hit when we shouldn't have been vulnerable.");
			return;
		}
		if (!currentlyInIframes) {
			currentHealth -= damage;
			meter.SetHealth(currentHealth);
			if (currentHealth <= 0) {
				ValidToGetHitNow = false;
				GameManager.instance.LoadGameState(0);
				return;
			}
			StartCoroutine(enableIFrames(iframesDurations));
			GetComponent<PlayerMovement>().KnockBack();
			GameManager.instance.PlaySound(hurt);
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
