using UnityEngine;
using System.Collections;

public class DestructableBlock : MonoBehaviour, IPlayerHittable {
	public void MeleeHit(int _damage) {
		Destroy(gameObject);
	}
	public void MissileHit(int _damage) {
		// No op
	}
}
