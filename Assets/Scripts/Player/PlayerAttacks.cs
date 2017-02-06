using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAttacks : MonoBehaviour {
	Animator animator;
	bool mayInitiateAttack = true;
	public float bulletVelocity = 5f;
	public float meleeOffset = 1f;

	public MissileHit missilePrefab;
	public AudioClip shootSoundEffect;
	public GameObject meleeHitPrefab;
	public AudioClip meleeSoundEffect;
	private PlayerMovement movement;
	private PlayerInputManager inputManager;

	public float meleeMoveTime;
	public AnimationCurve meleeMovementCurve;

	public SerailizableGameState currentGameState {
		get {
			return GetComponent<SerailizableGameStateComponent>().state;
		}
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		movement = GetComponent<PlayerMovement>();
		inputManager = GetComponent<PlayerInputManager>();
	}
	// Update is called once per frame
	public void Update () {
		if (inputManager.GetButtonDown("Ranged", GameMode.MOVEMENT) && MayInitiateAttack() && currentGameState.enabled(GameStateFlag.RANGED)) {
			ShootMissile();
		} else if (inputManager.GetButtonDown("Melee", GameMode.MOVEMENT) && MayInitiateAttack() && currentGameState.enabled(GameStateFlag.MELEE) && movement.vert.CheckGrounded() ) {
			Melee();
		}
	}

	private bool MayInitiateAttack() {
		// TODO movement shouldn't own controls being enabled - that should be its own script
		return mayInitiateAttack && movement.controlsAreEnabled;
	}

	public void ShootMissile() {
		mayInitiateAttack = false;
		animator.SetTrigger("shoot");
		GameManager.instance.PlaySound(shootSoundEffect);
		float dx = GetComponent<SpriteRenderer>().flipX ? bulletVelocity : -bulletVelocity;
		MissileHit b = GameObject.Instantiate(missilePrefab, transform.position, Quaternion.identity) as MissileHit;
		b.direction = new Vector3(dx, 0f);
	}

	public void Melee() {
		animator.SetTrigger("melee");
		mayInitiateAttack = false;
		GameManager.instance.PlaySound(meleeSoundEffect);
		Vector3 dx = new Vector3(GetComponent<SpriteRenderer>().flipX ? meleeOffset : -meleeOffset, 0f);
		(GameObject.Instantiate(meleeHitPrefab, transform.position + dx, Quaternion.identity) as GameObject).GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
		movement.airDodgeCoroutine = movement.MoveAlongVectorByCurve(new Vector2(GetComponent<SpriteRenderer>().flipX ? 2 : -2, 0), meleeMoveTime, meleeMovementCurve);
		movement.StartCoroutine(movement.airDodgeCoroutine);
	}

	// Triggered from the animator, tells us we're done shooting a fireball.
	public void AbleToAttack() {
		mayInitiateAttack = true;
	}
}
