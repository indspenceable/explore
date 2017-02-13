using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
	public float speed = 10f;
	public float timeAlive = 1f;
	public GameObject FireballPrefab;

	public void ShootFireball() {
		GameObject go = Instantiate(FireballPrefab, transform.position, Quaternion.identity, transform) as GameObject;
		Fireball f = go.GetComponent<Fireball>();
		f.mv = Vector3.right * speed;
		f.StartCoroutine(f.DestroyIn(timeAlive));
	}
}
