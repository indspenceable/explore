using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birdboss : AbstractBoss {
	public AnimationCurve flightPatternY;
	public Transform leftBorder;
	public Transform rightBorder;
	public StartBoss startStop;
	public AudioClip cry;
	public AudioClip Hurt;

	private int direction = 1;
	private float velocity = 1f;
	private float baselineY;

	private float swoopTime = 4f;
	private float delayTime = 1f;
	int hits = 5;

	public GameObject fireballPrefab;
	private float LazerTime = 12f;
	private float lazerSpeed = 3f;
	public Coroutine LAZERS;

	// Use this for initialization
	public override void StartUp () {
		baselineY = transform.position.y;
		Debug.Log("Baseline Y is: " + baselineY);
		StartCoroutine(Fly());
		LAZERS = StartCoroutine(FireLazers());
	}
	
	// Update is called once per frame
	public override void GetHit () {
		hits -= 1;
		swoopTime *= 3f/4f;
		delayTime *= 5f/6f;
		LazerTime *= 7f/8f;
		lazerSpeed += 0.25f;
		velocity *= 1.2f;
		StopCoroutine(LAZERS);
		LAZERS = StartCoroutine(FireLazers());

		GameManager.instance.PlaySound(Hurt);

		if (hits <= 0) {
			GameManager.instance.player.currentGameState.enable(GameStateFlag.BOSS_THREE_FINISHED);
			gameObject.SetActive(false);
			startStop.StartCoroutine(startStop.DisableBossNStuff());
		}
	}

	private IEnumerator Fly() {
		while (true) {
			int i = Random.Range(3,4);
			while (i > 0) {
				yield return Swoop(3);
				i -= 1;
			}
			GameManager.instance.PlaySound(cry);
			yield return DODELAY();
			yield return Swoop(7);
		}
	}

	private IEnumerator Swoop(int height) {
		float dt = 0;
		float time = 1f;
		while (dt < time) {
			yield return null;
			transform.position = new Vector3(transform.position.x + direction * GameManager.instance.ActiveGameDeltaTime * velocity, 
				baselineY - flightPatternY.Evaluate(dt/time) * height);
			dt += GameManager.instance.ActiveGameDeltaTime;
			if (transform.position.x > rightBorder.position.x) {
				direction = -1;
			}
			if (transform.position.x < leftBorder.position.x) {
				direction = 1;
			}
		}
		yield return DODELAY();
	}

	private IEnumerator DODELAY() {
		float delay = delayTime;
		float dt = 0f;
		while (dt < delay) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
	}

	private IEnumerator FireLazers() {
		while (true) {
			float delay = delayTime;
			float dt = 0f;
			while (dt < delay) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			GameObject fball = Instantiate(fireballPrefab) as GameObject;
			fball.transform.position = transform.position;
			fball.transform.parent = GameManager.instance.currentActiveObjects.transform;
			fball.GetComponent<Fireball>().mv = (GameManager.instance.player.transform.position - transform.position).normalized * lazerSpeed;
		}
	}
}
