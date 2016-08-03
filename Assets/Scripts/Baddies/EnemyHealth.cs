using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
	public int hits = 1;
	public void MeleeHit() {
		hits -= 1;
		if (hits <= 0) {
			Death();
		}
	}
	public void MissileHit() {
		MeleeHit();
	}

	public void Death() {
		Destroy(gameObject);
	}
}
