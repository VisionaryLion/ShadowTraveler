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
        [SerializeField]
        int selectedTab;
        [SerializeField]
        string[] tabNames;
        [SerializeField]
        IBuildStepHandler[] buildSteps;
        IBuildStepHandler currentBuildStep { get { return buildSteps[selectedTab]; } }

        void OnGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            currentBuildStep.OnGUI();
        }

        void OnEnable()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;

            if (buildSteps == null)
            {
                buildSteps = new IBuildStepHandler[] { new CollisionGeometryGatheringBuildStep(), new NavAgentUI() };
                tabNames = new string[buildSteps.Length];
                for (int iStep = 0; iStep < tabNames.Length; iStep++)
                {
                    tabNames[iStep] = buildSteps[iStep].TabName;
                }
            }
        }

        void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            SceneView.RepaintAll();
        }

        void OnSelectionChange()
        {
            currentBuildStep.OnSelectionChanges();
        }

        void OnSceneGUI(SceneView sceneView)
        {
            currentBuildStep.OnSceneGUI(sceneView);
        }
    }

    class CollisionGeometryGatheringBuildStep : IBuildStepHandler
    {
        enum ShowType
        {
            Nothing,
            Merged,
            Raw,
            RawAndFilled
        }

        [SerializeField]
        Collider2D[] inputGeometry;
        [SerializeField]
        LayerMask selectionMask;
        [SerializeField]
        int circleVertCount;
        [SerializeField]
        bool selectAllStatic;
        [SerializeField]
        bool useSelection;

        [SerializeField]
        public CollisionGeometrySet collisionGeometrySet;
        [SerializeField]
        public ContourTree contourTree;

        //Debug stuff
        [SerializeField]
        ShowType showType;

        public CollisionGeometryGatheringBuildStep()
        {
            inputGeometry = new Collider2D[] { };
            selectAllStatic = true;
            selectionMask = 0;
            circleVertCount = 10;
            showType = ShowType.Merged;
            Build();
        }

        public string TabName
        {
            get
            {
                return "Collision Geometry Gathering";
            }
        }

        public void Build()
        {
            GatherCollisionData();
            if (inputGeometry.Length > 0)
            {
                collisionGeometrySet = CollisionGeometrySetBuilder.Build(inputGeometry, circleVertCount);
                contourTree = ContourTree.Build(collisionGeometrySet);
                SceneView.RepaintAll();
            }
        }

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            selectAllStatic = EditorGUILayout.Toggle("Select all static", selectAllStatic);
            selectionMask = CustomEditorFields.LayerMaskField("Selection Mask", selectionMask);
            useSelection = EditorGUILayout.Toggle("Use Selection", useSelection);

            EditorGUILayout.HelpBox("Size of current selection = " + inputGeometry.Length, MessageType.Info);
            circleVertCount = EditorGUILayout.IntSlider("Circle Vert Count", circleVertCount, 4, 64);

            if (EditorGUI.EndChangeCheck() || GUILayout.Button("Update"))
                Build();

            EditorGUI.BeginChangeCheck();
            showType = (ShowType)EditorGUILayout.EnumPopup("Show Type", showType);
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();

            GUI.enabled = contourTree != null;
            if (GUILayout.Button("Save"))
            {
                SaveContourTree();
            }
        }

        void SaveContourTree()
        {
            string path = EditorUtility.SaveFilePanel("Save ContourTree", "Assets", "ContourTree", "asset");
            if (path == null || path.Length == 0)
                return;
            path = path.Substring(path.IndexOf("Assets"));
            Debug.Log(path);
            AssetDatabase.CreateAsset(contourTree, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = contourTree;
        }

        public void GatherCollisionData()
        {
            List<Collider2D> bufferedCollider;
            Collider2D[] allCollider = GameObject.FindObjectsOfType<Collider2D>();

            bufferedCollider = new List<Collider2D>(allCollider.Length);

            if (useSelection)
            {
                foreach (Transform selectedTransforms in Selection.transforms)
                {
                    Collider2D[] childCollider = selectedTransforms.GetComponentsInChildren<Collider2D>();
                    if (childCollider != null)
                        bufferedCollider.AddRange(childCollider);
                }
            }

            foreach (Collider2D col in allCollider)
            {
                if (selectionMask.IsLayerWithinMask(col.gameObject.layer))
                {
                    if (!useSelection || !bufferedCollider.Contains(col))
                        bufferedCollider.Add(col);
                }
                else if (selectAllStatic && col.gameObject.isStatic)
                {
                    if (!useSelection || !bufferedCollider.Contains(col))
                        bufferedCollider.Add(col);
                }

            }
            inputGeometry = bufferedCollider.ToArray();
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (showType == ShowType.Raw)
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
            else if (showType == ShowType.Merged)
            {
                if (contourTree != null)
                {
                    int colorIndex = 0;
                    foreach (ContourNode child in contourTree.FirstNode.children)
                        DrawContourNode(child, colorIndex++);
                }
            }
            else if (showType == ShowType.RawAndFilled)
            {
                if (collisionGeometrySet != null)
                {
                    Handles.color = Color.blue;
                    Vector3[] dummyArray;
                    foreach (Vector2[] vertSet in collisionGeometrySet.colliderVerts)
                    {
                        dummyArray = new Vector3[vertSet.Length + 1];
                        for (int iVert = 0; iVert < vertSet.Length; iVert++)
                            dummyArray[iVert] = vertSet[iVert];
                        dummyArray[dummyArray.Length - 1] = dummyArray[0];
                        Handles.DrawAAConvexPolygon(dummyArray);
                    }
                }
            }
        }

        void DrawContourNode(ContourNode node, int colorIndex)
        {
            Handles.color = Utility.DifferentColors.GetColor(colorIndex);
            Vector3[] dummyArray = new Vector3[node.contour.verticies.Count];
            for (int iVert = 0; iVert < dummyArray.Length; iVert++)
                dummyArray[iVert] = node.contour.verticies[iVert];
            Handles.DrawPolyLine(dummyArray);
            Handles.DrawPolyLine(dummyArray[0], dummyArray[dummyArray.Length - 1]);

            foreach (ContourNode child in node.children)
                DrawContourNode(child, colorIndex);
        }

        public void OnSelectionChanges()
        {
        }
    }

    class NavAgentUI : IBuildStepHandler
    {
        float height;
        float width;
        float maxJumpForce;
        float gravity;
        float maxXVel;
        float cullNodesSmallerThen;
        float slopeLimit;

        public string TabName
        {
            get
            {
                return "NavAgent";
            }
        }

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

        public void OnSceneGUI(SceneView sceneView)
        {

        }

        public void OnSelectionChanges()
        {

        }
    }

    interface IBuildStepHandler
    {
        string TabName { get; }
        void OnGUI();
        void OnSceneGUI(SceneView sceneView);
        void OnSelectionChanges();
    }
}
