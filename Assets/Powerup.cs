using UnityEngine;
using System.Collections;

public class Powerup : DisableIfFlagSet {
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			GameManager.instance.player.currentGameState.flags.Add(flag);
			Destroy(gameObject);
		}
	}
}
