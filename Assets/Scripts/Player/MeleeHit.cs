using UnityEngine;
using System.Collections.Generic;

public class MeleeHit : MonoBehaviour {
	public float lifespan = 0.75f;
	private float duration = 0f;

	public void Update () {
		duration += GameManager.instance.ActiveGameDeltaTime;
		if (duration > lifespan) {
			Destroy(gameObject);
		}
	}

	public void OnTriggerEnter2D(Collider2D other) {
		IPlayerHittable hittable = other.gameObject.GetComponent<IPlayerHittable>();
		if (hittable != null) {
			hittable.MeleeHit(1);
		}
	}
}
