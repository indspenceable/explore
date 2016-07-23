using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerAttacks : GameplayPausable {
	Animator animator;
	bool canShootMissile = true;

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire2")) {
			Debug.Log(canShootMissile);
			animator.SetBool("casting", Input.GetButtonDown("Fire2") && canShootMissile);
		}
	}
	// Triggered from the animator, shoots a fireball.
	public void ShootMissile() {
		canShootMissile = false;
	}

	// Triggered from the animator, tells us we're done shooting a fireball.
	public void NotShooting() {
		canShootMissile = true;
	}
}
