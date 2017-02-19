using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransformViewerWindow : EditorWindow
{	
	enum ViewerMode
	{
		Position,
		Rotation,
		Scale
	}

	private ViewerMode mMode = ViewerMode.Position;

	/// <summary>
	/// The transform whose properties we want to view.
	/// </summary>
	private Transform mTransform;


//	private AnimationCurve mCurveToDraw;

	/// <summary>
	/// The curve to be drawn (transform.localposition.x)
	/// </summary>
	private AnimationCurve mCurvePosX;

	/// <summary>
	/// The curve to be drawn (transform.localposition.y)
	/// </summary>
	private AnimationCurve mCurvePosY;

	/// <summary>
	/// The curve to be drawn (transform.localposition.z)
	/// </summary>
	private AnimationCurve mCurvePosZ;

	private AnimationCurve mCurveRotX;
	private AnimationCurve mCurveRotY;
	private AnimationCurve mCurveRotZ;

	private AnimationCurve mCurveScaleX;
	private AnimationCurve mCurveScaleY;
	private AnimationCurve mCurveScaleZ;

//	private Color mCurveColor;

	/// <summary>
	/// List of keys to add to the curve.
	/// </summary>
	private List<Keyframe> keysPosX;

	/// <summary>
	/// List of keys to add to the curve.
	/// </summary>
	private List<Keyframe> keysPosY;

	/// <summary>
	/// List of keys to add to the curve.
	/// </summary>
	private List<Keyframe> keysPosZ;

	private List<Keyframe> keysRotX;
	private List<Keyframe> keysRotY;
	private List<Keyframe> keysRotZ;

	private List<Keyframe> keysScaleX;
	private List<Keyframe> keysScaleY;
	private List<Keyframe> keysScaleZ;

	private float lastPosX = float.MinValue;
	private float lastPosY = float.MinValue;
	private float lastPosZ = float.MinValue;

	private float lastRotX = float.MinValue;
	private float lastRotY = float.MinValue;
	private float lastRotZ = float.MinValue;

	private float lastScaleX = float.MinValue;
	private float lastScaleY = float.MinValue;
	private float lastScaleZ = float.MinValue;

	[MenuItem("Window/Transform Viewer")]
	static void Init ()
	{
		GetWindow <TransformViewerWindow>();
	}

	void OnEnable()
	{
		ResetValues();
	}

	private void ResetValues()
	{
		mCurvePosX = new AnimationCurve();
		mCurvePosY = new AnimationCurve();
		mCurvePosZ = new AnimationCurve();

		mCurveRotX = new AnimationCurve();
		mCurveRotY = new AnimationCurve();
		mCurveRotZ = new AnimationCurve();

		mCurveScaleX = new AnimationCurve();
		mCurveScaleY = new AnimationCurve();
		mCurveScaleZ = new AnimationCurve();

		keysPosX = new List<Keyframe>();
		keysPosY = new List<Keyframe>();
		keysPosZ = new List<Keyframe>();

		keysRotX = new List<Keyframe>();
		keysRotY = new List<Keyframe>();
		keysRotZ = new List<Keyframe>();

		keysScaleX = new List<Keyframe>();
		keysScaleY = new List<Keyframe>();
		keysScaleZ = new List<Keyframe>();

		lastPosX = float.MinValue;
		lastPosY = float.MinValue;
		lastPosZ = float.MinValue;

		lastRotX = float.MinValue;
		lastRotY = float.MinValue;
		lastRotZ = float.MinValue;

		lastScaleX = float.MinValue;
		lastScaleY = float.MinValue;
		lastScaleZ = float.MinValue;
	}

	void OnGUI ()
	{
		mTransform = (Transform) EditorGUILayout.ObjectField(
			"Transform",
			mTransform,
			typeof(Transform),
			true);


		if (GUILayout.Button("Position"))
			mMode = ViewerMode.Position;
		else if (GUILayout.Button("Rotation"))
			mMode = ViewerMode.Rotation;
		else if (GUILayout.Button("Scale"))
			mMode = ViewerMode.Scale;

		if (mTransform)
		{
			switch(mMode)
			{
			case ViewerMode.Position:
				DrawLocalPosition();
				break;
			case ViewerMode.Rotation:
				DrawLocalRotation();
				break;
			case ViewerMode.Scale:
				DrawLocalScale();
				break;
			}
		}

		if (GUILayout.Button("Clear values"))
		{
			ResetValues();
		}
	}

	private void DrawLocalPosition()
	{
		Vector3 pos = mTransform.localPosition;
		EditorGUILayout.LabelField("X", pos.x.ToString());
		EditorGUILayout.LabelField("Y", pos.y.ToString());
		EditorGUILayout.LabelField("Z", pos.z.ToString());

		EditorGUILayout.CurveField(
			"localPosition.x",
			mCurvePosX,
			Color.yellow,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		EditorGUILayout.CurveField(
			"localPosition.y",
			mCurvePosY,
			Color.green,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		EditorGUILayout.CurveField(
			"localPosition.z",
			mCurvePosZ,
			Color.red,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		if (pos.x != lastPosX)
		{
			keysPosX.Add(new Keyframe(Time.time, pos.x));
			mCurvePosX = new AnimationCurve(keysPosX.ToArray());
			lastPosX = pos.x;
		}

		if (pos.y != lastPosY)
		{
			keysPosY.Add(new Keyframe(Time.time, pos.y));
			mCurvePosY = new AnimationCurve(keysPosY.ToArray());
			lastPosY = pos.y;
		}

		if (pos.z != lastPosZ)
		{
			keysPosZ.Add(new Keyframe(Time.time, pos.z));
			mCurvePosZ = new AnimationCurve(keysPosZ.ToArray());
			lastPosZ = pos.z;
		}
	}

	private void DrawLocalRotation()
	{
		Vector3 rot = mTransform.localRotation.eulerAngles;
		EditorGUILayout.LabelField("X", rot.x.ToString());
		EditorGUILayout.LabelField("Y", rot.y.ToString());
		EditorGUILayout.LabelField("Z", rot.z.ToString());

		EditorGUILayout.CurveField(
			"localRotation.x",
			mCurveRotX,
			Color.yellow,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		EditorGUILayout.CurveField(
			"localRotation.y",
			mCurveRotY,
			Color.green,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		EditorGUILayout.CurveField(
			"localRotation.z",
			mCurveRotZ,
			Color.red,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		if (rot.x != lastRotX)
		{
			keysRotX.Add(new Keyframe(Time.time, rot.x));
			mCurveRotX = new AnimationCurve(keysRotX.ToArray());
			lastRotX = rot.x;
		}

		if (rot.y != lastRotY)
		{
			keysRotY.Add(new Keyframe(Time.time, rot.y));
			mCurveRotY = new AnimationCurve(keysRotY.ToArray());
			lastRotY = rot.y;
		}

		if (rot.z != lastRotZ)
		{
			keysRotZ.Add(new Keyframe(Time.time, rot.z));
			mCurveRotZ = new AnimationCurve(keysRotZ.ToArray());
			lastRotZ = rot.z;
		}
	}

	private void DrawLocalScale()
	{
		Vector3 scale = mTransform.localScale;
		EditorGUILayout.LabelField("X", scale.x.ToString());
		EditorGUILayout.LabelField("Y", scale.y.ToString());
		EditorGUILayout.LabelField("Z", scale.z.ToString());

		EditorGUILayout.CurveField(
			"localScale.x",
			mCurveScaleX,
			Color.yellow,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		EditorGUILayout.CurveField(
			"localScale.y",
			mCurveScaleY,
			Color.green,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		EditorGUILayout.CurveField(
			"localScale.z",
			mCurveScaleZ,
			Color.red,
			Rect.MinMaxRect(0,0,0,0),
			GUILayout.ExpandHeight(true));

		if (scale.x != lastScaleX)
		{
			keysScaleX.Add(new Keyframe(Time.time, scale.x));
			mCurveScaleX = new AnimationCurve(keysScaleX.ToArray());
			lastScaleX = scale.x;
		}

		if (scale.y != lastScaleY)
		{
			keysScaleY.Add(new Keyframe(Time.time, scale.y));
			mCurveScaleY = new AnimationCurve(keysScaleY.ToArray());
			lastScaleY = scale.y;
		}

		if (scale.z != lastScaleZ)
		{
			keysScaleZ.Add(new Keyframe(Time.time, scale.z));
			mCurveScaleZ = new AnimationCurve(keysScaleZ.ToArray());
			lastScaleZ = scale.z;
		}
	}

	public void OnInspectorUpdate()
	{
		// This will only get called 10 times per second.
		Repaint();
	}
}