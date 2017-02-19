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
/// Editor window to align and distribute objects
/// </summary>
public class ClassicWindow : EditorWindow {
	
	// GUI Main View Scroll position
	Vector2 mainScroll;

	// Creation of window
	[MenuItem("Window/Equilibre/Align Editor/Classic")]
	public static void Init() {
		GetWindow(typeof(ClassicWindow), false, "AE Classic v" + AlignEditor.VERSION);
	}
	
	// Update the editor window when user changes something (mainly useful when selecting objects)
	void OnInspectorUpdate() {
		Repaint();
	}

	public static void DetectAlignAxis ()
	{
		if (AlignEditor.autoAxis && SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null) {
			Camera myCam = SceneView.lastActiveSceneView.camera;
			if (myCam != null && myCam.transform != null) {
				if (myCam.transform.right == Vector3.up || myCam.transform.right == -Vector3.up || myCam.transform.right == -Vector3.forward || myCam.transform.right == -Vector3.right) {
					AlignEditor.axis = AlignEditor.AlignAxis.Y;
				}
				else if (myCam.transform.right == Vector3.forward) {
					AlignEditor.axis = AlignEditor.AlignAxis.Z;
				}
				else if (myCam.transform.right == Vector3.right) {
					AlignEditor.axis = AlignEditor.AlignAxis.X;
				}
			}
		}
	}
	
