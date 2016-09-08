using UnityEngine;
using UnityEditor;
using Pathfinding2D;
using System.Collections;
using NavMesh2D.Core;
using System;
using System.Linq;
using Utility.ExtensionMethods;
using System.Collections.Generic;

namespace NavMesh2D.Core
{
    public class NavigationData2dBuildWindow : EditorWindow
    {

        [MenuItem("Pathfinding/NavData2DBuilder")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(NavigationData2dBuildWindow));
        }

        //controll vars
        int selectedTab;
        CollisionGeometryGatheringUI colUI;
        NavAgentUI navAgentUI;

        void OnGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Object", "Nav Agent" });

            switch (selectedTab)
            {
                case 0:
                    colUI.OnGUI();
                    break;
                case 1:
                    navAgentUI.OnGUI();
                    break;
            }
        }

        void OnEnable()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;

            colUI = new CollisionGeometryGatheringUI();
            navAgentUI = new NavAgentUI();
        }

        void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            colUI.OnDestroy();
        }

        void OnSelectionChange()
        {
            colUI.OnSelectionChange(this);
        }

        void OnSceneGUI(SceneView sceneView)
        {
            switch (selectedTab)
            {
                case 0:
                    colUI.OnSceneGUI(sceneView);
                    break;
            }
        }
    }

    class CollisionGeometryGatheringUI
    {
        enum GeometrySelectionType
        {
            BySelection = 0,
            ByLayer = 1,
            AllStatic = 2

        }

        GeometrySelectionType geometrySelectionType;
        CollisionGeometrySet collisionGeometrySet;
        CollisionGeometrySetBuilder builder;
        int fakeLayerMask;

        public CollisionGeometryGatheringUI()
        {
            geometrySelectionType = (GeometrySelectionType)EditorPrefs.GetInt("CollisionGeometryGatheringUI_GeometrySelectionType", 0);
            fakeLayerMask = EditorPrefs.GetInt("CollisionGeometryGatheringUI_fakeLayerMask", 0);
            builder = new CollisionGeometrySetBuilder(EditorPrefs.GetInt("CollisionGeometryGatheringUI_CircleColliderVerts", 4), (int)Mathf.Pow(2, fakeLayerMask));
        }

        public void OnGUI()
        {
            EditorGUIUtility.labelWidth = 200;
            geometrySelectionType = (GeometrySelectionType)EditorGUILayout.EnumPopup("Geometry selection methode", geometrySelectionType);

            switch (geometrySelectionType)
            {
                case GeometrySelectionType.BySelection:
                    EditorGUILayout.HelpBox("Current selection: " + Selection.transforms.Length, MessageType.Info);
                    GUI.enabled = Selection.transforms.Length != 0;
                    break;
                case GeometrySelectionType.ByLayer:
                    fakeLayerMask = EditorGUILayout.LayerField("LayerMask", fakeLayerMask);
                    break;
                case GeometrySelectionType.AllStatic:
                    Collider2D[] sel = ExecuteGatherMethode();
                    EditorGUILayout.HelpBox("Current static: " + sel.Length, MessageType.Info);
                    GUI.enabled = sel.Length != 0;
                    break;
            }

            builder.CircleVertCount = EditorGUILayout.IntSlider("Circle Vert Count", builder.CircleVertCount, 4, 64);

            if (GUILayout.Button("Start gather"))
            {
                Collider2D[] allCollider = ExecuteGatherMethode();

                System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();


                collisionGeometrySet = builder.Build(allCollider);
                SceneView.RepaintAll();
                Debug.Log("Collision-geometry-set builder finished in " + watch.ElapsedMilliseconds / 1000 + "sec. "
                    + allCollider.Length + " -> " + (collisionGeometrySet.colliderVerts.Count + collisionGeometrySet.edgeVerts.Count)
                    + "(Collider = " + collisionGeometrySet.colliderVerts.Count + ", "
                    + "Edges = " + collisionGeometrySet.edgeVerts.Count + ")");
            }
            GUI.enabled = collisionGeometrySet != null;
            if (GUILayout.Button("Clear"))
            {
                collisionGeometrySet = null;
                SceneView.RepaintAll();
            }
            GUI.enabled = true;
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (collisionGeometrySet != null)
            {
                Handles.color = Color.blue;
                Vector3[] dummyArray;
                foreach (Vector2[] vertSet in collisionGeometrySet.colliderVerts)
                {
                    dummyArray = new Vector3[vertSet.Length];
                    for (int iVert = 0; iVert < vertSet.Length; iVert++)
                        dummyArray[iVert] = vertSet[iVert];
                    Handles.DrawAAPolyLine(5f, dummyArray);
                    Handles.DrawAAPolyLine(5f, dummyArray[0], dummyArray[dummyArray.Length - 1]);
                }

            }
        }

        public void OnDestroy()
        {
            EditorPrefs.SetInt("CollisionGeometryGatheringUI_GeometrySelectionType", (int)geometrySelectionType);
            EditorPrefs.SetInt("CollisionGeometryGatheringUI_fakeLayerMask", fakeLayerMask);
            EditorPrefs.SetInt("CollisionGeometryGatheringUI_CircleColliderVerts", builder.CircleVertCount);
        }

        public void OnSelectionChange(EditorWindow window)
        {
            window.Repaint();
        }

        Collider2D[] ExecuteGatherMethode()
        {
            switch (geometrySelectionType)
            {
                case GeometrySelectionType.BySelection:
                    List<Collider2D> bufferedCollider = new List<Collider2D>(Selection.transforms.Length);
                    foreach (Transform trans in Selection.transforms)
                    {
                        Collider2D[] transResult = trans.GetComponents<Collider2D>();
                        if (transResult != null)
                        {
                            foreach (Collider2D col2 in transResult)
                                bufferedCollider.Add(col2);
                        }
                    }
                    builder.WalkableColliderMask = int.MaxValue;
                    return bufferedCollider.ToArray();
                case GeometrySelectionType.ByLayer:
                    builder.WalkableColliderMask = (int)Mathf.Pow(2, fakeLayerMask);
                    return GameObject.FindObjectsOfType<Collider2D>();
                case GeometrySelectionType.AllStatic:
                    builder.WalkableColliderMask = int.MaxValue;
                    Collider2D[] col = GameObject.FindObjectsOfType<Collider2D>();
                    List<Collider2D> bufferedCollider2 = new List<Collider2D>(col.Length);
                    foreach (Collider2D c in col)
                    {
                        if (c.gameObject.isStatic)
                            bufferedCollider2.Add(c);
                    }
                    return bufferedCollider2.ToArray();
            }
            return null;
        }

        class OutlineTreeBuilderUI
        {
            public OutlineTreeBuilderUI()
            {

            }
        }
    }

    class NavAgentUI
    {
        float height;
        float width;
        float maxJumpForce;
        float gravity;
        float maxXVel;
        float cullNodesSmallerThen;
        float slopeLimit;

        public void OnGUI()
        {
            EditorGUILayout.LabelField("NavAgent Settings:", EditorStyles.boldLabel);
            height = Mathf.Max(EditorGUILayout.FloatField("Height", height), 0.01f);
            width = Mathf.Max(EditorGUILayout.FloatField("Width", width), 0.01f);
            maxXVel = Mathf.Max(EditorGUILayout.FloatField("Max X Vel", maxXVel), 0.01f);
            slopeLimit = Mathf.Clamp(EditorGUILayout.FloatField("Slope Limit", slopeLimit), 0, 60);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("NavAgent Settings:", EditorStyles.boldLabel);
        }
    }
}
