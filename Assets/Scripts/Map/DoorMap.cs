using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DoorMap {
	// TODO move this editor crap out of here!
	[System.Serializable]
	public class Door {
		public int x;
		public int y;
	}
	public enum Direction {
		RIGHT,
		LEFT,
	}
	[SerializeField]
	public List<Door> HorizontalDoors = new List<Door>();
	private Door FindDoor(int x, int y) {
		foreach (var door in HorizontalDoors) {
			if (door.x == x && door.y == y) {
				return door;
			}
		}
		return null; 
	}
	public bool DoorAt(int x, int y) {
		return (FindDoor(x,y) != null);
	}
	public void SetDoor(int x, int y, bool shouldThereBe) {
		Door d = FindDoor(x, y);
		if (d == null && shouldThereBe) {
			d = new Door();
			d.x = x;
			d.y = y;
			HorizontalDoors.Add(d);
		} else if (d != null && !shouldThereBe) {
			HorizontalDoors.Remove(d);
		}
	}
}
