
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class LevelEditorWindow : EditorWindow {
	private Vector2 mapScrollPosition;

	Level currentLevel {
		get {
			return gm.currentLevel;
		}
	}

	GameManager _gm;
	GameManager gm {
		get {
			if (_gm == null) {
				GameObject go = GameObject.Find("GameManager");
				if (go) {
					_gm = go.GetComponent<GameManager>();
				}
				return _gm;
			} else {
				return _gm;
			}
		}
	}
		
	EditorUtil util {
		get {
			EditorUtil _util = gm.GetComponent<EditorUtil>();
			if (_util == null) {
				_util = gm.gameObject.AddComponent<EditorUtil>();
			}
			return _util;
		}
	}

	[MenuItem ("Window/LevelEditor")]
	public static void  ShowWindow () {
		LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
		window.titleContent = new GUIContent("Level Editor");
		window.Show();
	}

	void OnEnable(){
		Undo.undoRedoPerformed += HandleUndoRedoCallback;
	}

	void HandleUndoRedoCallback ()
	{
		foreach (var l in gm.levels.levels) {
			l.tiles = null;
		}
		Repaint();
	}

	void OnGUI () {
		if (gm == null) {
			GUILayout.Label("No Game Manager on the field");
			return;
		}
		if (currentLevel == null) {
			GUILayout.Label("No level currently selected.");
			return;
		}

//		util.currentLayer = GUILayout.Toolbar(util.currentLayer, Level.LAYER_OPTIONS);

//		EditorGUILayout.Separator();
		DrawCurrentMapWithSprites();
	}

	void DrawCurrentMapWithSprites(){
		Vector2 mapSize = currentLevel.mapSize;
		int width = (int)(mapSize.x * GameManager.SCREEN_SIZE.x);
		int height = (int)(mapSize.y * GameManager.SCREEN_SIZE.y);

		mapScrollPosition = EditorGUILayout.BeginScrollView(mapScrollPosition);
		// Drawing from top to bottom
		Vector2 size = new Vector2(16,16);

		for (int y = height-1; y >= 0; y -= 1) {
			EditorGUILayout.BeginHorizontal();
			for (int x = 0; x < width; x += 1) {
				// The rect we're rendering to, in the editor
				Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(16), GUILayout.Height(16));

				// If we click on this tile
				if (r.Contains(Event.current.mousePosition) && Event.current.isMouse) {
					if (Event.current.button == 0) {
						SetTile(x, y);
					} else if (Event.current.button == 1) {
						RemoveTile(x, y);
					} else if (Event.current.button == 2) { // Middle mouse
						Eyedropper(x, y);
					}
				}

				// The tile we wish to render
				GameObject currentTile = currentLevel.FindTileAt(x, y, util.currentLayer);
				if (currentTile != null) {
					if (!(util.CurrentLayerIsPrefabs() && currentTile.GetComponent<SpriteRenderer>() == null)) {
						// We got one!
						EditorUtil.DrawTextureGUI(
							r,
							currentTile.GetComponent<SpriteRenderer>().sprite,
							size
						);
					} else {
						EditorGUI.DrawRect(r, Color.blue);
					}
				} else {
					// Don't draw anything! We've already reserved the UI space.
					EditorGUI.DrawRect(r, Color.magenta);
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
	}

	void SetTile(int x, int y) {
		RemoveTile(x,y);	
		if (util.CurrentLayerIsPrefabs()) {
			if (util.currentlySelectedPrefab) {
				GameObject go = currentLevel.FindOrCreateTileAt(x, y, util.currentLayer, util, util.currentlySelectedPrefab);
				Repaint();
			}
		} else {
			if (util.currentlySelectedSprite) {
				GameObject go = currentLevel.FindOrCreateTileAt(x, y, util.currentLayer, util);
				go.GetComponent<SpriteRenderer>().sprite = util.currentlySelectedSprite;
				if (util.CurrentLayerNeedsCollider()) {
					go.AddComponent<BoxCollider2D>();
					go.layer = LayerMask.NameToLayer("Level Geometry");
				}
				Repaint();
			}
		}
	}

	void RemoveTile(int x, int y) {
		currentLevel.RemoveTileAt(x, y, util.currentLayer);
		Repaint();
	}

	void Eyedropper(int x, int y) {
		GameObject go = currentLevel.FindTileAt(x, y, util.currentLayer);
		if (go != null) {
			util.currentlySelectedSprite = go.GetComponent<SpriteRenderer>().sprite;
			EditorWindowUtil.RepaintAll();
		}
	}
}