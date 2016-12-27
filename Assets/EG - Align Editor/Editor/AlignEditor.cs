// by Equilibre Games http://www.equilibregames.com
// Lead: Frederic Rolland-Porche
// listed in unity asset store since version 1.2
//
// Post your feature request on Unity3D forum / Assets and Asset Store / Align Editor !
//
// Versions History : see readme.txt
//
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// Align editor Enumerations and Editor Preferences Management
/// </summary>
public class AlignEditor
{

	// Current version is :
	public static string VERSION = "2.2";

#region enumerations
	// All Axis that can be chosen in the editor : All, X, Y, Z
	public static Vector3[] Axis = new Vector3[] { Vector3.one, Vector3.right, Vector3.up, Vector3.forward };
	// Align Axis declaration (used as index for above Vector3 list)
	public enum AlignAxis { 
		X   = 0,
		Y   = 1,
		Z   = 2,
	    All = 3}
	// Grid axis declaration
	public enum GridAxis
	{
		All,
		XY,
		XZ,
		YZ
	}

	// Icon set enum
	public enum IconSet
	{
		AllBlue,
		Colored
	}

#endregion

#region settings
	// Selected Skin (default from v1.6 is SixColor)
	public static IconSet Skin {
		get {
			return (IconSet)Enum.Parse(typeof(IconSet), EditorPrefs.GetString ("AlignEditor.Skin", "" + IconSet.Colored));
		}
		set {
			EditorPrefs.SetString ("AlignEditor.Skin", value.ToString ());
		}
	}
	// Preferred bounds to use
	public static ExtentsGetter.BoundType PreferredBounds {
		get {
			return (ExtentsGetter.BoundType)Enum.Parse (typeof(ExtentsGetter.BoundType), EditorPrefs.GetString ("AlignEditor.PreferredBounds", "" + ExtentsGetter.BoundType.Collider));
		}
		set {
			EditorPrefs.SetString ("AlignEditor.PreferredBounds", value.ToString ());
		}
	}
	// Buton display (New from 2.0, default is ImageOnly)
	public static ImagePosition ButtonStyle {
		get {
			return (ImagePosition)Enum.Parse (typeof(ImagePosition), EditorPrefs.GetString ("AlignEditor.ImagePosition", "" + ImagePosition.ImageOnly));
		}
		set {
			EditorPrefs.SetString ("AlignEditor.ImagePosition", value.ToString ());
		}
	}
#endregion

#region alignment settings
	public static bool PositionTool {
		get {
			return (Tools.current == Tool.Move) || (Tools.current == Tool.None) || (Tools.current == Tool.View);
		}
	}

	public static bool RotationTool {
		get {
			return (Tools.current == Tool.Rotate);
		}
	}

