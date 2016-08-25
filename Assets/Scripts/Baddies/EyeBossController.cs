using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeBossController : MonoBehaviour {
	public GameObject spikeyPrefab;
	public int numberOfSpikiesPerLasers = 15;
	public float rotationSpeed = 30f;
	public float moveSpeed = 2f;

	private List<GameObject> lasers;

	public int damageTaken = 0;

	public void Start() {
		StartFight();
	}

	public void StartFight() {
		lasers = new List<GameObject>();
		for (int i = 0; i < 4; i+= 1) {
			GameObject empty = new GameObject();
			empty.transform.SetParent(transform);
			for (int j = 0; j < numberOfSpikiesPerLasers; j+=1) {
				GameObject go = GameObject.Instantiate(spikeyPrefab, new Vector3(j+1, 0)*0.5f, Quaternion.identity) as GameObject;
				go.transform.SetParent(empty.transform);
			}
			lasers.Add(empty);
			empty.transform.localScale = Vector3.zero;
			empty.transform.RotateAround(empty.transform.position, Vector3.forward, 90*i);
		}
		StartCoroutine(DeployLasers(10f));
	}

	public IEnumerator DeployLasers(float timeframe) {
		float time = 0f;
		while (time < timeframe) {
			yield return null;
			float ttf = time/timeframe;
			foreach (GameObject g in lasers) {
				g.transform.localScale = new Vector3(ttf, ttf, ttf);
			}
			time += GameManager.instance.ActiveGameDeltaTime;
		}
		foreach (GameObject g in lasers) {
			g.transform.localScale = Vector3.one;
		}
		yield return new WaitForSeconds(0.25f);

		// Mode 1
		yield return DoRandomAttacksUntilDamageHitsThreshold(10);
		rotationSpeed += 15f;

		Coroutine randomAttacks = StartCoroutine(DoRandomAttacksUntilDamageHitsThreshold(20));
		Coroutine randomMovement = StartCoroutine(DoRandomMovesUntilDamageHisThreshold(20, transform.position, 0.25f));

		yield return randomAttacks;
		yield return randomMovement;
		rotationSpeed += 15f;
		moveSpeed += 1f;

		randomAttacks = StartCoroutine(DoRandomAttacksUntilDamageHitsThreshold(30));
		randomMovement = StartCoroutine(DoRandomMovesUntilDamageHisThreshold(30, transform.position, 0.15f));

		yield return randomAttacks;
		yield return randomMovement;


	}

	public IEnumerator SelectAndExecuteRandomLaserAttack() {
		switch(Random.Range(0,3)){
		case 0:
			yield return RotateLasers(90f, false);
			break;
		case 1:
			yield return RotateLasers(90f, true);
			break;
		case 2:
			bool direction = Random.Range(0,2) == 0;
			for (int i = 0; i < 3; i+=1) {
				yield return RotateLasers(30f, direction);
				yield return new WaitForSeconds(0.25f);
			}
			break;
		}
	}

	IEnumerator DoRandomMovesUntilDamageHisThreshold(int threshold, Vector3 startingPosition, float pauseBetweenMovement) {
		while (damageTaken < threshold) {
			yield return new WaitForSeconds(pauseBetweenMovement);
			Vector3 randomOffset = new Vector3(Random.Range(-5,5), Random.Range(-5,5));
			yield return MoveTo(startingPosition + randomOffset);
		}
		yield return MoveTo(startingPosition);
	}

	public IEnumerator MoveTo(Vector3 targetPos) {
		Vector3 startPos = transform.position;
		float totalDist = Vector3.Distance(startPos, targetPos);
		float curDist = 0f;
		while (curDist < totalDist) {
			transform.position = Vector3.Lerp(startPos, targetPos, curDist/totalDist);
			yield return null;
			curDist += GameManager.instance.ActiveGameDeltaTime * moveSpeed;
		}
		transform.position = targetPos;
	}

	public IEnumerator DoRandomAttacksUntilDamageHitsThreshold(int threshold) {
		while (damageTaken < threshold) {
			yield return SelectAndExecuteRandomLaserAttack();
		}
	}

	public IEnumerator RotateLasers(float totalRotation, bool clockwise) {
		float amountRotated = 0f;
		while (amountRotated < totalRotation) {
			yield return null;
			float amountToRotate = Mathf.Clamp(GameManager.instance.ActiveGameDeltaTime*rotationSpeed, 0f, totalRotation-amountRotated);
			if (clockwise) {
				amountToRotate *= -1;
			}
			foreach (GameObject g in lasers) {
				g.transform.RotateAround(transform.position, Vector3.forward, amountToRotate);
			}
			amountRotated += GameManager.instance.ActiveGameDeltaTime*rotationSpeed;
		}
	}
}
