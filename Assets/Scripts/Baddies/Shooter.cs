using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
	public float speed = 10f;
	public float timeAlive = 1f;
	public GameObject FireballPrefab;
	public AudioClip sound;

	public void ShootFireball() {
		if (sound != null) {
			GameManager.instance.PlaySound(sound);
		}
		GameObject go = Instantiate(FireballPrefab, transform.position, Quaternion.identity, transform) as GameObject;
		Fireball f = go.GetComponent<Fireball>();
		f.mv = Vector3.right * speed;
		f.StartCoroutine(f.DestroyIn(timeAlive));
	}
}
