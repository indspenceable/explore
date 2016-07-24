using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerAttacks : GameplayPausable {
	Animator animator;
	bool mayInitiateAttack = true;
	public float bulletVelocity = 5f;
	public float meleeOffset = 1f;

	public GameObject bulletPrefab;
	public GameObject meleeHitPrefab;

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Ranged") && mayInitiateAttack) {
			ShootMissile();
		} else if (Input.GetButtonDown("Melee") && mayInitiateAttack) {
			Melee();
		}
	}

	public void ShootMissile() {
		mayInitiateAttack = false;
		animator.SetTrigger("shoot");
		float dx = GetComponent<SpriteRenderer>().flipX ? bulletVelocity : -bulletVelocity;
		Bullet b = (GameObject.Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Bullet>();
		b.direction = new Vector3(dx, 0f);
	}

	public void Melee() {
		animator.SetTrigger("melee");
		mayInitiateAttack = false;
		Vector3 dx = new Vector3(GetComponent<SpriteRenderer>().flipX ? meleeOffset : -meleeOffset, 0f);
		(GameObject.Instantiate(meleeHitPrefab, transform.position + dx, Quaternion.identity) as GameObject).GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
	}

	// Triggered from the animator, tells us we're done shooting a fireball.
	public void AbleToAttack() {
		mayInitiateAttack = true;
	}
}
