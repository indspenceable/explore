
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class LevelEditorWindow : EditorWindow {
	private Vector2 mapScrollPosition;
	private int currentLayer = 0;

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

		currentLayer = GUILayout.Toolbar(currentLayer, Level.LAYER_OPTIONS);

		RenderAllTileButtons();
		EditorGUILayout.Separator();
		DrawCurrentMapWithSprites();
	}

	void RenderTileButton(int i)
	{
		// Get the current sprite we're rendering
		Sprite s = util.sprites[i];
		// Get the position it's button should be at, using autolayout
		Rect re = EditorGUILayout.GetControlRect (GUILayout.Width (32), GUILayout.Height (32));
		// Draw a button, then draw the sprite on top of it.
		if (GUI.Button (re, "")) {
			util.currentlySelectedSprite = s;
		}
		EditorUtil.DrawTextureGUI (re, s, re.size);
	}

	void RenderAllTileButtons()
	{
		int i = 0;
		int numberOfTilesPerRow = Screen.width / 38;
		//		numberOfTilesPerRow = 3;
		int numberOfRows = (util.sprites.Length + numberOfTilesPerRow - 1) / numberOfTilesPerRow;
		for (int y = 0; y < numberOfRows; y += 1) {
			EditorGUILayout.BeginHorizontal ();
			for (int x = 0; x < numberOfTilesPerRow; x += 1) {
				if (i >= util.sprites.Length) {
					continue;
				}
				RenderTileButton (i);
				i += 1;
			}
			EditorGUILayout.EndHorizontal ();
		}
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
				// The tile we wish to render
				GameObject currentTile = currentLevel.FindTileAt(x, y, currentLayer);
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
}