	/// <summary>
	/// GUI definition for Classic Tab
	/// </summary>
	void OnGUI() {
		if (AlignEditor.editorPath == null)
			AlignEditor.editorPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));

		title = "AE Classic v" + AlignEditor.VERSION;
		mainScroll = EditorGUILayout.BeginScrollView(mainScroll);

		// New in 1.9 : the alignment is linked to the current Tool : None=Move=View=Position, Rotate=Rotation, Scale=Scale (!)
		
		// Axis alignement selection
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.PrefixLabel("Axis");
		bool wantedAutoAxis = GUILayout.Toggle(AlignEditor.autoAxis, "auto", "MiniButton", GUILayout.Width(70));
		if (wantedAutoAxis != AlignEditor.autoAxis) {
			Undo.RegisterUndo(this, "change auto axis");
			AlignEditor.autoAxis = wantedAutoAxis;
		}
		// Auto set the axis from the last active view camera
		DetectAlignAxis ();
		if (AlignEditor.autoAxis)
			GUILayout.Label("Detected: " + AlignEditor.axis);
		else {
			AlignEditor.AlignAxis wantedAxis = (AlignEditor.AlignAxis)EditorGUILayout.EnumPopup(AlignEditor.axis);
			if (wantedAxis != AlignEditor.axis) {
				Undo.RegisterUndo(this, "change axis");
				AlignEditor.axis = wantedAxis;
			}
		}
		EditorGUILayout.EndHorizontal();
		// Sort by axis selection
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Sort by");
		bool wantedUseAxis = GUILayout.Toggle(AlignEditor.useAlignAxis, "identical", "MiniButton", GUILayout.Width(70));
		if (wantedUseAxis != AlignEditor.useAlignAxis) {
			Undo.RegisterUndo(this, "change use align axis");
			AlignEditor.useAlignAxis = wantedUseAxis;
		}
		if (AlignEditor.useAlignAxis) {
			// Can not sort by "all" axis, indicates that to the user (X is the default)
			switch (AlignEditor.axis) {
			case AlignEditor.AlignAxis.Z:
				AlignEditor.sortBy = DistributionManager.SortAxis.Z;
				break;
			case AlignEditor.AlignAxis.Y:
				AlignEditor.sortBy = DistributionManager.SortAxis.Y;
				break;
			default:
				AlignEditor.sortBy = DistributionManager.SortAxis.X;
				break;
			}
			GUILayout.Label("Detected:" + AlignEditor.sortBy.ToString());
		} else {
			DistributionManager.SortAxis wantedSortBy = (DistributionManager.SortAxis) EditorGUILayout.EnumPopup(AlignEditor.sortBy);
			if (wantedSortBy != AlignEditor.sortBy) {
				Undo.RegisterUndo(this, "change sort by");
				AlignEditor.sortBy = wantedSortBy;
			}
		}
		EditorGUILayout.EndHorizontal();
		
		if (Selection.transforms.Length > 1) {
			EditorGUILayout.LabelField("Align with", Selection.activeTransform.name);
		} else {
			GUILayout.Label("Select at least 2 objects", EditorStyles.boldLabel);
		}
		
		GUILayout.Space(4);
		EditorGUILayout.BeginHorizontal();
		AlignButtonGUI(Selection.transforms, false);
		AlignEditor.SettingsButtonGUI();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndScrollView();
	}

	/// <summary>
	/// Display the GUI buttons for alignment features
	/// </summary>
	/// <param name="forceUseful">
	/// <see cref="true"/> to force the Fall button enabled if a selection is made : it can only align on Position
	/// </param>
	public static void AlignButtonGUI(Transform[] userSelection, bool forceUseful) {
		bool GuiWasEnabled = GUI.enabled;
		GUI.enabled = (userSelection.Length > 1);
		ImagePosition wasPosition = GUI.skin.button.imagePosition;
		GUI.skin.button.imagePosition = AlignEditor.ButtonStyle;

		GUIContent minValueGuiContent = new GUIContent ("Min", AssetDatabase.LoadAssetAtPath (AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/alignMin.png", typeof(Texture)) as Texture, "Align on minimum " + AlignEditor.axis + " value");
		if (GUILayout.Button(minValueGuiContent)) {
			// Align to the min value
			AlignEditor.landmark = AlignManager.Landmark.minimum;
			Align();
		}
		GUIContent meanValueGuiContent = new GUIContent ("Medium", AssetDatabase.LoadAssetAtPath (AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/alignMean.png", typeof(Texture)) as Texture, "Align on medium " + AlignEditor.axis + " value");
		if (GUILayout.Button(meanValueGuiContent)) {
			// Align to the center
			AlignEditor.landmark = AlignManager.Landmark.mean;
			Align();
		}
		GUIContent maxValueGuiContent = new GUIContent ("Max", AssetDatabase.LoadAssetAtPath (AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/alignMax.png", typeof(Texture)) as Texture, "Align on maximum " + AlignEditor.axis + " value");
		if (GUILayout.Button(maxValueGuiContent)) {
			// Align to the max value
			AlignEditor.landmark = AlignManager.Landmark.maximum;
			Align();
		}
		GUIContent lineDistributionGuiContent = new GUIContent ("Distribute", AssetDatabase.LoadAssetAtPath (AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/lineDistribution.png", typeof(Texture)) as Texture, "Make a linear distribution on " + AlignEditor.axis + " axis");
		if (GUILayout.Button(lineDistributionGuiContent)) {
			// Distribute objects
			AlignEditor.landmark = AlignManager.Landmark.distributed;
			Distribute();
		}
		GUIContent switchValuesGuiContent = new GUIContent ("Switch", AssetDatabase.LoadAssetAtPath (AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/switch.png", typeof(Texture)) as Texture, "Switch " + Tools.current.ToString() + " values");
		if (GUILayout.Button(switchValuesGuiContent)) {
			// Switch values on objects
			SwitchValues();
		}
		
		GUI.enabled = (userSelection.Length >= 1) && (AlignEditor.PositionTool || forceUseful);
		
		if (GUILayout.Button(new GUIContent("Fall", AssetDatabase.LoadAssetAtPath(AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/fall.png", typeof(Texture)) as Texture, "Fall on terrain or objects downside"))) {
			// Distribute objects
			AlignEditor.landmark = AlignManager.Landmark.minimum;
			FallOnTerrain();
		}
		GUI.enabled = GuiWasEnabled;
		GUI.skin.button.imagePosition = wasPosition;
	}

		// Switch for align functions
	static void Align() {
		if (Selection.transforms.Length == 0)
			return;
		
		if (AlignEditor.ScaleTool) {
			Undo.RegisterUndo(Selection.transforms, "Align Scale");
			AlignManager.AlignScale(Selection.activeTransform, Selection.transforms, AlignEditor.Axis [(int)AlignEditor.axis], AlignEditor.landmark);
		} else if (AlignEditor.RotationTool) {
			Undo.RegisterUndo(Selection.transforms, "Align Rotation");
			AlignManager.AlignRotation(Selection.activeTransform, Selection.transforms, AlignEditor.Axis [(int)AlignEditor.axis], AlignEditor.landmark);
		} else {
			Undo.RegisterUndo(Selection.transforms, "Align Position");
			AlignManager.AlignPosition(Selection.activeTransform, Selection.transforms, AlignEditor.Axis [(int)AlignEditor.axis], AlignEditor.landmark, AlignEditor.PreferredBounds);
		}
	}

	// Switch for distribute functions
	static void Distribute() {
		if (AlignEditor.ScaleTool) {
			Undo.RegisterUndo(Selection.transforms, "Distribute Scale");
			DistributionManager.DistributeScale(Selection.activeTransform, Selection.transforms, AlignEditor.sortBy, AlignEditor.Axis [(int)AlignEditor.axis]);
		} else if (AlignEditor.RotationTool) {
			Undo.RegisterUndo(Selection.transforms, "Distribute Rotation");
			DistributionManager.DistributeRotation(Selection.activeTransform, Selection.transforms, AlignEditor.sortBy, AlignEditor.Axis [(int)AlignEditor.axis]);
		} else {
			Undo.RegisterUndo(Selection.transforms, "Distribute Position");
			DistributionManager.DistributePosition(Selection.activeTransform, Selection.transforms, AlignEditor.sortBy, AlignEditor.Axis [(int)AlignEditor.axis]);
		}
	}
	
	// Switch for switch functions (!)
	static void SwitchValues() {
		if (AlignEditor.ScaleTool) {
			Undo.RegisterUndo(Selection.transforms, "Switch Scale");
			AlignManager.SwitchScale(Selection.transforms);
		} else if (AlignEditor.RotationTool) {
			Undo.RegisterUndo(Selection.transforms, "Switch Rotation");
			AlignManager.SwitchRotation(Selection.transforms);
		} else {
			Undo.RegisterUndo(Selection.transforms, "Switch Position");
			AlignManager.SwitchPosition(Selection.transforms);
		}
	}
	
	// Call for the fall on terrain feature, after calling the RegisterUndo
	static void FallOnTerrain() {
		Undo.RegisterSceneUndo("Fall on terrain");
		AlignManager.FallOnTerrain(Selection.transforms, AlignEditor.rotateToTerrainAngle, AlignEditor.PreferredBounds);
	}

}
