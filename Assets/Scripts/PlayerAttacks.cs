using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerAttacks : GameplayPausable {
	Animator animator;
	bool canShootMissile = true;
	public float bulletVelocity = 5f;
	public GameObject bulletPrefab;

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Ranged")) {
			animator.SetBool("casting", canShootMissile);
		}
	}
	// Triggered from the animator, shoots a fireball.
	public void ShootMissile() {
		canShootMissile = false;
		float dx = GetComponent<SpriteRenderer>().flipX ? bulletVelocity : -bulletVelocity;
		Bullet b = (GameObject.Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Bullet>();
		b.direction = new Vector3(dx, 0f);
	}

	// Triggered from the animator, tells us we're done shooting a fireball.
	public void NotShooting() {
		canShootMissile = true;
	}
}
