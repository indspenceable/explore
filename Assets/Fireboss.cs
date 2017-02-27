using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// It ended up as a giant plant instead of fire thing, but oh well.
public class Fireboss : AbstractBoss, IActivatableObject {
	public Shooter[] proxyShooters;
	public BossTarget[] targets;

	IEnumerator RotateCoroutine;
	int previousTarget = 0;

	public int hp = 5;
	public StartBoss startNStop;

	public float StartupCooldownForShots = 3f;
	public float timeBetweenTargetSwaps = 6f;

	public override void StartUp() {
		StartCoroutine(ShootNSwap());
		RotateTarget();
	}

	public Sprite ActiveShooter;
	public Sprite InactiveShooter;

	public Sprite TargetActive;
	public Sprite TargetInactive;

	List<IEnumerator> coroutinesToStart;
	public void Activate(Level l) {
		Debug.Log("Activating...");
		coroutinesToStart = new List<IEnumerator>();
		float time = 1f;

		foreach( Shooter s in proxyShooters) {
			Vector3 dest = s.transform.position;
			if (s.GetComponent<SpriteRenderer>().flipX) {
				s.transform.position = s.transform.position + Vector3.right;
			} else {
				s.transform.position = s.transform.position - Vector3.right;
			}
			coroutinesToStart.Add(MoveThing(s.gameObject, s.transform.position, dest, time));
		}
		foreach (BossTarget t in targets) {
			Vector3 dest = t.transform.position;
			t.transform.position += Vector3.down * 24;
			coroutinesToStart.Add(MoveThing(t.gameObject, t.transform.position, dest, time));
		}
	}

	public override IEnumerator Arrive(float time) {
		Debug.Log("Arriving...");
		foreach (IEnumerator coroutine in coroutinesToStart) {
			StartCoroutine(coroutine);
		}
		float dt = 0f;
		while (dt < time) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
	}

	IEnumerator MoveThing(GameObject thing, Vector3 sPos, Vector3 ePos, float time) {
		float dt = 0f;
		while (dt < time) {
			thing.transform.position = Vector3.Lerp(sPos, ePos, dt/time);
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		thing.transform.position = ePos;
	}

	public override void GetHit() {
		hp -= 1;

		if (hp == 0) {
			GameManager.instance.player.currentGameState.enable(GameStateFlag.BOSS_TWO_FINISHED);
			startNStop.StartCoroutine(startNStop.DisableBossNStuff());
			gameObject.SetActive(false);
		} else {
			StartupCooldownForShots -= 0.25f;
			timeBetweenTargetSwaps -= 1f;
			RotateTarget();
		}
	}

	private IEnumerator ShootNSwap() {
		while (true) {
			float dt = 0f;
			int currentShooter = Random.Range(0, proxyShooters.Length);
			foreach(Shooter shooter in proxyShooters) {
//				shooter.gameObject.SetActive(false);
				shooter.GetComponent<SpriteRenderer>().sprite = InactiveShooter;
			}
//			proxyShooters[currentShooter].gameObject.SetActive(true);
			proxyShooters[currentShooter].GetComponent<SpriteRenderer>().sprite = ActiveShooter;

			while (dt < StartupCooldownForShots) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			proxyShooters[currentShooter].ShootFireball();
			dt = 0f;
			while (dt < StartupCooldownForShots) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
		}
	}
	private void RotateTarget() {
		if (RotateCoroutine != null) {
			StopCoroutine(RotateCoroutine);
		}
		RotateCoroutine = RotateTargets();
		StartCoroutine(RotateCoroutine);

	}
	private IEnumerator RotateTargets() {
		float dt = 0;
		foreach(BossTarget target in targets) {
//			target.gameObject.SetActive(false);
			target.enabled = false;
			target.GetComponent<SpriteRenderer>().sprite = TargetInactive;
		}
		int newTarget = previousTarget;
		while (newTarget == previousTarget) {
			newTarget = Random.Range(0, targets.Length);
		}
		previousTarget = newTarget;
		targets[newTarget].enabled=true;
		targets[newTarget].GetComponent<SpriteRenderer>().sprite = TargetActive;
		while (dt < timeBetweenTargetSwaps) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		RotateTarget();
	}
}
