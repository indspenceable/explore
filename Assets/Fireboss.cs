using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireboss : AbstractBoss {
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
				shooter.gameObject.SetActive(false);
			}
			proxyShooters[currentShooter].gameObject.SetActive(true);

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
			target.gameObject.SetActive(false);
		}
		int newTarget = previousTarget;
		while (newTarget == previousTarget) {
			newTarget = Random.Range(0, targets.Length);
		}
		previousTarget = newTarget;
		targets[newTarget].gameObject.SetActive(true);
		while (dt < timeBetweenTargetSwaps) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		RotateTarget();
	}
}
