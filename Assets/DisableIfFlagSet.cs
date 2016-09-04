using UnityEngine;
using System.Collections;

public class DisableIfFlagSet : MonoBehaviour, IActivatableObject {
	public string flag;
	public void Activate() {
		Debug.Log("did it?");
		if (GameManager.instance.player.currentGameState.flags.Contains(flag)) {
			Destroy(gameObject);
		}
	}
}
