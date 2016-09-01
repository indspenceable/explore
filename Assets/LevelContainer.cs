using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelContainer : MonoBehaviour {
	Level[] _levels = null;
	public Level[] levels {
		get {
			if (_levels != null) {
				return _levels;
			}
			return GetComponentsInChildren<Level>();
		}
	}

	public void BuildCache() {
		_levels = levels;
	}
	public Level FindLevelWithCoord(int x, int y) {
		foreach (Level l in levels) {
			if ((l.mapPosition.x <= x && (l.mapPosition + l.mapSize).x > x) &&
				(l.mapPosition.y <= y && (l.mapPosition + l.mapSize).y > y)) {
				return l;
			}
		}
		return null;
	}
}
