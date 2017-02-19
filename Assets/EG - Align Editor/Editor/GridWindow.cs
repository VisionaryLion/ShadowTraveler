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
/// Editor Window to distribute and duplicate objects in 3D grid
/// </summary>
public class GridWindow : EditorWindow {

	// GUI Main View Scroll position
	Vector2 mainScroll;

		// Creation of window
	[MenuItem("Window/Equilibre/Align Editor/Grid")]
	public static void Init() {
		GetWindow(typeof(GridWindow), false, "AE Grid v" + AlignEditor.VERSION);
	}

	// Update the editor window when user changes something (mainly useful when selecting objects)
	void OnInspectorUpdate() {
		Repaint();
	}

	void OnGUI() {
		if (AlignEditor.editorPath == null)
			AlignEditor.editorPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));

		title = "AE Grid v" + AlignEditor.VERSION;
		mainScroll = EditorGUILayout.BeginScrollView(mainScroll);
		// auto grid size is enabled for distribution (if there is more than 1 selected object)
		bool GUIWasEnabled = GUI.enabled;
		GUI.enabled = (Selection.transforms.Length > 1);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Grid Size");
		Vector3 gridSize = AlignEditor.gridSize;
		bool wantedAutoSize = GUILayout.Toggle(AlignEditor.autoSize, "auto", "MiniButton", GUILayout.Width(70));
		if (wantedAutoSize != AlignEditor.autoSize) {
			Undo.RegisterUndo(this, "change auto grid size");
			AlignEditor.autoSize = wantedAutoSize;
		}
		EditorGUILayout.EndHorizontal();
		GUI.enabled = GUIWasEnabled;
		
		EditorGUILayout.BeginHorizontal();
		if (AlignEditor.autoSize && (Selection.transforms.Length > 1)) {
			AlignEditor.calculatedGridSize = Mathf.Max(Mathf.CeilToInt(Mathf.Pow(Selection.transforms.Length, 1 / 3)), 2);
			EditorGUILayout.PrefixLabel("Every axis");
			GUILayout.Label("" + AlignEditor.calculatedGridSize);
		} else {
			Vector3 wantedGridSize = gridSize;
			GUILayout.Label("X");
			wantedGridSize.x = Mathf.Max(1, EditorGUILayout.IntField((int) gridSize.x));
			GUILayout.Label("Y");
			wantedGridSize.y = Mathf.Max(1, EditorGUILayout.IntField((int) gridSize.y));
			GUILayout.Label("Z");
			wantedGridSize.z = Mathf.Max(1, EditorGUILayout.IntField((int) gridSize.z));
			if (!wantedGridSize.Equals(gridSize)) {
				Undo.RegisterUndo(this, "change grid size");
				AlignEditor.gridSize = wantedGridSize;
			}
		}
		EditorGUILayout.EndHorizontal();
		
		// Offset is enabled only for duplication (if there is only 1 selected object)
		GUIWasEnabled = GUI.enabled;
		GUI.enabled = (Selection.transforms.Length == 1);
		Vector3 wantedOffset = EditorGUILayout.Vector3Field("Offset", AlignEditor.gridOffset);
		if (!wantedOffset.Equals(AlignEditor.gridOffset)) {
			Undo.RegisterUndo(this, "change grid offset");
			AlignEditor.gridOffset = wantedOffset;
		}
		GUI.enabled = GUIWasEnabled;
		
		if (Selection.transforms.Length == 1) {
			EditorGUILayout.LabelField("Duplicate", Selection.activeTransform.name);
		} else if (Selection.transforms.Length > 1) {
			EditorGUILayout.LabelField("Distribute", "" + Selection.transforms.Length + " objects");
		} else {
			GUILayout.Label("Select 1 object to duplicate, N to distribute", EditorStyles.boldLabel);
		}
		EditorGUILayout.BeginHorizontal();
		GridButtonGUI(Selection.transforms);
		AlignEditor.SettingsButtonGUI();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndScrollView();
	}

	public static void GridButtonGUI(Transform[] userSelection) {
		bool GuiWasEnabled = GUI.enabled;
		GUI.enabled = (userSelection.Length >= 1);
		ImagePosition wasPosition = GUI.skin.button.imagePosition;
		GUI.skin.button.imagePosition = AlignEditor.ButtonStyle;

		GUIContent  buttonContent = new GUIContent ("Grid", AssetDatabase.LoadAssetAtPath (AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/gridDistribution.png", typeof(Texture)) as Texture, "Grid distribution");
		if (Selection.transforms.Length > 1)
			buttonContent.tooltip = "Distribute " + userSelection.Length + " objects in a grid";
		else if (Selection.transforms.Length == 1)
			buttonContent.tooltip = "Duplicate " + userSelection[0].name + " in a grid";
		if (GUILayout.Button(buttonContent)) {
			// Detect the duplicate mode from the selections :
			if (Selection.transforms.Length > 1) {
				// Many objects : duplicate object
				// Distribute objects in a grid
				AlignEditor.landmark = AlignManager.Landmark.distributed;
				DistributeInGrid(Selection.activeTransform, userSelection);
			} else if (userSelection.Length == 1) {
				// One prefab : duplicate prefab
				// Distribute objects in a grid and duplicate objects
				AlignEditor.landmark = AlignManager.Landmark.distributed;
				DuplicateInGrid(userSelection [0].gameObject);
			}
		}
		GUI.skin.button.imagePosition = wasPosition;
		GUI.enabled = GuiWasEnabled;
	}

	// Switch for distribute in grid functions
	public static void DistributeInGrid(Transform activeTransform, Transform[] userSelection) {
		Undo.RegisterUndo(userSelection, "Distribute In Grid");
		Vector3 gridVector = AlignEditor.gridSize;
		if (AlignEditor.autoSize)
			gridVector = Vector3.one * AlignEditor.calculatedGridSize;
		DistributionManager.DistributePositionInGrid(activeTransform, userSelection, gridVector);
	}

	public static void DuplicateInGrid(GameObject userSelectedGameObject) {
		Undo.RegisterSceneUndo("Duplicate In Grid");
		Vector3 gridVector = AlignEditor.gridSize;
		// Get the active object
		GameObject originalGO = null;
		// If we find a prefab, duplicate with prefab link, otherwise duplicate object only
		if (PrefabUtility.GetPrefabType(userSelectedGameObject) != PrefabType.None) {
			originalGO = PrefabUtility.GetPrefabParent(userSelectedGameObject) as GameObject;
		}
		
		DuplicateManager.DuplicateInGrid(originalGO, userSelectedGameObject, gridVector, AlignEditor.gridOffset, AlignEditor.PreferredBounds);
	}
}
