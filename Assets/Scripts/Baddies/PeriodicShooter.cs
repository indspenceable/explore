using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicShooter : Shooter, IActivatableObject {
	public float startingDelay = 0f;
	public float dt = 1f;
	public bool DO_SPRITE_STUFF;

	public void Activate(Level l) {
		StartCoroutine(ShootFireballs());
	}
	private SpriteRenderer sr;
	public void Start() {
		this.sr = GetComponent<SpriteRenderer>();
	}
	public void Update() {
		if (DO_SPRITE_STUFF) {
			this.sr.flipX = speed < 0;
		}
	}

	public IEnumerator ShootFireballs() {
		float t = dt - startingDelay;
		while (true) {
			t += Time.deltaTime;
			yield return null;
			if (t > dt) {
				ShootFireball();
				t -= dt;
			}
		}
	}

	public void OnDrawGizmos() {
//		Gizmos.DrawWireSphere(transform.position, speed*timeAlive);
	}
}
