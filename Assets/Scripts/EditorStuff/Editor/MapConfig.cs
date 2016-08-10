
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class MapConfig : EditorWindow {
	[MenuItem ("Window/Map Configuration")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(MapConfig));
	}

	Dictionary<Color, GUIStyle> colors = new Dictionary<Color, GUIStyle>();

	GameManager gm;
	GameObject levelContainer;
	Level currentLevel;

	void OnGUI () {
		// We should have a game manager.
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		levelContainer = GameObject.Find("Levels");
		if (gm == null) {
			Debug.LogError("No Game Manager in Screen! Aborting.");
			return;
		} else if (levelContainer == null) {
			Debug.Log("No Level Container. Creating one.");
			levelContainer = new GameObject("Levels");
		}

		AddColor(Color.blue);
		AddColor(Color.green);
		AddColor(Color.red);

		MoveViewport ();
		MoveCurrentLevelPosition ();
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

	static void MoveViewport()
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

	void MoveCurrentLevelPosition()
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
			Debug.Log("Drawing a level...");
			for (int dx = 0; dx < l.mapSize.x; dx+=1) {
				for (int dy = 0; dy < l.mapSize.y; dy+=1) {
					DrawMapRect(r, l, (int)l.mapPosition.x + dx, (int)l.mapPosition.y + dy);
				}
			}
		}
	}

	void DrawMapRect(Rect r, Level l, int x, int y) {
		GUIStyle style = currentLevel == l ? colors[Color.blue] : colors[Color.green];
		if (GUI.Button(new Rect(r.x + x*25, r.y + y*25, 20, 20), GUIContent.none, style)) {
			DisableAllLevelsExceptFor(l);
		}
	}

	void DisableAllLevelsExceptFor(Level _l) {
		foreach(Level l in gm.levels) {
			l.gameObject.SetActive(l == _l);
		}
		currentLevel = _l;
	}
}