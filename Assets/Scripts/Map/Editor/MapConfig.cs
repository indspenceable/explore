
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class MapConfig : EditorWindow {
	[MenuItem ("Window/Map Configuration")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(MapConfig));
	}

	[MenuItem("Edit/Reset Playerprefs")] public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }

	Dictionary<Color, GUIStyle> colors = new Dictionary<Color, GUIStyle>();

	GameManager gm;
	LevelContainer levelContainer;
	Level currentLevel {
		get {
			return gm.currentLevel;
		}
		set {
			if (!EditorApplication.isPlaying) {
				gm.currentLevel = value;
			} else {
				Debug.LogError("Trying to set the current level in play mode...");
			}
		}
	}

	bool inPlayModeLastFrame = false;
	bool change = false;

	void OnGUI () {
		change = false;
		levelContainer = GameObject.Find("Levels").GetComponent<LevelContainer>();
		if (GameObject.Find("GameManager") == null) {
			EditorGUILayout.LabelField("No Game Manager.");
			return;
		} else if (levelContainer == null) {
			Debug.Log("No Level Container. Creating one.");
			levelContainer = new GameObject("Levels").AddComponent<LevelContainer>();
		}
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();

		if (EditorApplication.isPlaying != inPlayModeLastFrame) {
			colors = new Dictionary<Color, GUIStyle>();
		}
		inPlayModeLastFrame = EditorApplication.isPlaying;

		AddColor(Color.blue);
		AddColor(Color.green);
		AddColor(Color.red);
		AddColor(Color.black);
		AddColor(Color.gray);

		levelContainer.BuildCache();
	
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add new Level")) {
			levelContainer.DestroyCache();
			GameObject levelGO = new GameObject();
			levelGO.transform.parent = levelContainer.transform;
			Level l = levelGO.AddComponent<Level>();
			l.mapPosition = currentLevel.mapPosition;
			currentLevel = l;
			Undo.RegisterCreatedObjectUndo(l, "Undo create level");
			levelContainer.BuildCache();
		}
		EditorGUILayout.EndHorizontal();

		MoveViewportOptions ();
		MoveCurrentLevelPositionOptions ();
		DisplayMap(EditorGUILayout.GetControlRect());

		levelContainer.DestroyCache();

		if (change) {
			currentLevel.MoveMeToMyPosition();
			EditorWindowUtil.RepaintAll();
		}
	}

	void AddColor(Color c) {
		if (colors.ContainsKey(c)){
			return;
		}
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0,0, c);
		texture.Apply();
		GUIStyle style = new GUIStyle();
		style.normal.background = texture;
		colors.Add(c,style);

	}

	private void MoveAll(Vector2 v) {
		foreach (Level l in gm.levels.levels) {
			Undo.RecordObject(l, "Shift Map");
			l.mapPosition += v;
			l.MoveMeToMyPosition();
		}
	}

	void MoveViewportOptions()
	{
		GUILayout.Label ("Move Viewport");
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<")) {
			MoveAll(new Vector2(-1,0));
			change = true;
		}
		if (GUILayout.Button ("v")) {
			MoveAll(new Vector2(0,-1));
			change = true;
		}
		if (GUILayout.Button ("^")) {
			MoveAll(new Vector2(0, 1));
			change = true;
		}
		if (GUILayout.Button (">")) {
			MoveAll(new Vector2(1,0));
			change = true;
		}
		EditorGUILayout.EndHorizontal ();
	}

	void MoveCurrentLevelPositionOptions()
	{
		GUILayout.Label ("Move Current Level");
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<")) {
			currentLevel.mapPosition += new Vector2(-1,0);
			change = true;
		}
		if (GUILayout.Button ("v")) {
			currentLevel.mapPosition += new Vector2(0,-1);
			change = true;
		}
		if (GUILayout.Button ("^")) {
			currentLevel.mapPosition += new Vector2(0,1);
			change = true;
		}
		if (GUILayout.Button (">")) {
			currentLevel.mapPosition += new Vector2(1,0);
			change = true;
		}
		EditorGUILayout.EndHorizontal ();
	}

	void DisplayMap(Rect r) {
		foreach(Level l in gm.levels.levels) {
			if (l == null) {
				continue;
			}
			DrawFullRoomRect(r, l, (int)l.mapPosition.x, (int)l.mapPosition.y);
			for (int dx = 0; dx < l.mapSize.x; dx+=1) {
				for (int dy = 0; dy < l.mapSize.y; dy+=1) {
					DrawSingleScreenRectButton(r, l, (int)l.mapPosition.x + dx, (int)l.mapPosition.y + dy);
				}
			}
		}
		DrawDoors(r);
	}

	void DrawDoors(Rect r) {
		int minX = 999;
		int maxX = -999;
		int minY = 999;
		int maxY = -999;

		// Calculate the outer bounds of the map
		foreach(Level l in gm.levels.levels) {
			if (l.mapPosition.x < minX) {
				minX = (int)l.mapPosition.x;
			}
			if (l.mapPosition.y < minY) {
				minY = (int)l.mapPosition.y;
			}
			if (l.mapPosition.x + l.mapSize.x > maxX) {
				maxX = (int)(l.mapPosition+l.mapSize).x;
			}
			if (l.mapPosition.y + l.mapSize.y > maxY) {
				maxY = (int)(l.mapPosition+l.mapSize).y;
			}
		}
		for (int x = minX; x < maxX; x += 1)  {
			for (int y = minY; y < maxY; y+=1) {
				Level l = gm.levels.FindLevelByMapCoords(x, y);
				Level o = gm.levels.FindLevelByMapCoords(x+1, y);
				if (l != null && o != null && l != o) {
					DrawDoorToRight(r, x, y);
				}
			}
		}
	}

	const int editorUIRoomSize = 20;
	const int editorUIRoomPadding = 6;
	const int editorUIRoomMargin = 10;

	void DrawDoorToRight(Rect r, int x, int y) {
		bool doorHere = gm.doors.DoorAt(x,y);
		GUIStyle style = doorHere ? colors[Color.red] : colors[Color.black];
		if (GUI.Button(new Rect(
			r.x + (x+1)*editorUIRoomSize - editorUIRoomMargin,
			-(r.y + y*editorUIRoomSize),
			editorUIRoomMargin,
			editorUIRoomSize - editorUIRoomMargin
		), GUIContent.none, style)) {
			change = true;
//			gm.doors.SetDoor(x, y, !doorHere);
		}
	}

	void DrawFullRoomRect(Rect r, Level l, int x, int y) {
		GUIStyle style = colors[Color.black];
		GUI.Label(new Rect(
			r.x + x*editorUIRoomSize + editorUIRoomPadding, 
			-(r.y + y*editorUIRoomSize + editorUIRoomPadding - editorUIRoomMargin),
			l.mapSize.x*editorUIRoomSize - 2*editorUIRoomPadding - editorUIRoomMargin, 
			-(l.mapSize.y*editorUIRoomSize - 2*editorUIRoomPadding - editorUIRoomMargin)
		), GUIContent.none, style);
	}

	void DrawSingleScreenRectButton(Rect r, Level l, int x, int y) {
		GUIStyle style = currentLevel == l ? colors[Color.green] : colors[Color.blue];
		if (GUI.Button(new Rect(
			r.x + x*editorUIRoomSize, 
			-(r.y + y*editorUIRoomSize),
			editorUIRoomSize - editorUIRoomMargin, 
			(editorUIRoomSize - editorUIRoomMargin)
		), GUIContent.none, style)) {
			change = true;
			gm.currentLevel = l;
			Selection.activeGameObject = l.gameObject;
			SceneView.FrameLastActiveSceneView();
		}
	}
}