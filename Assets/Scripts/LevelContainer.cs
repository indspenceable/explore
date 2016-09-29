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

	public void BuildCache(bool force=false) {
		if (force) {
			_levels = null;
			Debug.Log("FORCING A CACHE BUILD.");
		}
		_levels = levels;
		if (force) {
			Debug.Log("now we have: " + _levels.Length);
		}
	}
	// Only should be used in the editor...
	#if UNITY_EDITOR
	public void DestroyCache() {
		if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
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

	public KeyValuePair<int, int> MapCoordsTolevelCoords(float x, float y) {
		return new KeyValuePair<int, int> (Mathf.FloorToInt (x / GameManager.SCREEN_SIZE.x), Mathf.FloorToInt (y / GameManager.SCREEN_SIZE.y));
	}

	public Level FindLevelByWorldCoords(float x, float y) {
		KeyValuePair<int, int> p = MapCoordsTolevelCoords (x, y);
		return FindLevelByMapCoords(p.Key, p.Value);
	}
	public Level FindLevelByWorldCoords(Vector3 pos) {
		return FindLevelByWorldCoords(pos.x, pos.y);
	}
}
