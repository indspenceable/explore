
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
					if (Event.current.button == 0 && util.currentlySelectedSprite) {
						currentLevel.FindOrCreateTileAt(x, y, util.currentLayer, util).GetComponent<SpriteRenderer>().sprite = util.currentlySelectedSprite;
						Repaint();
					} else if (Event.current.button == 1) {
						currentLevel.RemoveTileAt(x, y, util.currentLayer);
						Repaint();
					} else if (Event.current.button == 2) { // Middle mouse
						GameObject go = currentLevel.FindTileAt(x, y, util.currentLayer);
						if (go != null) {
							util.currentlySelectedSprite = go.GetComponent<SpriteRenderer>().sprite;
							ReRenderEditorPaletteWindow();
						}
					}
				}

				// The tile we wish to render
				GameObject currentTile = currentLevel.FindTileAt(x, y, util.currentLayer);
				if (currentTile != null) {
					// We got one!
					EditorUtil.DrawTextureGUI(
						r,
						currentTile.GetComponent<SpriteRenderer>().sprite,
						size
					);
				} else {
					// Don't draw anything! We've already reserved the UI space.
					 EditorGUI.DrawRect(r, Color.magenta);
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
	}
		
	public void ReRenderEditorPaletteWindow() {
		LevelEditorPaletteWindow[] windows = Resources.FindObjectsOfTypeAll<LevelEditorPaletteWindow>();
		if(windows != null && windows.Length > 0)
		{
			windows[0].Repaint();
		}
	}
}