	public static bool ScaleTool {
		get {
			return (Tools.current == Tool.Scale);
		}
	}
	// Land mark
	public static AlignManager.Landmark landmark {
		get {
			return (AlignManager.Landmark)Enum.Parse (typeof(AlignManager.Landmark), EditorPrefs.GetString ("AlignEditor.Landmark", "" + AlignManager.Landmark.mean));
		}
		set {
			EditorPrefs.SetString ("AlignEditor.Landmark", value.ToString ());
		}

	}
	// Axis
	public static AlignAxis axis {
		get {
			return (AlignAxis)EditorPrefs.GetInt ("AlignEditor.Axis", 0);
		}
		set {
			EditorPrefs.SetInt ("AlignEditor.Axis", (int)value);
		}

	}
	// Auto assign axis ?
	public static bool autoAxis {
		get {
			return EditorPrefs.GetBool ("AlignEditor.AutoAxis", true);
		}
		set {
			EditorPrefs.SetBool ("AlignEditor.AutoAxis", value);
		}

	}
	// Sort Axis
	public static DistributionManager.SortAxis sortBy {
		get {
			return (DistributionManager.SortAxis)Enum.Parse (typeof(DistributionManager.SortAxis), EditorPrefs.GetString ("AlignEditor.SortAxis", "" + DistributionManager.SortAxis.None));
		}
		set {
			EditorPrefs.SetString ("AlignEditor.SortAxis", value.ToString ());
		}

	}
	// use alignement axis ?
	public static bool useAlignAxis {
		get {
			return EditorPrefs.GetBool ("AlignEditor.UseAlignAxis", false);
		}
		set {
			EditorPrefs.SetBool ("AlignEditor.UseAlignAxis", value);
		}
	}
	// align transform to terrain angle ?
	public static bool rotateToTerrainAngle {
		get {
			return EditorPrefs.GetBool ("AlignEditor.TerrainAngle", true);
		}
		set {
			EditorPrefs.SetBool ("AlignEditor.TerrainAngle", value);
		}
	}
#endregion

#region grid settings
	// Grid distribution : grid size - ugly way to get/set but I didn't wan't more than one property
	public static Vector3 gridSize {
		get {
			String[] values = EditorPrefs.GetString ("AlignEditor.GridSize", "1#1#1").Split ('#');
			return new Vector3 (System.Single.Parse (values [0]), System.Single.Parse (values [1]), System.Single.Parse (values [2]));
		}
		set {
			EditorPrefs.SetString ("AlignEditor.GridSize", String.Join ("#", new String[] {value.x.ToString (), value.y.ToString (), value.z.ToString ()}));
		}
	}
	// Grid every axis auto calculated size (from selection length)
	public static int calculatedGridSize {
		get {
			return EditorPrefs.GetInt ("AlignEditor.CalculatedGridSize", 1);
		}
		set {
			EditorPrefs.SetInt ("AlignEditor.CalculatedGridSize", value);
		}
	}
	// Auto grid size
	public static bool autoSize {
		get {
			return EditorPrefs.GetBool ("AlignEditor.AutoSize", false);
		}
		set {
			EditorPrefs.SetBool ("AlignEditor.AutoSize", value);
		}

	}
	// Grid offset
	public static Vector3 gridOffset {
		get {
			String[] values = EditorPrefs.GetString ("AlignEditor.GridOffset", "0#0#0").Split ('#');
			return new Vector3 (System.Single.Parse (values [0]), System.Single.Parse (values [1]), System.Single.Parse (values [2]));
		}
		set {
			EditorPrefs.SetString ("AlignEditor.GridOffset", String.Join ("#", new String[] {value.x.ToString (), value.y.ToString (), value.z.ToString ()}));
		}
	}
#endregion

#region GUI
	// Store the path to the align editor assets
	public static string editorPath = null;
	private static AlignSettingsPopup settings;

	public static void SettingsButtonGUI ()
	{
		ImagePosition wasPosition = GUI.skin.button.imagePosition;
		GUI.skin.button.imagePosition = AlignEditor.ButtonStyle;
		if (GUILayout.Button(new GUIContent("Settings", AssetDatabase.LoadAssetAtPath(AlignEditor.editorPath + "/Icons/" + AlignEditor.Skin + "/settings.png", typeof(Texture)) as Texture, "Open the settings popup"))) {
			DisplaySettingsWindow ();
		}
		GUI.skin.button.imagePosition = wasPosition;
	}

	[MenuItem("Window/Equilibre/Align Editor/Settings")]
	public static void DisplaySettingsWindow ()
	{
		if (settings)
			settings.Close ();
		// 2.1 : remove previously used property
		if (EditorPrefs.HasKey ("AlignEditor.IconSet"))
			EditorPrefs.DeleteKey ("AlignEditor.IconSet");
		// 1.9 : remove previously used property
		if (EditorPrefs.HasKey ("AlignEditor.AlignField"))
			EditorPrefs.DeleteKey ("AlignEditor.AlignField");

		settings = AlignSettingsPopup.CreateInstance<AlignSettingsPopup> ();
		settings.ShowUtility ();
	}
#endregion
}
