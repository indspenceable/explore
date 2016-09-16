using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
		util.currentTool = (EditorUtil.Tool)GUILayout.Toolbar((int)util.currentTool, System.Enum.GetNames(typeof(EditorUtil.Tool)));

		if (util.CurrentLayerIsPrefabs()) {
			RenderAllSelectPrefabButtons();
			DisplayCurrentPrefabSpriteLarge();
		} else {
			SelectTileSheetDropdown();
			RenderAllTileButtons();
			DisplayCurrentTileSpriteLarge();
		}

		if (util.currentLayer != oldLayer) {
			EditorWindowUtil.RepaintAll();
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

	void RenderSpriteTileButton(int i)
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
				RenderSpriteTileButton (i);
				i += 1;
			}
			EditorGUILayout.EndHorizontal ();
		}
	}

	void RenderPrefabTileButton(string guid) {
		string assetPath = AssetDatabase.GUIDToAssetPath(guid);
		// Get the current sprite we're rendering
		GameObject[] gameObjects = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<GameObject>().ToArray();
		if (gameObjects.Length == 0) return;
		GameObject go = gameObjects[0];

		// Get the position it's button should be at, using autolayout
		Rect re = EditorGUILayout.GetControlRect (GUILayout.Width (64), GUILayout.Height (64));
		// Draw a button, then draw the sprite on top of it.
		Sprite s = null;
		if (go.GetComponent<SpriteRenderer>() != null) {
			s = go.GetComponent<SpriteRenderer>().sprite;
		}
		string label = (s == null) ? go.name : "";

		if (GUI.Button (re, label)) {
			util.currentlySelectedPrefab = go;
		}
		if (s != null) {
			EditorUtil.DrawTextureGUI (re, s, re.size);
		}
	}

	void RenderAllSelectPrefabButtons() {
		string[] objs = AssetDatabase.FindAssets("t:Object", new string[]{"Assets/Prefabs"});

		int i = 0;
		int numberOfTilesPerRow = Screen.width / 38;
		numberOfTilesPerRow = 8;
		int numberOfRows = (objs.Length + numberOfTilesPerRow - 1) / numberOfTilesPerRow;
		for (int y = 0; y < numberOfRows; y += 1) {
			EditorGUILayout.BeginHorizontal ();
			for (int x = 0; x < numberOfTilesPerRow; x += 1) {
				if (i >= objs.Length) {
					continue;
				}
				RenderPrefabTileButton (objs[i]);
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

	void DisplayCurrentPrefabSpriteLarge()
	{
		Rect rect = EditorGUILayout.GetControlRect (GUILayout.Width (128), GUILayout.Height (128));
		if ( util.currentlySelectedPrefab != null && util.currentlySelectedPrefab.GetComponent<SpriteRenderer>() != null) {
			EditorUtil.DrawTextureGUI (rect, util.currentlySelectedPrefab.GetComponent<SpriteRenderer>().sprite, rect.size);
		}
	}
}

