using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicShooter : Shooter, IActivatableObject {
	public float startingDelay = 0f;

	public void Activate(Level l) {
		StartCoroutine(ShootFireballs());
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
