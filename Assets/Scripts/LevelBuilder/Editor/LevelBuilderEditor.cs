//c# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LevelBuilder))]
[CanEditMultipleObjects]
public class LookAtPointEditor : Editor 
{
//	const int numberOfTilesPerRow = 15;

	SerializedProperty gridSize;
	SerializedProperty mapSize;
	SerializedProperty importTileSheet;
	SerializedProperty sprites;
	SerializedProperty defaultTilePrefab;
	SerializedProperty currentlySelectedSprite;
	public bool editorIsEnabled = true;

	void OnEnable()
	{
		gridSize = serializedObject.FindProperty("gridSize");
		mapSize = serializedObject.FindProperty("mapSize");
		importTileSheet = serializedObject.FindProperty("importTileSheet");
		sprites = serializedObject.FindProperty("sprites");
		defaultTilePrefab = serializedObject.FindProperty("defaultTilePrefab");
		currentlySelectedSprite = serializedObject.FindProperty("currentlySelectedSprite");
	}

	static void SelectTileSheetDropdown(LevelBuilder lb)
	{
		var tileSheetOptions = lb.knownTileSheets;
		if (tileSheetOptions == null) {
			tileSheetOptions = new List<string> ();
		}
		int currentIndex = tileSheetOptions.IndexOf (lb.currentlySelectedTileSheetAssetLocation);
		int selectedIndex = EditorGUILayout.Popup (currentIndex, tileSheetOptions.ToArray ());
		if (currentIndex != selectedIndex) {
			lb.SetCurrentTileSheet (lb.knownTileSheets [selectedIndex]);
		}
	}

	static void SelectCurrentTilePrefab(LevelBuilder lb)
	{
		lb.SetCurrentPrefab (EditorGUILayout.ObjectField ("Current Tile Prefab", lb.CurrentPrefab(), typeof(GameObject), false) as GameObject);
	}

	void EnableOrDisableEditor()
	{
		editorIsEnabled = EditorGUILayout.Toggle ("Editor enabled", editorIsEnabled);
	}

	static void BulkEditButtons(LevelBuilder lb)
	{
		EditorGUILayout.BeginHorizontal ();
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
			DrawTextureGUI (rect, currentlySelectedSprite.objectReferenceValue as Sprite, rect.size);
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
		DrawTextureGUI (re, s, re.size);
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		LevelBuilder lb = (LevelBuilder)target;

		EnableOrDisableEditor ();
		EditorGUILayout.PropertyField(gridSize);
		EditorGUILayout.PropertyField(mapSize);
		EditorGUILayout.PropertyField(defaultTilePrefab);
		SelectCurrentTilePrefab (lb);
		EditorGUILayout.PropertyField(importTileSheet);
		SelectTileSheetDropdown(lb);
		BulkEditButtons (lb);

		DisplayCurrentTileSpriteLarge ();

		int i = 0;
		int numberOfTilesPerRow = Screen.width / 38;
		numberOfTilesPerRow=3;
		int numberOfRows = (sprites.arraySize+numberOfTilesPerRow-1) / numberOfTilesPerRow;
		for (int y = 0; y < numberOfRows; y+=1) {
			EditorGUILayout.BeginHorizontal();
			for (int x = 0; x < numberOfTilesPerRow; x+=1) {
				if (i >= sprites.arraySize) {
					continue;
				}
				RenderTileButton (i);
				i += 1;
			}
			EditorGUILayout.EndHorizontal();
		}


		serializedObject.ApplyModifiedProperties();
	}

	public static void DrawTexture(Rect position, Sprite sprite, Vector2 size)
	{
		Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
			sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
		Vector2 actualSize = size;

		actualSize.y *= (sprite.rect.height / sprite.rect.width);
		Graphics.DrawTexture(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect, 0, 0, 0, 0);
	}

	public static void DrawTextureGUI(Rect position, Sprite sprite, Vector2 size)
	{
		Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, 
			sprite.rect.y / sprite.texture.height,
			sprite.rect.width / sprite.texture.width, 
			sprite.rect.height / sprite.texture.height);
		Vector2 actualSize = size;

