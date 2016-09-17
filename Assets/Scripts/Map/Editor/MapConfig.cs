
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
	int displayWidth = 20;
	int displayHeight = 20;

	Dictionary<int, Dictionary<int, Level>> coordsToLevel;
	void AddLevel(int x, int y, Level l) {
		if (coordsToLevel == null)
		{
			coordsToLevel = new Dictionary<int, Dictionary<int, Level>>();
		}
		if (!coordsToLevel.ContainsKey(x)) {
			coordsToLevel[x] = new Dictionary<int, Level>();
		}
		coordsToLevel[x][y] = l;
	}
	Level FindLevel(int x, int y) {
		if (coordsToLevel == null)
		{
			coordsToLevel = new Dictionary<int, Dictionary<int, Level>>();
		}
		if (coordsToLevel.ContainsKey(x)) {
			if (coordsToLevel[x].ContainsKey(y)) {
				return coordsToLevel[x][y];
			}
		}
		return null;
	}

	Vector2 viewPortPosition = Vector2.zero;
	EditorUtil util;

	void OnGUI () {
		change = false;
		if (GameObject.Find("GameManager") == null) {
			EditorGUILayout.LabelField("No Game Manager.");
			return;
		}
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		util = gm.GetComponent<EditorUtil>();

		if (EditorApplication.isPlaying != inPlayModeLastFrame) {
			colors = new Dictionary<Color, GUIStyle>();
		}
		inPlayModeLastFrame = EditorApplication.isPlaying;

		AddColor(Color.blue);
		AddColor(Color.green);
		AddColor(Color.red);
		AddColor(Color.black);
		AddColor(Color.gray);


		coordsToLevel = null;
		gm.levels.DestroyCache();
		foreach (Level l in gm.levels.levels) {
			for (int x = 0; x < (int)l.mapSize.x; x += 1) {
				for (int y = 0; y < (int)l.mapSize.y; y += 1) {
					int ox = (int)l.mapPosition.x;
					int oy = (int)l.mapPosition.y;
					AddLevel(x+ox, y+oy, l);
				}
			}
		}
	
		EditorGUILayout.BeginHorizontal();
		viewPortPosition = EditorGUILayout.Vector2Field("Viewport Position", viewPortPosition);
		viewPortPosition.x = (int)viewPortPosition.x;
		viewPortPosition.y = (int)viewPortPosition.y;
		if (GUILayout.Button("Add new Level")) {
			GameObject levelGO = new GameObject();
			levelGO.transform.parent = gm.levels.transform;
			Level l = levelGO.AddComponent<Level>();
			l.mapPosition = currentLevel.mapPosition;
			currentLevel = l;
			Undo.RegisterCreatedObjectUndo(l, "Undo create level");
		}
		if (GUILayout.Button("Align Current Level")) {
			currentLevel.AlignGameObjects();
		}
		EditorGUILayout.EndHorizontal();

		MoveViewportOptions ();
		MoveCurrentLevelPositionOptions ();
		DisplayMap();

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
			viewPortPosition += (new Vector2(-1,0));
//			change = true;
		}
		if (GUILayout.Button ("v")) {
			viewPortPosition += (new Vector2(0,-1));
//			change = true;
		}
		if (GUILayout.Button ("^")) {
			viewPortPosition += (new Vector2(0, 1));
//			change = true;
		}
		if (GUILayout.Button (">")) {
			viewPortPosition += (new Vector2(1,0));
//			change = true;
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

	void DisplayMap() {
		int ox = (int)viewPortPosition.x;
		int oy = (int)viewPortPosition.y;
		GUIStyle buttonStyle = new GUIStyle();
//		buttonStyle.

		for (int y = displayHeight-1; y >= 0; y -= 1) {
			EditorGUILayout.BeginHorizontal(util.style);
			for (int x = 0; x < displayWidth; x += 1) {
				Rect r = EditorGUILayout.GetControlRect(false, 16, util.style, GUILayout.Width(16));
				Level l = FindLevel(x+ox, y+oy);
				if (l != null) {
					if (GUI.Button(r, GUIContent.none, util.style)) {
						change = true;
						gm.currentLevel = l;
						Selection.activeGameObject = l.gameObject;
						SceneView.FrameLastActiveSceneView();
					}
					Color c = l.color;
					EditorGUI.DrawRect(r, c);
					if (FindLevel(x+ox+1, y+oy) != l) {
						// Connect to right
						EditorGUI.DrawRect(new Rect(r.center + new Vector2(4, -8), new Vector2(4,16)), Color.black);
					}
					if (FindLevel(x+ox-1, y+oy) != l) {
						EditorGUI.DrawRect(new Rect(r.center + new Vector2(-8, -8), new Vector2(4,16)), Color.black);
					}
					if (FindLevel(x+ox, y+oy+1) != l) {
						EditorGUI.DrawRect(new Rect(r.center + new Vector2(-8, -8), new Vector2(16,4)), Color.black);
					}
					if (FindLevel(x+ox, y+oy-1) != l) {
						EditorGUI.DrawRect(new Rect(r.center + new Vector2(-8, 4), new Vector2(16,4)), Color.black);
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}

	}
//
//	void DisplayMapOld(Rect r) {
//		foreach(Level l in gm.levels.levels) {
//			if (l == null) {
//				continue;
//			}
//			DrawFullRoomRect(r, l, (int)l.mapPosition.x, (int)l.mapPosition.y);
//			for (int dx = 0; dx < l.mapSize.x; dx+=1) {
//				for (int dy = 0; dy < l.mapSize.y; dy+=1) {
//					DrawSingleScreenRectButton(r, l, (int)l.mapPosition.x + dx, (int)l.mapPosition.y + dy);
//				}
//			}
//		}
//		DrawDoors(r);
//	}
//
//	void DrawDoors(Rect r) {
//		int minX = 0;
//		int maxX = displayWidth;
//		int minY = 0;
//		int maxY = displayHeight;
//		for (int x = minX; x < maxX; x += 1)  {
//			for (int y = minY; y < maxY; y+=1) {
//				Level l = FindLevel(x, y);
//				Level o = FindLevel(x+1, y);
//				if (l != null && o != null && l != o) {
//					DrawDoorToRight(r, x, y);
//				}
//			}
//		}
//	}
//
//	const int editorUIRoomSize = 20;
//	const int editorUIRoomPadding = 6;
//	const int editorUIRoomMargin = 10;
//
//	void DrawDoorToRight(Rect r, int x, int y) {
//		bool doorHere = gm.doors.DoorAt(x,y);
//		GUIStyle style = doorHere ? colors[Color.red] : colors[Color.black];
//		if (GUI.Button(new Rect(
//			r.x + (x+1)*editorUIRoomSize - editorUIRoomMargin,
//			-(r.y + y*editorUIRoomSize),
//			editorUIRoomMargin,
//			editorUIRoomSize - editorUIRoomMargin
//		), GUIContent.none, style)) {
//			change = true;
////			gm.doors.SetDoor(x, y, !doorHere);
//		}
//	}
//
//	void DrawFullRoomRect(Rect r, Level l, int x, int y) {
//		GUIStyle style = colors[Color.black];
//		GUI.Label(new Rect(
//			r.x + x*editorUIRoomSize + editorUIRoomPadding, 
//			-(r.y + y*editorUIRoomSize + editorUIRoomPadding - editorUIRoomMargin),
//			l.mapSize.x*editorUIRoomSize - 2*editorUIRoomPadding - editorUIRoomMargin, 
//			-(l.mapSize.y*editorUIRoomSize - 2*editorUIRoomPadding - editorUIRoomMargin)
//		), GUIContent.none, style);
//	}
//
//	void DrawSingleScreenRectButton(Rect r, Level l, int x, int y) {
//		GUIStyle style = currentLevel == l ? colors[Color.green] : colors[Color.blue];
//		if (GUI.Button(new Rect(
//			r.x + x*editorUIRoomSize, 
//			-(r.y + y*editorUIRoomSize),
//			editorUIRoomSize - editorUIRoomMargin, 
//			(editorUIRoomSize - editorUIRoomMargin)
//		), GUIContent.none, style)) {
//			change = true;
//			gm.currentLevel = l;
//			Selection.activeGameObject = l.gameObject;
//			SceneView.FrameLastActiveSceneView();
//		}
//	}
//
//	void OnSceneGUI() {
//		Debug.Log("DOINN IT UGHH");
//		Vector3 a = Vector3.Scale(viewPortPosition, GameManager.SCREEN_SIZE);
//		Vector3 b = Vector3.Scale(viewPortPosition + new Vector2(10, 0), GameManager.SCREEN_SIZE);
//		Vector3 c = Vector3.Scale(viewPortPosition + new Vector2(0, 10), GameManager.SCREEN_SIZE);
//		Vector3 d = Vector3.Scale(viewPortPosition + new Vector2(10, 10), GameManager.SCREEN_SIZE);
//		Debug.DrawLine(a,b);
//		Debug.DrawLine(a,c);
//		Debug.DrawLine(b,d);
//		Debug.DrawLine(c,d);
//	}
}