using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Powerup : MonoBehaviour, IActivatableObject {
	public GameStateFlag upgrade;
	public static Dictionary<GameStateFlag, string> upgradeMessages = new Dictionary<GameStateFlag, string>() {
//		{GameStateFlag.MELEE, "Acquired Melee Attack.\n\nHit <c> to use."},
//		{GameStateFlag.HIGH_JUMP, "Acquired High Jump."},
//		{GameStateFlag.DOUBLE_JUMP, "Acquried Double Jump.\n\nHit <z> while airborn to use."},
//		{GameStateFlag.SHADOW_STEP, "Acquried Shadowstep.\n\nHit <v>+a direction to use."},
	};



	public void Activate(Level l) {
		if (GameManager.instance.player.currentGameState.enabled(upgrade)) {
			Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			GameManager.instance.player.currentGameState.enable(upgrade);
//			GameManager.instance.StartCoroutine (GameManager.instance.Read ("Got Powerup: " + upgrade.ToString(), null));
			GameManager.instance.StartCoroutine(GameManager.instance.Read(upgradeMessages[upgrade], null));
			Destroy(gameObject);
		}
	}
}
