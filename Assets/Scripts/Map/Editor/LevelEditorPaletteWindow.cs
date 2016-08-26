using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LevelEditorPaletteWindow : EditorWindow {
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


	[MenuItem ("Window/Level Editor Palette")]
	public static void  ShowWindow () {
		LevelEditorPaletteWindow window = (LevelEditorPaletteWindow)EditorWindow.GetWindow(typeof(LevelEditorPaletteWindow));
		window.titleContent = new GUIContent("Palette");
		window.Show();
	}

	public void OnGUI() {
		int oldLayer = util.currentLayer;
		util.currentLayer = GUILayout.Toolbar(util.currentLayer, Level.LAYER_OPTIONS);

		SelectTileSheetDropdown();
		RenderAllTileButtons();
		DisplayCurrentTileSpriteLarge();


		if (util.currentLayer != oldLayer) {
			ReRender();
		}
	}

	void SelectTileSheetDropdown()
	{
		var tileSheetOptions = util.knownTileSheets;
		if (tileSheetOptions == null) {
			tileSheetOptions = new List<string> ();
		}
		int currentIndex = tileSheetOptions.IndexOf (util.currentlySelectedTileSheetAssetLocation);
		int selectedIndex = EditorGUILayout.Popup (currentIndex, tileSheetOptions.ToArray ());
		if (currentIndex != selectedIndex) {
			util.SetCurrentTileSheet (util.knownTileSheets [selectedIndex]);
		}
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
		numberOfTilesPerRow = 8;
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

	void DisplayCurrentTileSpriteLarge()
	{
		Rect rect = EditorGUILayout.GetControlRect (GUILayout.Width (128), GUILayout.Height (128));
		if (util.currentlySelectedSprite) {
			if ( util.currentlySelectedSprite != null) {
				EditorUtil.DrawTextureGUI (rect, util.currentlySelectedSprite, rect.size);
			}
		}
	}

	public void ReRender() {
		LevelEditorWindow[] windows = Resources.FindObjectsOfTypeAll<LevelEditorWindow>();
		if(windows != null && windows.Length > 0)
		{
			windows[0].Repaint();
		}
		Repaint();
	}
}

