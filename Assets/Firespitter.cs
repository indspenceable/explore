using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firespitter : MonoBehaviour, IActivatableObject {
	public float dt = 1f;
	public float speed = 10f;
	public float timeAlive = 1f;
	public float startingDelay = 0f;
	public GameObject FireballPrefab;

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

	public void ShootFireball() {
		GameObject go = Instantiate(FireballPrefab, transform.position, Quaternion.identity, transform) as GameObject;
		Fireball f = go.GetComponent<Fireball>();
		f.mv = Vector3.right * speed;
		f.StartCoroutine(f.DestroyIn(timeAlive));
	}
}
