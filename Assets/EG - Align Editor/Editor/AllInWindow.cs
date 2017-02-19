// by Equilibre Games http://www.equilibregames.com
// Lead: Frederic Rolland-Porche
// listed in unity asset store since version 1.2
//
// Post your feature request on Unity3D forum / Assets and Asset Store / Align Editor !
//
using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Editor window with buttons only for minimum space usage
/// </summary>
public class AllInWindow : EditorWindow {

	// Creation of window
	[MenuItem("Window/Equilibre/Align Editor/Buttons Only")]
	public static void Init() {
		GetWindow(typeof(AllInWindow), false, "AllIn v" + AlignEditor.VERSION);
	}

	// Update the editor window when user changes something (mainly useful when selecting objects)
	void OnInspectorUpdate() {
		Repaint();
	}

	void OnGUI() {
		if (AlignEditor.editorPath == null)
			AlignEditor.editorPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));

		title = "AllIn v" + AlignEditor.VERSION;
		ImagePosition wasPosition = AlignEditor.ButtonStyle;
		// v2.0 : automatic layout from ratio width/height
		if (this.position.width > this.position.height) {
			EditorGUILayout.BeginHorizontal();
			this.minSize = new Vector2(292, 58);
		} else {
			// Force image above for vertical layout
			if (wasPosition == ImagePosition.ImageLeft)
				AlignEditor.ButtonStyle = ImagePosition.ImageAbove;
			this.minSize = new Vector2(52, 256);
		}
		ClassicWindow.DetectAlignAxis ();
		ClassicWindow.AlignButtonGUI(Selection.transforms, true);
		GridWindow.GridButtonGUI(Selection.transforms);
		CameraWindow.CameraButtonGUI(Selection.transforms);

		AlignEditor.SettingsButtonGUI();
		if (this.position.width > this.position.height)
			EditorGUILayout.EndHorizontal();
		else
			AlignEditor.ButtonStyle = wasPosition;
		// Restore the previous state of button style
	}
}
