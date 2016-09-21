using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SerailizableGameStateComponent : MonoBehaviour {
	public SerailizableGameState state;
}
[System.Flags]
public enum GameStateFlag {
	HIGH_JUMP = 1 << 0,
	DOUBLE_JUMP = 1 << 1,
	MAGNET = 1 << 2,
	MELEE = 1 << 3,
	RANGED = 1 << 4,
	SHADOW_STEP = 1 << 5,
}

[System.Serializable]
public class SerailizableGameState {
	public int mapX;
	public int mapY;

	[EnumFlagsAttribute]
	public GameStateFlag upgrades;

	public List<string> flags = new List<string>();
	public void ToggleFlag(string flagName) {
		if (flags.Contains(flagName)) {
			flags.Remove(flagName);
		} else {
			flags.Add(flagName);
		}
	}
	public bool enabled(GameStateFlag flag) {
		return (flag & upgrades) != 0;
	}
	public void enable(GameStateFlag flag) {
		upgrades = (flag | upgrades);
	}
}

// For the editor.

public class EnumFlagsAttribute : PropertyAttribute
{
	public EnumFlagsAttribute() { }
}