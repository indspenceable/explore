using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAttacks : GameplayPausable {
	Animator animator;
	bool mayInitiateAttack = true;
	public float bulletVelocity = 5f;
	public float meleeOffset = 1f;

	public GameObject bulletPrefab;
	public AudioSource shootSoundEffect;
	public GameObject meleeHitPrefab;
	public AudioSource meleeSoundEffect;


	private PlayerMovement movement;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		movement = GetComponent<PlayerMovement>();
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Ranged") && MayInitiateAttack()) {
			ShootMissile();
		} else if (Input.GetButtonDown("Melee") && MayInitiateAttack()) {
			Melee();
		}
	}

	private bool MayInitiateAttack() {
		return mayInitiateAttack && movement.controlsAreEnabled;
	}

	public void ShootMissile() {
		mayInitiateAttack = false;
		animator.SetTrigger("shoot");
		shootSoundEffect.Play();
		float dx = GetComponent<SpriteRenderer>().flipX ? bulletVelocity : -bulletVelocity;
		Bullet b = (GameObject.Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Bullet>();
		b.direction = new Vector3(dx, 0f);
	}

	public void Melee() {
		animator.SetTrigger("melee");
		mayInitiateAttack = false;
		meleeSoundEffect.Play();
		Vector3 dx = new Vector3(GetComponent<SpriteRenderer>().flipX ? meleeOffset : -meleeOffset, 0f);
		(GameObject.Instantiate(meleeHitPrefab, transform.position + dx, Quaternion.identity) as GameObject).GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
	}

	// Triggered from the animator, tells us we're done shooting a fireball.
	public void AbleToAttack() {
		mayInitiateAttack = true;
	}
}
