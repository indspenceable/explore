using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerTakeDamage))]
[RequireComponent(typeof(VerticalMovement))]
[RequireComponent(typeof(HorizontalMovement))]
[RequireComponent(typeof(PlayerInputManager))]
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
	bool doubleJumpAvailable = false;
	public AudioClip doubleJumpSoundEffect;

	public AnimationCurve airDodgeMovementCurve;
	public float airDodgeMovementDistance = 2f;
	public float airDodgeDuration = 0.25f;
	public float airDodgeInitialDelay = 0.25f;

	public bool currentlyPerformingAirDodge { 
		get { 
			return airDodgeCoroutine != null;
		}
	}
	public IEnumerator airDodgeCoroutine;
	private PlayerTakeDamage health;

	public VerticalMovement vert {get; private set;}
	public HorizontalMovement horiz {get; private set;}

	public LayerMask interactableMask;

	private PlayerInputManager inputManager;
	public bool controlsAreEnabled = true;

	public SerailizableGameState currentGameState {
		get {
			SerailizableGameStateComponent gsc = GetComponent<SerailizableGameStateComponent>();
			gsc.state.pos = transform.position;
			return gsc.state;
		}
	}

	// Use this for initialization
	void Start () {
		airDodgeCoroutine = null;
		animator = GetComponent<Animator>();
		health = GetComponent<PlayerTakeDamage>();
		vert = GetComponent<VerticalMovement>();
		horiz = GetComponent<HorizontalMovement>();
		inputManager = GetComponent<PlayerInputManager>();
	}

	IEnumerator AirDodge() {
		health.currentlyInIframes = true;
		yield return new WaitForSeconds(airDodgeInitialDelay);

//		Vector3 direction = new Vector3(inputManager.GetAxis("Horizontal", GameMode.MOVEMENT), inputManager.GetAxis("Vertical", GameMode.MOVEMENT)).normalized * airDodgeMovementDistance;
		Vector3 direction;
		if (vert.CheckGrounded()) {
			direction = Vector2.down;
		} else {
			direction = Vector2.zero;
		}
		yield return MoveAlongVectorByCurve(direction, airDodgeDuration, airDodgeMovementCurve, 0f);
	}

	public IEnumerator MoveAlongVectorByCurve(Vector3 direction, float time, AnimationCurve curve, float cooldown) {
		float dt = 0f;
		Vector3 startPosition = transform.position;
		Vector3 endPosition = transform.position + direction;

		while (dt <= time){
//			Debug.Log("Did a thing." + dt + ", " + time);
			Vector3 dtStart = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(dt/time));
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
			Vector3 dtEnd = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(dt/time));

			float dx = (dtEnd - dtStart).x;
			float dy = (dtEnd - dtStart).y;

			vert.RiseOrFall(dy, false);
			vert.vy = dy;
			horiz.MoveLeftOrRight(dx);
			horiz.vx = dx;
		}
		controlsAreEnabled = false;
		horiz.vx = 0f;
		FinishAirDodge();
		dt = 0f;
		while (dt <= cooldown) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		controlsAreEnabled = true;
	}

	public void FinishAirDodge()
	{
		health.currentlyInIframes = false;
		airDodgeCoroutine = null;
	}

	public void VisitCurrentLocation ()
	{
//		currentGameState.visitedLocations.Add (GameManager.instance.levels.MapCoordsTolevelCoords (transform.position.x, transform.position.y));
	}

	// Woo not fixedupdate
	public void Update () {
		VisitCurrentLocation ();

		if (currentlyPerformingAirDodge)
			return;
	
//		if (inputManager.GetButtonDown("Airdodge", GameMode.MOVEMENT) && currentGameState.enabled(GameStateFlag.SHADOW_STEP)) {
//			airDodgeCoroutine = AirDodge ();
//			StartCoroutine(airDodgeCoroutine);
//			return;
//		}

		if (inputManager.GetButtonDown("Interact", GameMode.MOVEMENT)) {
			InteractIfAble();
		}
	
		PlayerMoveUpDown();
		vert.RiseOrFall(vert.vy * GameManager.instance.ActiveGameDeltaTime);
		PlayerMoveLeftRight();
		horiz.MoveLeftOrRight(horiz.vx * GameManager.instance.ActiveGameDeltaTime);
		PushOutFromWalls();
		FlipIfNeeded();
	}

	public void ApplyMagnet(Vector3 targetPosition, float weight) {
		Vector2 move = (targetPosition - transform.position).normalized;
		horiz.vx += move.x * GameManager.instance.ActiveGameDeltaTime * weight;
		vert.vy += move.y * GameManager.instance.ActiveGameDeltaTime * weight;
	}

	public void InteractIfAble() {
		BoxCollider2D _collider = GetComponent<BoxCollider2D>();
		RaycastHit2D interactable = Physics2D.BoxCast((Vector2)transform.position + _collider.offset, Vector3.Scale(transform.lossyScale, _collider.size), 0, Vector2.right, 0, interactableMask);
		if (interactable) {
			IInteractable[] l = interactable.collider.gameObject.GetComponents<IInteractable>();
			if (l.Length > 0) {
				l[0].Interact();
			}
		}
	}

	private void FlipIfNeeded() {
		if (horiz.vx < 0 && (inputManager.GetAxis("Horizontal", GameMode.MOVEMENT) < 0)) {
			facingLeft = true;
		} else if (horiz.vx > 0 && (inputManager.GetAxis("Horizontal", GameMode.MOVEMENT) > 0)) {
			facingLeft = false;
		}
		GetComponent<SpriteRenderer>().flipX = !facingLeft;
	}

	public IEnumerator DisableControlsWhileInjured(float time) {
		controlsAreEnabled = false;
		animator.SetBool("injured", true);
		float dt = 0f;
		while (dt < time) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		animator.SetBool("injured", false);
		controlsAreEnabled = true;
	}

	public float knockbackXVel = 6f;
	public float knockbackYVel = 5f;
	public float knockbackDisableDuration = 0.25f;
	public AudioClip knockbackSound;
	public void KnockBack() {
		horiz.vx = (facingLeft ? knockbackXVel : -knockbackXVel);
		vert.vy = knockbackYVel;
		StartCoroutine(DisableControlsWhileInjured(knockbackDisableDuration));
		GameManager.instance.PlaySound(knockbackSound);
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
				targetVelocity = maxWalkSpeed * inputManager.GetAxis("Horizontal", GameMode.MOVEMENT);
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
				horiz.vx = approach(horiz.vx, targetVelocity, frictionToUse*GameManager.instance.ActiveGameDeltaTime);
			} else {
				float accelerationToUse = grounded ? acc : airAcc;
				horiz.vx = approach(horiz.vx, targetVelocity, accelerationToUse*GameManager.instance.ActiveGameDeltaTime);
			}
		}

		animator.SetBool("horiz", horiz.vx!=0f);
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
//		return currentGameState.enabled(GameStateFlag.HIGH_JUMP) ? highJumpStrength :jumpStrength;
		return jumpStrength;
	}


	void PlayerMoveUpDown() {
		if (vert.CheckGrounded()) {
			vert.vy = 0f;
			doubleJumpAvailable = true;
			RaycastHit2D plat = vert.CheckCollisionVerticalAtDistance(-VerticalMovement.tinyMovementStep);
			IPlatform platform = plat.collider.gameObject.GetComponent<IPlatform>();
			if (platform != null) {
				platform.NotifyPlayerIsOnTop();
			}

			// We can jump, here.
			if (inputManager.GetButtonDown("Jump", GameMode.MOVEMENT) && controlsAreEnabled) {
				GameManager.instance.PlaySound(jumpSoundEffect);
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
				vert.vy = -floatSpeed * GameManager.instance.ActiveGameDeltaTime;
			} else {
				vert.vy -= gravityStrength*GameManager.instance.ActiveGameDeltaTime;
			}
			if (vert.vy < -maxGravity)
				vert.vy = -maxGravity;

			if (inputManager.GetButtonDown("Jump", GameMode.MOVEMENT) && controlsAreEnabled) {
//				if (currentGameState.enabled(GameStateFlag.DOUBLE_JUMP) && doubleJumpAvailable) {
//					GameManager.instance.PlaySound(doubleJumpSoundEffect);
//					vert.vy = getJumpStrength()*2/3f;
//					doubleJumpAvailable = false;
//				}
			} else if (inputManager.GetButtonUp("Jump", GameMode.MOVEMENT) && vert.vy > 0 && initiatedJump) {
				vert.vy /= 2f;
				initiatedJump = false;
			}

		}

		animator.SetBool("rising", vert.vy > 0);
		animator.SetBool("falling", vert.vy < 0);
	}
}
