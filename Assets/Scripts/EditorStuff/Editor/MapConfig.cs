
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
	GameObject levelContainer;
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

	void OnGUI () {
		Debug.Log("hey howdy there.");
		// We should have a game manager.
//		return;
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		levelContainer = GameObject.Find("Levels");
		if (gm == null) {
			Debug.LogError("No Game Manager in Screen! Aborting.");
			return;
		} else if (levelContainer == null) {
			Debug.Log("No Level Container. Creating one.");
			levelContainer = new GameObject("Levels");
		}
		Debug.Log("hi");
		Debug.Log(gm);
		Debug.Log(gm.levels.Count);

		if (EditorApplication.isPlaying != inPlayModeLastFrame) {
			colors = new Dictionary<Color, GUIStyle>();
		}
		inPlayModeLastFrame = EditorApplication.isPlaying;

		AddColor(Color.blue);
		AddColor(Color.green);
		AddColor(Color.red);
		AddColor(Color.black);


		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add new Level")) {
			GameObject levelGO = new GameObject();
			levelGO.transform.parent = levelContainer.transform;
			Level l = levelGO.AddComponent<Level>();
			gm.levels.Add(l);
			DisableAllLevelsExceptFor(l);
		}
		if (GUILayout.Button("Re-scan for current level")) {
			foreach(Level l in gm.levels) {
				if (l.gameObject.activeSelf && currentLevel != l) {
					DisableAllLevelsExceptFor(l);
				}
			}
		}
		EditorGUILayout.EndHorizontal();

		MoveViewportOptions ();
		MoveCurrentLevelPositionOptions ();
		Debug.Log("woop woop....");
		DisplayMap(EditorGUILayout.GetControlRect());
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

	void MoveViewportOptions()
	{
		GUILayout.Label ("Move Viewport");
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<")) {
		}
		;
		if (GUILayout.Button ("v")) {
		}
		;
		if (GUILayout.Button ("^")) {
		}
		;
		if (GUILayout.Button (">")) {
		}
		;
		EditorGUILayout.EndHorizontal ();
	}

	void MoveCurrentLevelPositionOptions()
	{
		GUILayout.Label ("Move Current Level");
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<")) {
			currentLevel.mapPosition += new Vector2(-1,0);
		}
		if (GUILayout.Button ("v")) {
			currentLevel.mapPosition += new Vector2(0,1);
		}
		if (GUILayout.Button ("^")) {
			currentLevel.mapPosition += new Vector2(0,-1);
		}
		if (GUILayout.Button (">")) {
			currentLevel.mapPosition += new Vector2(1,0);
		}
		EditorGUILayout.EndHorizontal ();
	}

	void DisplayMap(Rect r) {
		foreach(Level l in gm.levels) {
			DrawFullRoomRect(r, l, (int)l.mapPosition.x, (int)l.mapPosition.y);
			for (int dx = 0; dx < l.mapSize.x; dx+=1) {
				for (int dy = 0; dy < l.mapSize.y; dy+=1) {
					DrawSingleScreenRectButton(r, l, (int)l.mapPosition.x + dx, (int)l.mapPosition.y + dy);
				}
			}
		}
	}

	const int editorUIRoomSize = 20;
	const int editorUIRoomPadding = 6;
	const int editorUIRoomMargin = 10;



	void DrawFullRoomRect(Rect r, Level l, int x, int y) {
		GUIStyle style = colors[Color.black];
		GUI.Label(new Rect(
			r.x + x*editorUIRoomSize + editorUIRoomPadding, 
			r.y + y*editorUIRoomSize + editorUIRoomPadding, 
			l.mapSize.x*editorUIRoomSize - 2*editorUIRoomPadding - editorUIRoomMargin, 
			l.mapSize.y*editorUIRoomSize - 2*editorUIRoomPadding - editorUIRoomMargin
		), GUIContent.none, style);
	}

	void DrawSingleScreenRectButton(Rect r, Level l, int x, int y) {
		GUIStyle style = currentLevel == l ? colors[Color.green] : colors[Color.blue];
		if (GUI.Button(new Rect(r.x + x*editorUIRoomSize, r.y + y*editorUIRoomSize, editorUIRoomSize - editorUIRoomMargin, editorUIRoomSize - editorUIRoomMargin), GUIContent.none, style)) {
			DisableAllLevelsExceptFor(l);
		}
	}

	void DisableAllLevelsExceptFor(Level _l) {
		foreach(Level l in gm.levels) {
			l.gameObject.SetActive(l == _l);
		}
		currentLevel = _l;
		Selection.activeGameObject = _l.gameObject;
	}

}