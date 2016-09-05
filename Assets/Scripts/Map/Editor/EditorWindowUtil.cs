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
	

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
	{
		_property.intValue = EditorGUI.MaskField( _position, _label, _property.intValue, _property.enumNames );
	}
}
