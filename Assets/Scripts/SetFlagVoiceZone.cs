using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetFlagVoiceZone : MonoBehaviour, IVoiceReciever {
	string[] validEntries;
	/* this is great for debugging, but not really ideal for the final game.
	 * remove this at some point? */
	public void RecieveString(string s) {
//		GameManager.instance.player.currentGameState.ToggleFlag(s);
	}
}
