using UnityEngine;
using System.Collections.Generic;

public class MeleeHit : GameplayPausable {
	public LayerMask thignsICanHit;
	public float lifespan = 0.75f;
	private float duration = 0f;

	public override void UnpausedUpdate () {
		duration += Time.deltaTime;
		if (duration > lifespan) {
			Destroy(gameObject);
		}
	}

	public void OnTriggerEnter2D(Collider2D other) {
		other.SendMessage("MeleeHit", SendMessageOptions.DontRequireReceiver);
	}
}
