// by Equilibre Games http://www.equilibregames.com
// Lead: Frederic Rolland-Porche
// listed in unity asset store since version 1.2
//
// Post your feature request on Unity3D forum / Assets and Asset Store / Align Editor !
//
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Editor window to align and distribute objects
/// </summary>
public class CameraWindow : EditorWindow {

	// GUI Main View Scroll position
	Vector2 mainScroll;
	
	// Creation of window
	[MenuItem("Window/Equilibre/Align Editor/Camera and Screen")]
	public static void Init() {
		GetWindow(typeof(CameraWindow), false, "AE Camera v" + AlignEditor.VERSION);
	}

	// Update the editor window when user changes something (mainly useful when selecting objects)
	void OnInspectorUpdate() {
		Repaint();
	}
	
	/// <summary>
	/// GUI definition for Classic Tab
	/// </summary>
	void OnGUI() {
		if (AlignEditor.editorPath == null)
			AlignEditor.editorPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));

		title = "AE Camera v" + AlignEditor.VERSION;
		this.minSize = new Vector2(180, 56);
		mainScroll = EditorGUILayout.BeginScrollView(mainScroll);
		GUILayout.BeginHorizontal();
		CameraButtonGUI (Selection.transforms);
		AlignEditor.SettingsButtonGUI();
		GUILayout.EndHorizontal();
		EditorGUILayout.EndScrollView();
	}

	public static void CameraButtonGUI(Transform[] userSelection) {
		ImagePosition wasPosition = GUI.skin.button.imagePosition;
		// We set the image position from the user settings only for this part of the GUI
		GUI.skin.button.imagePosition = AlignEditor.ButtonStyle;

		GUIContent faceCameraGuiContent = new GUIContent ("LookAt", AssetDatabase.LoadAssetAtPath (AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/cameraFacing.png", typeof(Texture)) as Texture, "Make objects face camera");
		if (GUILayout.Button(faceCameraGuiContent)) {
			OrientToCamera(userSelection);
		}
		GUIContent screenCenterGuiContent = new GUIContent ("Center", AssetDatabase.LoadAssetAtPath (AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/screenCenter.png", typeof(Texture)) as Texture, "Center objects in screen");
		if (GUILayout.Button(screenCenterGuiContent)) {
			ScreenCenter(userSelection);
		}
		
		GUI.skin.button.imagePosition = wasPosition;
	}
	
	private static void OrientToCamera(Transform[] userSelection) {
		if (userSelection.Length == 0)
			return;

		Camera myCam = Camera.current;
		if (myCam == null && SceneView.lastActiveSceneView != null)
			myCam = SceneView.lastActiveSceneView.camera;
		
		if (myCam == null) {
			Debug.LogWarning("No Camera not found to look at !");
			return;
		}
		
		Undo.RegisterUndo(userSelection, "Orient to Camera");
		CameraManager.OrientToCamera(userSelection, myCam);
	}

	private static void ScreenCenter(Transform[] userSelection) {
		if (userSelection.Length == 0)
			return;
		
		Camera myCam = Camera.current;
		if (myCam == null && SceneView.lastActiveSceneView != null)
			myCam = SceneView.lastActiveSceneView.camera;
		
		if (myCam == null) {
			Debug.LogWarning("No Camera not found to look at !");
			return;
		}		
		Undo.RegisterUndo(userSelection, "Center in screen");
		CameraManager.CenterInScreen(userSelection, myCam);
	}
	
}
