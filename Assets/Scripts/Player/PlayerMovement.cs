using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerTakeDamage))]
public class PlayerMovement : PlatformPhysicsBase {
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

	public bool disabled {get; private set;}
	private PlayerTakeDamage health;

	// Use this for initialization
	void Start () {
		disabled = false;
		animator = GetComponent<Animator>();
		health = GetComponent<PlayerTakeDamage>();
	}

	IEnumerator AirDodge() {
		float dt = 0f;
		disabled = true;
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

			RiseOrFall(dy);
			MoveLeftOrRight(dx);
			vx = dx;
			vy = dy;

		}
		health.currentlyInIframes = false;
		disabled = false;
	}

	// Woo not fixedupdate
	public void Update () {
		if (disabled)
			return;
		if (Input.GetButtonDown("Airdodge")) {
			StartCoroutine(AirDodge());
			return;
		}
	
		PlayerMoveUpDown();
		RiseOrFall(vy * Time.deltaTime);
		PlayerMoveLeftRight();
		MoveLeftOrRight(vx * Time.deltaTime);
		PushOutFromWalls();
		FlipIfNeeded();
	}

	public void ApplyMagnet(Vector3 targetPosition, float weight) {
		Vector2 move = (targetPosition - transform.position).normalized;
		vx += move.x * Time.deltaTime * weight;
		vy += move.y * Time.deltaTime * weight;
	}

	private void FlipIfNeeded() {
		if (Mathf.Abs(vx) > 0 && controlsAreEnabled) {
			facingLeft = (vx < 0);
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
		vx = (facingLeft ? knockbackXVel : -knockbackXVel);
		vy = knockbackYVel;
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
			grounded = CheckCollisionVerticalAtDistance(-tinyMovementStep) && vy <= 0f;
			if (grounded || airControlAllowed || (jumpVx == 0f && vy < 0f && initiatedJump)) {
				targetVelocity = maxWalkSpeed * Input.GetAxis("Horizontal");
			} else {
				targetVelocity = jumpVx;
			}


			bool between = (
				// target velocity is between 0 and current velocity
				(0f <= targetVelocity && targetVelocity <= vx) ||
				(0f >= targetVelocity && targetVelocity >= vx)
			);

			if (between) {
				float frictionToUse = grounded ? friction : airFriction;
				vx = approach(vx, targetVelocity, frictionToUse*Time.deltaTime);
			} else {
				float accelerationToUse = grounded ? acc : airAcc;
				vx = approach(vx, targetVelocity, accelerationToUse*Time.deltaTime);
			}
		}

		animator.SetBool("horiz", vx!=0f);
		if (vx > 0) {
			facingLeft = false;
		} else if (vx < 0) {
			facingLeft = true;
		}
	}

	void PushOutFromWalls() {
		Vector3 step = new Vector3(tinyMovementStep, 0f);
		while (CheckCollisionHorizontalAtDistance(tinyMovementStep)) {
			transform.Translate(-step);
		} 
		while (CheckCollisionHorizontalAtDistance(-tinyMovementStep)) {
			transform.Translate(step);
		} 
	}

	void restOnGround() {
		Vector3 step = new Vector3(0f, tinyMovementStep);
		while (CheckCollisionVerticalAtDistance(-tinyMovementStep))
			transform.Translate(step);
		transform.Translate(-step);
	}

	float getJumpStrength() {
		return highJumpEnabled ? highJumpStrength :jumpStrength;
	}


	void PlayerMoveUpDown() {
		grounded = CheckCollisionVerticalAtDistance(-tinyMovementStep) && vy <= 0f;
		if (CheckCollisionVerticalAtDistance(-0.25f) && vy <= 0f) {
			Fall(-0.25f);
			grounded = true;
		}
		if (grounded && vy < 0f) {
			if (vy < -5f) {
			}
		}

		if (grounded) {
			restOnGround();
			vy = 0f;
			doubleJumpAvailable = true;
			// We can jump, here.
			if (Input.GetButtonDown("Jump") && controlsAreEnabled) {
				AudioSource.PlayClipAtPoint(jumpSoundEffect, Vector3.zero);
				vy = getJumpStrength();
				jumpVx = vx;
				initiatedJump = true;
			} else {
				jumpVx = 0f;
				initiatedJump = false;
			}
			floating = false;
		} else {
			if (floating) {
				vy = -floatSpeed * Time.deltaTime;
			} else {
				vy -= gravityStrength*Time.deltaTime;
			}
			if (vy < -maxGravity)
				vy = -maxGravity;

			if (Input.GetButtonDown("Jump") && controlsAreEnabled) {
				if (doubleJumpEnabled && doubleJumpAvailable) {
					AudioSource.PlayClipAtPoint(doubleJumpSoundEffect, Vector3.zero);
					vy = getJumpStrength()*2/3f;
					doubleJumpAvailable = false;
				}
			} else if (Input.GetButtonUp("Jump") && vy > 0 && initiatedJump) {
				vy /= 2f;
				initiatedJump = false;
			}

		}

		animator.SetBool("rising", vy > 0);
		animator.SetBool("falling", vy < 0);
	}
}
