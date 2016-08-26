using UnityEngine;
using System.Collections;

public class SetFlagVoiceZone : MonoBehaviour, IVoiceReciever {
	/* this is great for debugging, but not really ideal for the final game.
	 * remove this at some point? */
	public void RecieveString(string s) {
		GameManager.instance.player.currentGameState.ToggleFlag(s);
	}
}
