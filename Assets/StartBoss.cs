using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBoss : MonoBehaviour {
	public AbstractBoss boss;
	public bool started = false;
	public BoxCollider2D box;
	public LayerMask playerLayer;

	public List<GameObject> walls;
	public float wallEnableDelay = 0.25f;
	public GameStateFlag bossFlag;

	public void Update() {
		if (!GameManager.instance.player.currentGameState.enabled(bossFlag) && !started &&
			box.IsTouchingLayers(playerLayer)) {
			started = true;
			StartCoroutine(EnableBossNStuff());
		}
	}

	public IEnumerator EnableBossNStuff() {
		GameManager.instance.player.controlsAreEnabled = false;
		GameManager.instance.player.horiz.vx = 0f;

		yield return GameManager.instance.FadeOutMusic();

		foreach(GameObject wall in walls) {
			float dt = 0f;
			while (dt < wallEnableDelay) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			wall.SetActive(true);
		}
		boss.gameObject.SetActive(true);
		yield return boss.FadeIn(3f);
		boss.StartUp();
		StartCoroutine(GameManager.instance.FadeInMusic(boss.bossMusic, null, true));
		GameManager.instance.player.controlsAreEnabled = true;
	}

	public IEnumerator DisableBossNStuff() {
		GameManager.instance.player.controlsAreEnabled = false;
		yield return GameManager.instance.FadeOutMusic();
		yield return null;
		GameManager.instance.player.horiz.vx = 0f;
		foreach(GameObject wall in walls) {
			float dt = 0f;
			while (dt < wallEnableDelay) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			wall.SetActive(false);
		}

		yield return GameManager.instance.FadeInMusic(GameManager.instance.currentLevel.currentMusic);
		GameManager.instance.player.controlsAreEnabled = true;
	}
}
