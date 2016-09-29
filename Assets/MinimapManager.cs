using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinimapManager : MonoBehaviour {
	private List<MinimapCell> cells;
	private GameManager gm;
	// Use this for initialization
	void Start () {
		cells = new List<MinimapCell>(GetComponentsInChildren<MinimapCell> ());
		gm = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (gm == null) {
			Start ();
		}
		KeyValuePair<int, int> pp = gm.levels.MapCoordsTolevelCoords (
			gm.player.transform.position.x, 
			gm.player.transform.position.y);
		foreach (MinimapCell cell in cells) {
			setCell (pp.Key, pp.Value, cell);
		}
	}

	private void setCell(int px, int py, MinimapCell cell) {
		// Get the coordinates for the tile that the cell should be looking at

		int lx = px + cell.x;
		int ly = py + cell.y;
		Debug.Log ("looking at " + lx + ", " + ly);
		// Set up variables for if we should enable walls on this cell
		bool left, right, up, down;
		left = right = up = down = false;
		// Find the actual Level that the cell represents
		Level cellLevel = gm.levels.FindLevelByMapCoords(lx, ly);
		if (cellLevel != null) {
			cell.gameObject.SetActive (true);
			left = gm.levels.FindLevelByMapCoords  (lx - 1, ly)     != cellLevel;
			right = gm.levels.FindLevelByMapCoords (lx + 1, ly)     != cellLevel;
			up = gm.levels.FindLevelByMapCoords    (lx,     ly + 1) != cellLevel;
			down = gm.levels.FindLevelByMapCoords  (lx,     ly - 1) != cellLevel;
		} else {
			Debug.Log ("DEACTIVATING A CELL.");
			cell.gameObject.SetActive (false);
		}
		cell.leftBorder.SetActive (left);
		cell.rightBorder.SetActive (right);
		cell.topBorder.SetActive (up);
		cell.bottomBorder.SetActive (down);
	}
}
