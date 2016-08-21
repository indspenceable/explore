//c# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//[CustomEditor(typeof(Level))]
[CanEditMultipleObjects]
public class LookAtPointEditor : Editor 
{
	SerializedProperty mapSize;
	SerializedProperty mapPosition;
	SerializedProperty importTileSheet;
	SerializedProperty sprites;
//	SerializedProperty defaultTilePrefab;
	SerializedProperty currentlySelectedSprite;
	SerializedProperty backgroundImage;

	void OnEnable()
	{
		mapSize = serializedObject.FindProperty("mapSize");
		mapPosition = serializedObject.FindProperty("mapPosition");
		importTileSheet = serializedObject.FindProperty("importTileSheet");
		sprites = serializedObject.FindProperty("sprites");
//		defaultTilePrefab = serializedObject.FindProperty("defaultTilePrefab");
		currentlySelectedSprite = serializedObject.FindProperty("currentlySelectedSprite");
		backgroundImage = serializedObject.FindProperty("backgroundImage");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		Level lb = (Level)target;

		EditorGUILayout.PropertyField(backgroundImage);
		EditorGUILayout.PropertyField(mapSize);
		EditorGUILayout.PropertyField(mapPosition);
		CurrentEditLayerDropdown(lb);
		BulkEditButtons (lb);

		if (lb.currentEditLayer == 1) {
//			EditorGUILayout.PropertyField(defaultTilePrefab);
			lb.shared.defaultTilePrefab =  (EditorGUILayout.ObjectField ("Default Tile Prefab", lb.shared.defaultTilePrefab, typeof(GameObject), false) as GameObject);

			SelectCurrentTilePrefab (lb);
			EditorGUILayout.PropertyField(importTileSheet);
		}
		SelectTileSheetDropdown(lb);

		DisplayCurrentTileSpriteLarge ();
		RenderAllTileButtons ();
		serializedObject.ApplyModifiedProperties();
	}

	public void CurrentEditLayerDropdown(Level lb) {
		lb.currentEditLayer = EditorGUILayout.Popup("Select Current Edit Layer", lb.currentEditLayer, Level.LAYER_OPTIONS); 
	}



	static void SelectTileSheetDropdown(Level lb)
	{
		var tileSheetOptions = lb.shared.knownTileSheets;
		if (tileSheetOptions == null) {
			tileSheetOptions = new List<string> ();
		}
		int currentIndex = tileSheetOptions.IndexOf (lb.currentlySelectedTileSheetAssetLocation);
		int selectedIndex = EditorGUILayout.Popup (currentIndex, tileSheetOptions.ToArray ());
		if (currentIndex != selectedIndex) {
			lb.SetCurrentTileSheet (lb.shared.knownTileSheets [selectedIndex]);
		}
	}

	static void SelectCurrentTilePrefab(Level lb)
	{
		lb.SetCurrentPrefab (EditorGUILayout.ObjectField ("Current Tile Prefab", lb.CurrentPrefab(), typeof(GameObject), false) as GameObject);
	}

	static void BulkEditButtons(Level lb)
	{
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Rebuild Tile List")) {
			lb.RebindTileList();
		}
		if (GUILayout.Button ("Remove Current Spritesheet")) {
			lb.RemoveCurrentSpritesheet ();
		}
		if (GUILayout.Button ("Re-instantiate tiles")) {
			lb.ReInstantiateTiles ();
		}
		EditorGUILayout.EndHorizontal ();
	}

	void DisplayCurrentTileSpriteLarge()
	{
		Rect rect = EditorGUILayout.GetControlRect (GUILayout.Width (128), GUILayout.Height (128));
		if (currentlySelectedSprite.objectReferenceValue) {
			if (currentlySelectedSprite.objectReferenceValue == null) {
				currentlySelectedSprite.objectReferenceValue = sprites.GetArrayElementAtIndex (0).objectReferenceValue as Sprite;
				serializedObject.ApplyModifiedProperties ();
			}
			EditorUtil.DrawTextureGUI (rect, currentlySelectedSprite.objectReferenceValue as Sprite, rect.size);
		}
	}

	void RenderTileButton(int i)
	{
		// Get the current sprite we're rendering
		Sprite s = sprites.GetArrayElementAtIndex (i).objectReferenceValue as Sprite;
		// Get the position it's button should be at, using autolayout
		Rect re = EditorGUILayout.GetControlRect (GUILayout.Width (32), GUILayout.Height (32));
		// Draw a button, then draw the sprite on top of it.
		if (GUI.Button (re, "")) {
			currentlySelectedSprite.objectReferenceValue = sprites.GetArrayElementAtIndex (i).objectReferenceValue as Sprite;
			serializedObject.ApplyModifiedProperties ();
		}
		EditorUtil.DrawTextureGUI (re, s, re.size);
	}

	void RenderAllTileButtons()
	{
		int i = 0;
		int numberOfTilesPerRow = Screen.width / 38;
//		numberOfTilesPerRow = 3;
		int numberOfRows = (sprites.arraySize + numberOfTilesPerRow - 1) / numberOfTilesPerRow;
		for (int y = 0; y < numberOfRows; y += 1) {
			EditorGUILayout.BeginHorizontal ();
			for (int x = 0; x < numberOfTilesPerRow; x += 1) {
				if (i >= sprites.arraySize) {
					continue;
				}
				RenderTileButton (i);
				i += 1;
			}
			EditorGUILayout.EndHorizontal ();
		}
	}
}