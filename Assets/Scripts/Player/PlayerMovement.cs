using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerTakeDamage))]
[RequireComponent(typeof(VerticalMovement))]
[RequireComponent(typeof(HorizontalMovement))]
public class PlayerMovement : MonoBehaviour {
	Animator animator;

	public bool facingLeft = true;
	public float maxWalkSpeed = 5f;
	public float acc = 30f;
	public float airAcc = 15f;
	public float friction = 5f;
	public float airFriction = 1f;

	public float jumpStrength = 10f;
	public float highJumpStrength = 20f;
	public bool highJumpEnabled = false;
	public float gravityStrength = 30f;
	public AudioClip jumpSoundEffect;

	//TODO move this into a "move vertically" script
	private bool grounded;
	public float maxGravity = 100f;

	// Jump related stuff
	bool floating;
	public float floatSpeed = 1f;
	float jumpVx;
	public bool initiatedJump;
	public bool doubleJumpEnabled = true;
	bool doubleJumpAvailable = false;
	public AudioClip doubleJumpSoundEffect;

	public AnimationCurve airDodgeMovementCurve;
	public float airDodgeMovementDistance = 2f;
	public float airDodgeDuration = 0.25f;
	public float airDodgeInitialDelay = 0.25f;

	public bool currentlyPerformingAirDodge {get; private set;}
	private PlayerTakeDamage health;

	public VerticalMovement vert {get; private set;}
	public HorizontalMovement horiz {get; private set;}

	public LayerMask interactableMask;

	// Use this for initialization
	void Start () {
		currentlyPerformingAirDodge = false;
		animator = GetComponent<Animator>();
		health = GetComponent<PlayerTakeDamage>();
		vert = GetComponent<VerticalMovement>();
		horiz = GetComponent<HorizontalMovement>();
	}

	IEnumerator AirDodge() {
		float dt = 0f;
		currentlyPerformingAirDodge = true;
		health.currentlyInIframes = true;
		yield return new WaitForSeconds(airDodgeInitialDelay);

		Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * airDodgeMovementDistance;
		Vector3 startPosition = transform.position;
		Vector3 endPosition = transform.position + direction;

		while (dt <= airDodgeDuration){
			Vector3 dtStart = Vector3.Lerp(startPosition, endPosition, airDodgeMovementCurve.Evaluate(dt/airDodgeDuration));
			yield return null;
			dt += Time.deltaTime;
			Vector3 dtEnd = Vector3.Lerp(startPosition, endPosition, airDodgeMovementCurve.Evaluate(dt/airDodgeDuration));

			float dx = (dtEnd - dtStart).x;
			float dy = (dtEnd - dtStart).y;

			vert.RiseOrFall(dy);
			vert.vy = dy;
			horiz.MoveLeftOrRight(dx);
			horiz.vx = dx;

		}
		health.currentlyInIframes = false;
		currentlyPerformingAirDodge = false;
	}

	// Woo not fixedupdate
	public void Update () {
		if (currentlyPerformingAirDodge)
			return;
	
		if (Input.GetButtonDown("Airdodge")) {
			StartCoroutine(AirDodge());
			return;
		}

		if (Input.GetButtonDown("Interact")) {
			InteractIfAble();
		}
	
		PlayerMoveUpDown();
		vert.RiseOrFall(vert.vy * Time.deltaTime);
		PlayerMoveLeftRight();
		horiz.MoveLeftOrRight(horiz.vx * Time.deltaTime);
		PushOutFromWalls();
		FlipIfNeeded();
	}

	public void ApplyMagnet(Vector3 targetPosition, float weight) {
		Vector2 move = (targetPosition - transform.position).normalized;
		horiz.vx += move.x * Time.deltaTime * weight;
		vert.vy += move.y * Time.deltaTime * weight;
	}

	public void InteractIfAble() {
		BoxCollider2D _collider = GetComponent<BoxCollider2D>();
		RaycastHit2D interactable = Physics2D.BoxCast((Vector2)transform.position + _collider.offset, Vector3.Scale(transform.lossyScale, _collider.size), 0, Vector2.right, 0, interactableMask);
		if (interactable) {
			interactable.collider.gameObject.SendMessage("Interact");
		}
	}

	private void FlipIfNeeded() {
		if (Mathf.Abs(horiz.vx) > 0 && controlsAreEnabled) {
			facingLeft = (horiz.vx < 0);
		}
		GetComponent<SpriteRenderer>().flipX = !facingLeft;
	}

	public bool controlsAreEnabled = true;

