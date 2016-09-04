using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorWindowUtil {
	public static void RepaintAll() {
		Repaint<LevelEditorWindow>();
		Repaint<LevelEditorPaletteWindow>();
		Repaint<MapConfig>();
	}

	public static void Repaint<T>() where T : EditorWindow {
		T[] windows = Resources.FindObjectsOfTypeAll<T>();
		if(windows != null && windows.Length > 0)
		{
			windows[0].Repaint();
		}
	}
}
