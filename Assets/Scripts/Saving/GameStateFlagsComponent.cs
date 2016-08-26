using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateFlagsComponent : MonoBehaviour {
	public GameStateFlags state;
}
[System.Serializable]
public class GameStateFlags {
	public int mapX;
	public int mapY;
	public bool highJumpEnabled;
	public bool doubleJumpEnabled;
	public bool magnetEnabled;
	public bool meleeAttackEnabled;
	public bool rangedAttackEnabled;

	public List<string> flags = new List<string>();
	public void ToggleFlag(string flagName) {
		if (flags.Contains(flagName)) {
			flags.Remove(flagName);
		} else {
			flags.Add(flagName);
		}
	}
}
