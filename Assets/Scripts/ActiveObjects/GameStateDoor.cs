using UnityEngine;
using System.Collections;

public class GameStateDoor : MonoBehaviour, IActivatableObject {
	public string flagName;

	public void Activate () {
		if (GameManager.instance.player.currentGameState.flags.IndexOf(flagName) != -1) {
			Destroy(gameObject);
		}
	}
}
