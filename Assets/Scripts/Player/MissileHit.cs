using UnityEngine;
using System.Collections.Generic;

public class MissileHit : MonoBehaviour {
	public float lifespan = 0.75f;
	private float dt = 0f;
	public Vector3 direction;
	public LayerMask levelGeometryLayerMask;
	private Collider2D myCollider;

	public void Start() {
		myCollider = GetComponent<Collider2D>();
	}

	public void Update () {
		transform.position += direction*Time.deltaTime;

		dt += Time.deltaTime;
		if (dt > lifespan || Physics2D.IsTouchingLayers(myCollider, levelGeometryLayerMask)) {
			Destroy(gameObject);
		}
	}

	public void OnTriggerEnter2D(Collider2D other) {
		IPlayerHittable hittable = other.gameObject.GetComponent<IPlayerHittable>();
		if (hittable != null) {
			hittable.MissileHit(1);
		}
	}
}
