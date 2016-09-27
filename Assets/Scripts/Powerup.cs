using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour, IActivatableObject {
	public GameStateFlag upgrade;
	public void Activate() {
		if (GameManager.instance.player.currentGameState.enabled(upgrade)) {
			Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			GameManager.instance.player.currentGameState.enable(upgrade);
			GameManager.instance.StartCoroutine (GameManager.instance.Read ("Got Powerup: " + upgrade.ToString(), null));
			Destroy(gameObject);
		}
	}
}
