using UnityEngine;
using System.Collections;

public class MeleeHit : GameplayPausable {
	public float lifespan = 0.75f;
	private float duration = 0f;
	// Update is called once per frame
	public override void UnpausedUpdate () {
		duration += Time.deltaTime;
		if (duration > lifespan) {
			Destroy(gameObject);
		}
	}
}
