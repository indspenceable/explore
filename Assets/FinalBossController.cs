using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossController : AbstractBoss {
	public Transform rightGround;
	public Transform rightWall;
	public Transform leftGround;
	public Transform leftWall;
	public LayerMask walls;

	public float crawlPeriod = 1f;
	public float crawlVelocityScale = 1f;
	public AnimationCurve crawlVelocity;
	public bool facingRight = true;

	public float gravity = 30f;
	public float SHOTSPEED = 10f;
	private VerticalMovement vert;
	[SerializeField]
	private Shooter[] shooters;
	private Shooter shooter {
		get {
			return shooters[Random.Range(0, shooters.Length)];
		}
	}

	private Spike spike;
	public BossTarget target;

	Coroutine CURRENT_AI_STUFF;

	public float LENGTH_OF_TIME_VULNERABLE = 5f;
	public float WAIT_AFTER_FIRING = 3f;
	public int hp = 7;

	public AudioClip GetHitAudio;
	public AudioClip Laugh;

	public StartBoss startStop;

	private void ReduceValues() {
		LENGTH_OF_TIME_VULNERABLE *= 0.9f;
		WAIT_AFTER_FIRING *= 0.8f;
		SHOTSPEED *= 1.1f;
		hp -= 1;
	}

	// Use this for initialization
	public override void StartUp () {
		GameManager.instance.PlaySound(Laugh);
		vert = GetComponent<VerticalMovement>();
		GetComponent<SpriteRenderer>().flipX = facingRight;
//		this.shooter = GetComponent<Shooter>();
		this.spike = GetComponent<Spike>();
		CURRENT_AI_STUFF = StartCoroutine(ChooseMove());
	}

	private bool checkCurrentDirection() {
		if (facingRight) {
			return Physics2D.OverlapPoint(rightGround.position, walls) && !Physics2D.OverlapPoint(rightWall.position, walls);
		} else {
			return Physics2D.OverlapPoint(leftGround.position, walls) && !Physics2D.OverlapPoint(leftWall.position, walls);
		}
	}

	public int scaleByDirection() {
		return facingRight ? 1 : -1;
	}

	private IEnumerator Fall() {
		while (!vert.CheckGrounded()) {
			vert.vy -= gravity * GameManager.instance.ActiveGameDeltaTime;
			vert.Fall(GameManager.instance.ActiveGameDeltaTime * vert.vy);
			yield return null;
		}
	}

	private IEnumerator Pace() {
		float dt = 0f;
		while (dt < crawlPeriod) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
			float pct = dt/crawlPeriod;
			transform.Translate(Vector3.right * crawlVelocity.Evaluate(pct) * GameManager.instance.ActiveGameDeltaTime * crawlVelocityScale * scaleByDirection(), Space.World);
			if (!vert.CheckGrounded()) {
				// Start another coroutine
				yield return StartCoroutine(Fall());
				break;
			}

			if (!checkCurrentDirection()) {
				facingRight = !facingRight;
				GetComponent<SpriteRenderer>().flipX = facingRight;
				break;
			}
		}
	}

	public IEnumerator ChooseMove() {
		while (true) {
			int i = Random.Range(0, 5);
			if (!vert.CheckGrounded()) {
				yield return Fall();
				continue;
			}
			if (i < 2) {
				Debug.Log("PACING.");
				yield return Pace();
			} else if(i < 4) {
				Debug.Log("SHOOTING");
				yield return ShootFireball();
			} else {
				Debug.Log("HITTABLE.");
				yield return BecomeVulnerable();
			}
		}
	}

	public override void GetHit() {
		StopAllCoroutines();
		target.enabled = false;
		GameManager.instance.PlaySound(GetHitAudio);
		ReduceValues();
		if (hp > 0) {
			CURRENT_AI_STUFF = StartCoroutine(WAIT_THEN_START());
		} else {
			gameObject.SetActive(false);
			startStop.StartCoroutine(startStop.DisableBossNStuff());
		}
	}

	public IEnumerator WAIT_THEN_START() {
		GetComponent<SpriteRenderer>().color = Color.white;
		float dt = 0f;
		float time = 3f;
		while (dt < time) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		spike.enabled = true;
		target.enabled = false;
		GetComponent<SpriteRenderer>().color = Color.white;
		yield return ChooseMove();
	}

	public IEnumerator ShootFireball() {
		Shooter cShoot = shooter;
		cShoot.speed = SHOTSPEED * scaleByDirection();
		cShoot.ShootFireball();
		float dt = 0f;
		float time = WAIT_AFTER_FIRING;
		while (dt < time) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
//		Debug.Log('we did it"
	}
	public IEnumerator BecomeVulnerable() {
		spike.enabled = false;
		target.enabled = true;
		GetComponent<SpriteRenderer>().color = Color.blue;
		float dt = 0f;
		float time = LENGTH_OF_TIME_VULNERABLE;
		while (dt < time) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		spike.enabled = true;
		target.enabled = false;
		GetComponent<SpriteRenderer>().color = Color.white;
	}
}
