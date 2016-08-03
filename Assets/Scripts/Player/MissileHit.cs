using UnityEngine;
using System.Collections.Generic;

public class MissileHit : MonoBehaviour {
	public float lifespan = 0.75f;
	private float dt = 0f;
	public Vector3 direction;

	public void Update () {
		transform.position += direction*Time.deltaTime;

		dt += Time.deltaTime;
		if (dt > lifespan) {
			Destroy(gameObject);
		}
	}

	public void OnTriggerEnter2D(Collider2D other) {
		other.SendMessage("MissileHit", SendMessageOptions.DontRequireReceiver);
	}
}
