using UnityEngine;
using System.Collections;

public class GameStateGate : MonoBehaviour, IActivatableObject {
	public string flagName;

	public void Activate(Level l) {
//		if (GameManager.instance.player.currentGameState.flags.Contains(flagName)) {
//			Destroy(gameObject);
//		}
	}
}