	public IEnumerator DisableControlsWhileInjured(float time) {
		controlsAreEnabled = false;
		animator.SetBool("injured", true);
		float dt = 0f;
		while (dt < time) {
			yield return null;
			dt += Time.deltaTime;
		}
		animator.SetBool("injured", false);
		controlsAreEnabled = true;
	}

	public float knockbackXVel = 6f;
	public float knockbackYVel = 5f;
	public float knockbackDisableDuration = 0.25f;
	public void KnockBack() {
		horiz.vx = (facingLeft ? knockbackXVel : -knockbackXVel);
		vert.vy = knockbackYVel;
		StartCoroutine(DisableControlsWhileInjured(knockbackDisableDuration));
	}

	public bool airControlAllowed = true;

	float approach(float initial, float target, float dx) {
		if (Mathf.Abs(initial - target) < dx) {
			return target;
		} else if (initial < target) {
			return initial + dx;
		} else {
			return initial - dx;
		}
	}

	void PlayerMoveLeftRight() {
		if (controlsAreEnabled) {
			float targetVelocity;
			grounded = vert.CheckCollisionVerticalAtDistance(-VerticalMovement.tinyMovementStep) && vert.vy <= 0f;
			if (grounded || airControlAllowed || (jumpVx == 0f && vert.vy < 0f && initiatedJump)) {
				targetVelocity = maxWalkSpeed * Input.GetAxis("Horizontal");
			} else {
				targetVelocity = jumpVx;
			}

			bool between = (
				// target velocity is between 0 and current velocity
				(0f <= targetVelocity && targetVelocity <= horiz.vx) ||
				(0f >= targetVelocity && targetVelocity >= horiz.vx)
			);

			if (between) {
				float frictionToUse = grounded ? friction : airFriction;
				horiz.vx = approach(horiz.vx, targetVelocity, frictionToUse*Time.deltaTime);
			} else {
				float accelerationToUse = grounded ? acc : airAcc;
				horiz.vx = approach(horiz.vx, targetVelocity, accelerationToUse*Time.deltaTime);
			}
		}

		animator.SetBool("horiz", horiz.vx!=0f);
		if (horiz.vx > 0) {
			facingLeft = false;
		} else if (horiz.vx < 0) {
			facingLeft = true;
		}
	}

	void PushOutFromWalls() {
		Vector3 step = new Vector3(HorizontalMovement.tinyMovementStep, 0f);
		while (horiz.CheckCollisionHorizontalAtDistance(HorizontalMovement.tinyMovementStep)) {
			transform.Translate(-step);
		} 
		while (horiz.CheckCollisionHorizontalAtDistance(-HorizontalMovement.tinyMovementStep)) {
			transform.Translate(step);
		} 
	}

	float getJumpStrength() {
		return highJumpEnabled ? highJumpStrength :jumpStrength;
	}


	void PlayerMoveUpDown() {
		if (vert.CheckGrounded()) {
			vert.vy = 0f;
			doubleJumpAvailable = true;
			RaycastHit2D plat = vert.CheckCollisionVerticalAtDistance(-VerticalMovement.tinyMovementStep);
			plat.collider.gameObject.SendMessage("PlayerResting", SendMessageOptions.DontRequireReceiver);

			// We can jump, here.
			if (Input.GetButtonDown("Jump") && controlsAreEnabled) {
				AudioSource.PlayClipAtPoint(jumpSoundEffect, Vector3.zero);
				vert.vy = getJumpStrength();
				jumpVx = horiz.vx;
				initiatedJump = true;
			} else {
				jumpVx = 0f;
				initiatedJump = false;
			}
			floating = false;
		} else {
			if (floating) {
				vert.vy = -floatSpeed * Time.deltaTime;
			} else {
				vert.vy -= gravityStrength*Time.deltaTime;
			}
			if (vert.vy < -maxGravity)
				vert.vy = -maxGravity;

			if (Input.GetButtonDown("Jump") && controlsAreEnabled) {
				if (doubleJumpEnabled && doubleJumpAvailable) {
					AudioSource.PlayClipAtPoint(doubleJumpSoundEffect, Vector3.zero);
					vert.vy = getJumpStrength()*2/3f;
					doubleJumpAvailable = false;
				}
			} else if (Input.GetButtonUp("Jump") && vert.vy > 0 && initiatedJump) {
				vert.vy /= 2f;
				initiatedJump = false;
			}

		}

		animator.SetBool("rising", vert.vy > 0);
		animator.SetBool("falling", vert.vy < 0);
	}
}
