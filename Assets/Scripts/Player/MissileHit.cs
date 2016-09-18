using UnityEngine;
using System.Collections.Generic;

public class MissileHit : MonoBehaviour {
	public float lifespan = 0.75f;
	private float dt = 0f;
	public Vector3 direction;
	public LayerMask levelGeometryLayerMask;
	private Collider2D myCollider;
	public GameObject spawnablePlatform;

	public void Start() {
		myCollider = GetComponent<Collider2D>();
	}

	public void Update () {
		transform.position += direction*GameManager.instance.ActiveGameDeltaTime;

		dt += GameManager.instance.ActiveGameDeltaTime;
		if (Physics2D.IsTouchingLayers(myCollider, levelGeometryLayerMask)) {
			// Step back, and then spawn a block
			transform.position -= direction*GameManager.instance.ActiveGameDeltaTime;
			Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x+0.5f)-0.5f, Mathf.RoundToInt(transform.position.y+0.5f)-0.5f);
			Instantiate(spawnablePlatform, pos, Quaternion.identity);
			Destroy(gameObject);
		} else if (dt > lifespan) {
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
