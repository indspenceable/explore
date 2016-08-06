using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IPlayerHittable {
	public int hits = 1;
	public void MeleeHit(int damage) {
		hits -= damage;
		if (hits <= 0) {
			Death();
		}
	}
	public void MissileHit(int damage) {
		MeleeHit(damage);
	}

	public void Death() {
		Destroy(gameObject);
	}
}
