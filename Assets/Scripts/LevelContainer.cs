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
	// Only should be used in the editor...
	#if UNITY_EDITOR
	public void DestroyCache() {
		_levels = null;
	}
	#endif
	public Level FindLevelByMapCoords(int x, int y) {
		foreach (Level l in levels) {
			if ((l.mapPosition.x <= x && (l.mapPosition + l.mapSize).x > x) &&
				(l.mapPosition.y <= y && (l.mapPosition + l.mapSize).y > y)) {
				return l;
			}
		}
		return null;
	}
	public Level FindLevelByWorldCoords(float x, float y) {
		return FindLevelByMapCoords(Mathf.FloorToInt(x/GameManager.SCREEN_SIZE.x), Mathf.FloorToInt(y/GameManager.SCREEN_SIZE.y));
	}
	public Level FindLevelByWorldCoords(Vector3 pos) {
		return FindLevelByWorldCoords(pos.x, pos.y);
	}
}