		actualSize.y *= (sprite.rect.height / sprite.rect.width);
		GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect);
	}

	Vector2? storedMousePosition = null;

	bool needRerender = false;

	static void DrawGrid(LevelBuilder lb, Vector2 _mapSize, Vector2 _gridSize)
	{
		// Draw the grid
		Handles.color = Color.green;
		for (int x = 0; x <= _mapSize.x; x += 1) {
			float xp = lb.transform.position.x + (x * _gridSize.x);
			float yp = lb.transform.position.y;
			Handles.DrawDottedLine (new Vector3 (xp, yp), new Vector3 (xp, yp + (_gridSize.y * _mapSize.y)), 4f);
		}
		for (int y = 0; y <= _mapSize.y; y += 1) {
			float xp = lb.transform.position.x;
			float yp = lb.transform.position.y + (y * _gridSize.y);
			Handles.DrawDottedLine (new Vector3 (xp + (_gridSize.x * _mapSize.x), yp), new Vector3 (xp, yp), 4f);
		}
	}

	public void OnSceneGUI() {
		if (!editorIsEnabled) {
			return;
		}

		LevelBuilder lb = target as LevelBuilder;
		Vector2 _mapSize = lb.mapSize;
		Vector2 _gridSize = lb.gridSize;

		DrawGrid (lb, _mapSize, _gridSize);

		int controlId = GUIUtility.GetControlID(FocusType.Passive);
		if (lb.currentlySelectedSprite == null) {
			lb.currentlySelectedSprite = lb.sprites[0];
			Repaint();
		}

		EventType et = Event.current.type;
		int mouseButton = Event.current.button;
		if (et == EventType.MouseDown || et == EventType.MouseDrag || et == EventType.MouseMove)
		{
			// For the current location setup up MousePosition correctly.
			Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
			Vector3 pos = worldRay.GetPoint(0) - lb.transform.position;

			int xPos = Mathf.FloorToInt(pos.x) / (int) _gridSize.x;
			int yPos = Mathf.FloorToInt(pos.y) / (int) _gridSize.y;

			if (xPos >= 0 && xPos < _mapSize.x && yPos >= 0 && yPos < _mapSize.y) {
				storedMousePosition = new Vector2(xPos*_gridSize.x, yPos*_gridSize.y);


				if (GUIUtility.hotControl == controlId || et == EventType.MouseDown) {
					// Tell the UI your event is the main one to use, it override the selection in  the scene view
					GUIUtility.hotControl = controlId;
					// Don't forget to use the event
					Event.current.Use();
				}

				if (et == EventType.MouseDown || et == EventType.MouseDrag) {
					if (mouseButton == 0) {
						lb.RemoveTileAt(xPos,yPos);
						GameObject tile = lb.FindOrCreateTileAt(xPos, yPos);
						tile.GetComponent<SpriteRenderer>().sprite = lb.currentlySelectedSprite;
						EditorUtility.SetDirty(lb);
					} else if (mouseButton == 1) {
						lb.RemoveTileAt(xPos, yPos);
					} else if (mouseButton == 2) {
						GameObject go = lb.FindTileAt(xPos, yPos);
						lb.currentlySelectedSprite = (go == null) ? lb.sprites[0] : go.GetComponent<SpriteRenderer>().sprite;
						Repaint();
					}
				} else {
					SceneView.RepaintAll();
				}

			} else {
				storedMousePosition = null;
				// Don't do the other GUI stuff so it doesn't screw up the rest of the editor.
				if ( GUIUtility.hotControl == controlId) {
					GUIUtility.hotControl = 0;
				}
			}
		}
//		Handles.DrawWireDisc(mousePosition + lb.transform.position, Vector3.forward, 0.25f);
		if (storedMousePosition.HasValue) {
			Vector2 mpv = storedMousePosition.Value;

			// Draw better outline
			Handles.color = Color.red;
			Handles.DrawDottedLine(
				new Vector3(lb.transform.position.x+mpv.x, lb.transform.position.y+mpv.y),
				new Vector3(lb.transform.position.x+mpv.x+1, lb.transform.position.y+mpv.y),
				4
			);
			Handles.DrawDottedLine(
				new Vector3(lb.transform.position.x+mpv.x, lb.transform.position.y+mpv.y),
				new Vector3(lb.transform.position.x+mpv.x, lb.transform.position.y+mpv.y+1),
				4
			);
			Handles.DrawDottedLine(
				new Vector3(lb.transform.position.x+mpv.x, lb.transform.position.y+mpv.y+1),
				new Vector3(lb.transform.position.x+mpv.x+1, lb.transform.position.y+mpv.y+1),
				4
			);
			Handles.DrawDottedLine(
				new Vector3(lb.transform.position.x+mpv.x+1, lb.transform.position.y+mpv.y),
				new Vector3(lb.transform.position.x+mpv.x+1, lb.transform.position.y+mpv.y+1),
				4
			);

			// +1, because when we flip the y (the -1 at the end) it flips over the x-axis, so offset.
//			DrawTexture(new Rect(mpv.x+0.25f, mpv.y+1.25f, 0.5f, 0.5f), currentlySelectedSprite, new Vector2(0.5f, -1));
			needRerender = true;
		} else {
			if (needRerender) {
				HandleUtility.Repaint();
				needRerender = false;
			}
		}
	}
}