using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBoss : MonoBehaviour {
	public AudioClip bossMusic;
	public abstract void StartUp();
	public abstract void GetHit();
	public IEnumerator FadeIn(float time) {
		if (GetComponent<SpriteRenderer>() == null) {
			yield break;
		}
		float dt = 0f;
		Color start = new Color(1f, 1f, 1f, 0f);
		Color end = new Color(1f, 1f, 1f, 1f);
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		while (dt < time) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
			sr.color = Color.Lerp(start, end, dt/time);
		}
		sr.color = end;
	}
}

public class DoomEye : AbstractBoss {
	public Transform rightSensor;
	public Transform leftSensor;
	public GameObject VulnerableTarget;
	public Transform targetPositionTarget;

	public LayerMask horizWallLayerMask;
	public float startingYPosition;
	public float sinAmplication = 5f;
	public float theta = 0f;
	public float thetaSpeed = 1f;
	public int movementDirection = 1;
	public float xVel = 0.1f;
	public float raiseLowerTime = 1f;

	public float movePeriod;
	public float vulnerabilityPeriod;

	int hits = 0;

	public Shooter shooter;
	public float shotPeriod = 3f;

	public StartBoss starterEnder;

	public override void StartUp() {
		shooter = GetComponent<Shooter>();
		startingYPosition = transform.position.y;
		Debug.Log(startingYPosition);
		StartCoroutine(MoveInSinWave());
	}

	public void SetGameObjectActive() {
		gameObject.SetActive(true);
	}

	public IEnumerator MoveInSinWave() {
		float shotDt = 0f;
		float dt = 0;
		while (dt < movePeriod || Mathf.Abs(transform.position.y - startingYPosition) > 0.1f) {
			theta += thetaSpeed * GameManager.instance.ActiveGameDeltaTime;
			transform.position = new Vector3(transform.position.x + movementDirection*xVel*GameManager.instance.ActiveGameDeltaTime, startingYPosition + Mathf.Sin(theta) * sinAmplication);
			if (Physics2D.OverlapPoint(rightSensor.position, horizWallLayerMask)) {
				movementDirection = -1;
			}
			if (Physics2D.OverlapPoint(leftSensor.position, horizWallLayerMask)) {
				movementDirection = 1;
			}
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
			shotDt += GameManager.instance.ActiveGameDeltaTime;
			if (shotDt > shotPeriod) {
				shotDt = 0f;
				float dx = GameManager.instance.player.transform.position.x - transform.position.x;
				if ((dx > 0) != (shooter.speed >0)) {
					shooter.speed *= -1;
				}
				shooter.ShootFireball();
			}
		}
		yield return LowerTarget();
	}

	public IEnumerator LowerTarget() {
		// TIME TO GET VULNERABLE!;
		(VulnerableTarget.GetComponent<IPlayerHittable>() as MonoBehaviour).enabled = true;
		transform.position = new Vector3(transform.position.x, startingYPosition);
		float dt = 0f;
		while (dt < raiseLowerTime) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
			VulnerableTarget.transform.position = Vector3.Lerp(transform.position, targetPositionTarget.position, dt/raiseLowerTime);
		}
		dt = 0f;
		while (dt < vulnerabilityPeriod) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		yield return RetractTarget();
	}

	public IEnumerator RetractTarget() {
		(VulnerableTarget.GetComponent<IPlayerHittable>() as MonoBehaviour).enabled = false;

		float dt = 0f;
		while (dt < raiseLowerTime) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
			VulnerableTarget.transform.position = Vector3.Lerp(targetPositionTarget.position, transform.position, dt/raiseLowerTime);
		}
		StartCoroutine(MoveInSinWave());
	}

	public override void GetHit(){
		Debug.Log("GoT HIT!");
		sinAmplication += 0.5f;
		xVel += 0.5f;
		thetaSpeed += 1f;
		shotPeriod *= 3f/4f;
		StopAllCoroutines();
		hits += 1;
		if (hits < 1) {
			StartCoroutine(RetractTarget());
		} else {
			StartCoroutine(Explode());
		}
	}
	public IEnumerator Explode() {
		yield return GameManager.instance.FadeOutMusic();
		yield return null;
		starterEnder.StartCoroutine(starterEnder.DisableBossNStuff());
		GameManager.instance.player.currentGameState.enable(GameStateFlag.BOSS_ONE_FINISHED);
		Destroy(gameObject);
	}
}
