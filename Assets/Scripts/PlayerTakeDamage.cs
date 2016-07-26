using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerTakeDamage : MonoBehaviour {	
	public float iframesDurations = 1f;
	public bool currentlyInIframes = false;

	public void GetHit(int damage = 1) {
		if (!currentlyInIframes) {
			StartCoroutine(enableIFrames(iframesDurations));
			GetComponent<PlayerMovement>().KnockBack();
		}
	}
	public IEnumerator enableIFrames(float time) {
		currentlyInIframes = true;
		float dt = 0f;
		while (dt < time) {
			yield return null;
			dt += Time.deltaTime;
		}
		currentlyInIframes = false;
	}
}
