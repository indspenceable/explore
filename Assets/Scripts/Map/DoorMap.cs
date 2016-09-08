using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DoorMap {
	public enum Direction {
		RIGHT,
		LEFT,
	}
	public bool DoorAt(int x, int y) {
		return true;
	}
}
