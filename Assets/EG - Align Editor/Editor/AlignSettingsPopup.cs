using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Editor window to change the tool settings
/// </summary>
public class AlignSettingsPopup : EditorWindow {

	void OnGUI() {
		if (AlignEditor.editorPath == null)
			AlignEditor.editorPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));

		title = "Align Editor v" + AlignEditor.VERSION + " Settings";
		maxSize = new Vector2(450, 200);
		minSize = maxSize;
		
		if (GUILayout.Button("Click here for web site & Documentation", EditorStyles.boldLabel)) {
			Application.OpenURL("http://www.equilibregames.com/align+editor");
		}
		GUILayout.FlexibleSpace();
		
		// Settings
		GUILayout.Label("Global settings", EditorStyles.toolbar, GUILayout.ExpandWidth(true));
		
		// Prefer to use boundaries / extents
		ExtentsGetter.BoundType newBound = (ExtentsGetter.BoundType) EditorGUILayout.EnumPopup("Extents to use", AlignEditor.PreferredBounds);
		if (newBound != AlignEditor.PreferredBounds) {
			Undo.RegisterUndo(this, "extents to use");
			AlignEditor.PreferredBounds = newBound;
		}
		// Prefer to align gameobject to collider angle or not
		bool rotateToTerrainAngle = EditorGUILayout.Toggle("Align to collider angle", AlignEditor.rotateToTerrainAngle);
		if (rotateToTerrainAngle != AlignEditor.rotateToTerrainAngle) {
			Undo.RegisterUndo(this, "Align to collider angle setting");
			AlignEditor.rotateToTerrainAngle = rotateToTerrainAngle;
		}
		
		// Layout
		GUILayout.Label("Editor layout", EditorStyles.toolbar, GUILayout.ExpandWidth(true));
		
		// Button display : skin and style

		// Icon Set / Skin
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		// show immediately one icon to the user
		EditorGUILayout.PrefixLabel("Icon Set");
		AlignEditor.IconSet newSkin = (AlignEditor.IconSet) EditorGUILayout.EnumPopup(AlignEditor.Skin);
		if (newSkin != AlignEditor.Skin) {
			Undo.RegisterUndo(this, "change icon set");
			AlignEditor.Skin = newSkin;
		}
		EditorGUILayout.EndHorizontal();

		// Button Display
		EditorGUILayout.BeginHorizontal();
		// show immediately a sample button to the user
		EditorGUILayout.PrefixLabel("Button display");
		ImagePosition newStyle = (ImagePosition) EditorGUILayout.EnumPopup(AlignEditor.ButtonStyle);
		if (newStyle != AlignEditor.ButtonStyle) {
			Undo.RegisterUndo(this, "change button style");
			AlignEditor.ButtonStyle = newStyle;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		Texture icon = AssetDatabase.LoadAssetAtPath(AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/alignMin.png", typeof(Texture)) as Texture;
		
		GUIContent myButtonContent = new GUIContent();
		myButtonContent.image = icon;
		myButtonContent.text = "Click";
		myButtonContent.tooltip = "This is a sample";
		
		ImagePosition wasPosition = GUI.skin.button.imagePosition;
		GUI.skin.button.imagePosition = AlignEditor.ButtonStyle;
		GUILayout.Label(myButtonContent, "Button");
		EditorGUILayout.EndHorizontal();

		GUI.skin.button.imagePosition = wasPosition;
		
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close")) {
			Close();
		}
	}
}
