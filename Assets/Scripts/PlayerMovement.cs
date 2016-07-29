using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerTakeDamage))]
public class PlayerMovement : GameplayPausable {
	Animator animator;

	public bool facingLeft = true;

	public float maxWalkSpeed = 5f;
	public float acc = 30f;
	public float airAcc = 15f;
	public float friction = 5f;
	public float airFriction = 1f;

	public float vy = 0f;
	public float jumpStrength = 10f;
	public float highJumpStrength = 20f;
	public bool highJumpEnabled = false;
	public float gravityStrength = 30f;

	//TODO move this into a "move vertically" script
	private bool grounded;
	public LayerMask levelGeometryMask;
	public LayerMask jumpThruPlatformMask;
	public float maxGravity = 100f;

	// Jump related stuff
	bool floating;
	public float floatSpeed = 1f;
	float jumpVx;
	bool initiatedJump;
	bool hasDoubleJump = false;

	public AnimationCurve airDodgeMovementCurve;
	public float airDodgeMovementDistance = 2f;
	public float airDodgeDuration = 0.25f;
	public float airDodgeInitialDelay = 0.25f;

	private bool disabled = false;
	private PlayerTakeDamage health;
	// Use this for initialization
	void Start () {
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
		while (true){
			transform.position = Vector3.Lerp(startPosition, endPosition, airDodgeMovementCurve.Evaluate(dt/airDodgeDuration));
			yield return null;
			dt += Time.deltaTime;
			if (dt >= airDodgeDuration)
				break;
		}
		health.currentlyInIframes = false;
		disabled = false;
		vx = 0f;
		vy = 0f;
	}

	// Woo not fixedupdate
	public override void UnpausedUpdate () {
		if (disabled)
			return;
		if (Input.GetButtonDown("Airdodge")) {
			StartCoroutine(AirDodge());
			return;
		}
		moveUpDown();
		moveLeftRight();
		FlipIfNeeded();
		// checkForExits();
		// interact();
	}

	private void FlipIfNeeded() {
		if (Mathf.Abs(vx) > 0 && controlsAreEnabled) {
			facingLeft = (vx < 0);
		}
		GetComponent<SpriteRenderer>().flipX = !facingLeft;
	}

	public bool controlsAreEnabled = true;

	public IEnumerator DisableControlsForTimeframe(float time) {
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
		StartCoroutine(DisableControlsForTimeframe(knockbackDisableDuration));
	}

	public bool airControlAllowed = true;
	public float vx = 0f;

	float approach(float initial, float target, float dx) {
		if (Mathf.Abs(initial - target) < dx) {
			return target;
		} else if (initial < target) {
			return initial + dx;
		} else {
			return initial - dx;
		}
	}

	void moveLeftRight() {
		if (controlsAreEnabled) {
			float targetVelocity;
			grounded = VertCheck(-yAxisWallCollisionDistance, true) && vy <= 0f;
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
			moveRight(vx*Time.deltaTime);
		} else if (vx < 0) {
			moveLeft(vx*Time.deltaTime);
		}
	}

	void OnDrawGizmos() {
		Vector3 origin;
		origin = new Vector2(transform.position.x,transform.position.y+horizCheckOffset);
		Gizmos.DrawLine(origin, origin + Vector3.right*xAxisWallCollisionDistance);
		Gizmos.DrawLine(origin, origin - Vector3.right*xAxisWallCollisionDistance);

		origin = new Vector2(transform.position.x,transform.position.y-horizCheckOffset);
		Gizmos.DrawLine(origin, origin + Vector3.right*xAxisWallCollisionDistance);
		Gizmos.DrawLine(origin, origin - Vector3.right*xAxisWallCollisionDistance);

		origin = new Vector2(transform.position.x+(vertCheckOffset),transform.position.y);
		Gizmos.DrawLine(origin, origin + Vector3.up*yAxisWallCollisionDistance);
		Gizmos.DrawLine(origin, origin - Vector3.up*yAxisWallCollisionDistance);

		origin = new Vector2(transform.position.x-(vertCheckOffset),transform.position.y);
		Gizmos.DrawLine(origin, origin + Vector3.up*yAxisWallCollisionDistance);
		Gizmos.DrawLine(origin, origin - Vector3.up*yAxisWallCollisionDistance);
	}

	public float horizCheckOffset = 0.4f;
	bool horizCheck(float dv) {
		return (Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y-(horizCheckOffset)), Vector2.right, dv, levelGeometryMask) ||
			Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y+(horizCheckOffset)), Vector2.right, dv, levelGeometryMask));
	}

	public float vertCheckOffset = 0.1f;
	bool VertCheck(float dv, bool falling=false) {
		LayerMask mask = (falling ? jumpThruPlatformMask.value : 0) | levelGeometryMask.value;
		return (Physics2D.Raycast(new Vector2(transform.position.x-(vertCheckOffset),transform.position.y), Vector2.up, dv, mask) ||
			Physics2D.Raycast(new Vector2(transform.position.x+(vertCheckOffset),transform.position.y), Vector2.up, dv, mask));
	}


	public float xAxisWallCollisionDistance = 0.3f;
	void moveRight(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(0.001f, 0f);
		while (i < amt && !horizCheck(xAxisWallCollisionDistance)) {
			transform.Translate(step);
			i += 0.001f;
		}
		if (horizCheck(xAxisWallCollisionDistance) && vx > 0) {
			vx = 0;
		}
	}
	void moveLeft(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(-0.001f, 0f);
		while (i > amt && !horizCheck(-xAxisWallCollisionDistance)) {
			transform.Translate(step);
			i -= 0.001f;
		}
		if (horizCheck(-xAxisWallCollisionDistance) && vx < 0) {
			vx = 0;
		}
	}

	public float yAxisWallCollisionDistance = 0.5f;
	void rise(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(0f, 0.001f);
		while (i < amt && !VertCheck(yAxisWallCollisionDistance)) {
			transform.Translate(step);
			i += 0.001f;
		}
		if (VertCheck(yAxisWallCollisionDistance) && vy > 0) {
			vy = 0;
		}
	}
	void fall(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(0f, -0.001f);
		while (i > amt && !VertCheck(-yAxisWallCollisionDistance, true)) {
			transform.Translate(step);
			i -= 0.001f;
		}
	}

	void restOnGround() {
		Vector3 step = new Vector3(0f, 0.001f);
		while (VertCheck(-yAxisWallCollisionDistance, true))
			transform.Translate(step);
		transform.Translate(-step);
	}

	void moveUpDown() {
		grounded = VertCheck(-yAxisWallCollisionDistance, true) && vy <= 0f;

		if (grounded) {
			restOnGround();
			vy = 0f;
			hasDoubleJump = true;
			// We can jump, here.
			if (Input.GetButtonDown("Jump") && controlsAreEnabled) {
				vy = highJumpEnabled ? highJumpStrength :jumpStrength;
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
				if (hasDoubleJump) {
					vy = jumpStrength*2/3f;
					hasDoubleJump = false;
				}
			} else if (Input.GetButtonUp("Jump") && vy > 0) {
				vy /= 2f;
			}

		}

		animator.SetBool("rising", vy > 0);
		animator.SetBool("falling", vy < 0);
		if (vy > 0) {
			rise(vy*Time.deltaTime);
		} else if (vy < 0) {
			fall(vy*Time.deltaTime);
			//			transform.Translate(new Vector3(0, vy*Time.deltaTime));
		}
	}
}
