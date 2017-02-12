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
	public float mapX;
	public float mapY;
	public Vector3 pos {
		get {
			return new Vector3(mapX, mapY);
		}
		set {
			this.mapX = value.x;
			this.mapY = value.y;
		}
	}

	[EnumFlagsAttribute]
	public GameStateFlag upgrades;

	public bool enabled(GameStateFlag flag) {
		return (flag & upgrades) != 0;
	}
	public void enable(GameStateFlag flag) {
		upgrades = (flag | upgrades);
	}

//	public HashSet<KeyValuePair<int,int>> visitedLocations = new HashSet<KeyValuePair<int, int>>();
//	public bool hasVisited(int x, int y) {
//		return visitedLocations.Contains (new KeyValuePair<int, int> (x, y));
//	}
}

// For the editor.

public class EnumFlagsAttribute : PropertyAttribute
{
	public EnumFlagsAttribute() { }
}