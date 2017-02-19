using UnityEngine;
using UnityEditor;
using Pathfinding2D;
using System.Collections;
using NavMesh2D.Core;
using System;
using System.Linq;
using Utility.ExtensionMethods;
using System.Collections.Generic;
using CC2D;
using Entities;

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
        IBuildStepHandler[] buildSteps;
        IBuildStepHandler currentBuildStep { get { return buildSteps[selectedTab]; } }
        IBuildStepHandler oldBuildStep;

        [SerializeField]
        NavAgentUI navAgentUIStep;
        [SerializeField]
        CollisionGeometryGatheringBuildStep collisionGeometryGatheringStep;
        [SerializeField]
        JumpLinkPlacerUI jumpLinkPlacerUIStep;
        [SerializeField]
        BakeNavData bakeNavDataStep;

        [SerializeField]
        public NavAgentGroundWalkerSettings groundWalkerSettings;
        [SerializeField]
        public RawNavigationData2D rawNavData2d;

        [SerializeField]
        public JumpLinkPlacer jumpLinkPlacer;

        void OnGUI()
        {
            EditorGUILayout.LabelField("NavData Builder", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            groundWalkerSettings = (NavAgentGroundWalkerSettings)EditorGUILayout.ObjectField("GroundWalkerSetting", groundWalkerSettings, typeof(NavAgentGroundWalkerSettings), false);
            rawNavData2d = (RawNavigationData2D)EditorGUILayout.ObjectField("NavData2d", rawNavData2d, typeof(RawNavigationData2D), false);
            jumpLinkPlacer = (JumpLinkPlacer)EditorGUILayout.ObjectField("JumpLinkPlacer", jumpLinkPlacer, typeof(JumpLinkPlacer), false);
            if (EditorGUI.EndChangeCheck())
                currentBuildStep.OnEnable();

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            EditorGUI.BeginChangeCheck();
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            if (EditorGUI.EndChangeCheck())
            {
                if (oldBuildStep != null)
                    oldBuildStep.OnDisable();
                oldBuildStep = currentBuildStep;
                currentBuildStep.OnEnable();
            }
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
                if (navAgentUIStep == null)
                    navAgentUIStep = new NavAgentUI(this);
                if (collisionGeometryGatheringStep == null)
                    collisionGeometryGatheringStep = new CollisionGeometryGatheringBuildStep(this);
                if (jumpLinkPlacerUIStep == null)
                    jumpLinkPlacerUIStep = new JumpLinkPlacerUI(this);
                if (bakeNavDataStep == null)
                    bakeNavDataStep = new BakeNavData(this);
                buildSteps = new IBuildStepHandler[] { navAgentUIStep, collisionGeometryGatheringStep, jumpLinkPlacerUIStep, bakeNavDataStep };
                tabNames = new string[buildSteps.Length];
                for (int iStep = 0; iStep < tabNames.Length; iStep++)
                {
                    tabNames[iStep] = buildSteps[iStep].TabName;
                }
            }
            currentBuildStep.OnEnable();
        }

        void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            SceneView.RepaintAll();
            currentBuildStep.OnDisable();
        }

        void OnDisable()
        {
            currentBuildStep.OnDisable();
        }

        void OnSelectionChange()
        {
            currentBuildStep.OnSelectionChanges();
        }

        void OnSceneGUI(SceneView sceneView)
        {
            currentBuildStep.OnSceneGUI(sceneView);
        }

        public void RepaintThisWindow()
        {
            Repaint();
        }

        public void DrawJumpLink(JumpLinkPlacer.JumpLink link, bool showInSceneDetailed, bool isBiDirectional, bool editable)
        {
            Handles.color = Color.white;

            Handles.DrawLine(link.navPointA, link.navPointB);
            Vector2 tangent = (link.navPointB - link.navPointA);
            float dist = tangent.magnitude;
            tangent /= dist;
            Vector2 arrowOrigin = (tangent * (dist / 2)) + link.navPointA;


            if (!isBiDirectional)
            {
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
                arrowOrigin += tangent * 0.2f;
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
                arrowOrigin -= tangent * 0.4f;
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * -tangent * 0.2f) + (Vector3)arrowOrigin);

                arrowOrigin += tangent * 0.2f;
            }
            else
            {
                arrowOrigin += tangent * 0.1f;
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * tangent * 0.2f) + (Vector3)arrowOrigin);
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * tangent * 0.2f) + (Vector3)arrowOrigin);
                arrowOrigin += tangent * 0.2f;
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * tangent * 0.2f) + (Vector3)arrowOrigin);
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * tangent * 0.2f) + (Vector3)arrowOrigin);
                arrowOrigin -= tangent * 0.3f;
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
                arrowOrigin -= tangent * 0.2f;
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
                Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * -tangent * 0.2f) + (Vector3)arrowOrigin);

                arrowOrigin += tangent * 0.2f;
            }



            if (!showInSceneDetailed)
            {
                Handles.DrawWireDisc(link.navPointA, Vector3.forward, 0.1f);
                Handles.DrawWireDisc(link.navPointB, Vector3.forward, 0.1f);

                Handles.color = link.isJumpLinkValid ? Color.green : Color.red;
            }
            else
            {
                Handles.DrawLine(link.navPointA, link.worldPointA);
                Handles.DrawLine(link.navPointB, link.worldPointB);
                if (editable)
                {
                    link.worldPointA = Handles.PositionHandle(link.worldPointA, Quaternion.identity);
                    link.worldPointB = Handles.PositionHandle(link.worldPointB, Quaternion.identity);

                    link.xSpeedScale = Handles.ScaleSlider(link.xSpeedScale, arrowOrigin, Vector3.up, Quaternion.identity, HandleUtility.GetHandleSize(arrowOrigin), 0.01f);
                    if (link.xSpeedScale > 1)
                        link.xSpeedScale = 1;
                    else if (link.xSpeedScale < 0)
                        link.xSpeedScale = 0;
                }

                Vector2 upRight, downRight, upLeft, downLeft;
                float halfWidth = groundWalkerSettings.width / 2;
                upRight = new Vector2(link.navPointA.x - halfWidth, link.navPointA.y + groundWalkerSettings.height);
                upLeft = new Vector2(link.navPointA.x + halfWidth, link.navPointA.y + groundWalkerSettings.height);
                downLeft = new Vector2(link.navPointA.x + halfWidth, link.navPointA.y);
                downRight = new Vector2(link.navPointA.x - halfWidth, link.navPointA.y);
                Handles.DrawLine(upRight, upLeft);
                Handles.DrawLine(upLeft, downLeft);
                Handles.DrawLine(downLeft, downRight);
                Handles.DrawLine(downRight, upRight);

                Vector2 endPointOffset = link.navPointB - link.navPointA;
                Handles.DrawLine(upRight + endPointOffset, upLeft + endPointOffset);
                Handles.DrawLine(upLeft + endPointOffset, downLeft + endPointOffset);
                Handles.DrawLine(downLeft + endPointOffset, downRight + endPointOffset);
                Handles.DrawLine(downRight + endPointOffset, upRight + endPointOffset);

                Handles.DrawWireDisc(link.navPointA, Vector3.forward, 0.05f);
                Handles.DrawWireDisc(link.navPointB, Vector3.forward, 0.05f);

                Handles.DrawWireDisc(downRight, Vector3.forward, 0.1f);
                Handles.DrawWireDisc(downLeft, Vector3.forward, 0.1f);

                Handles.DrawWireDisc(downLeft + endPointOffset, Vector3.forward, 0.1f);
                Handles.DrawWireDisc(downRight + endPointOffset, Vector3.forward, 0.1f);

                Handles.color = link.isJumpLinkValid ? Color.green : Color.red;

                if (link.navPointA.x > link.navPointB.x)
                {
                    upRight.x -= link.navPointA.x;
                    upLeft.x -= link.navPointA.x;
                    downLeft.x -= link.navPointA.x;
                    downRight.x -= link.navPointA.x;
                    DrawJumpArc(link, upRight);
                    DrawJumpArc(link, upLeft);
                    DrawJumpArc(link, downLeft);
                    DrawJumpArc(link, downRight);
                }
                else
                {
                    upRight.x -= link.navPointA.x;
                    upLeft.x -= link.navPointA.x;
                    downLeft.x -= link.navPointA.x;
                    downRight.x -= link.navPointA.x;
                    DrawJumpArc(link, upRight);
                    DrawJumpArc(link, upLeft);
                    DrawJumpArc(link, downLeft);
                    DrawJumpArc(link, downRight);
                }
            }

            Handles.color = Color.white;
        }

        void DrawJumpArc(JumpLinkPlacer.JumpLink link, Vector2 offset)
        {
            Vector2 swapPos;
            Vector2 prevPos = new Vector2(link.jumpArc.startX, link.jumpArc.Calc(0)) + offset;
            float absStepWidth = (link.jumpArc.maxX - link.jumpArc.minX) / 100;
            float stepWidth = absStepWidth * (link.jumpArc.endX < link.jumpArc.startX ? -1f : 1f);

            for (int n = 0; n <= 100; n++)
            {
                swapPos = new Vector2(link.jumpArc.startX + (n * stepWidth), link.jumpArc.Calc(n * absStepWidth)) + offset;
                Handles.DrawLine(prevPos, swapPos);
                prevPos = swapPos;
            }
            //Handles.DrawLine(prevPos, new Vector2(link.jumpArc.endX, link.jumpArc.Calc(link.jumpArc.maxX - link.jumpArc.minX)) + origin);
        }
    }

    [Serializable]
    class CollisionGeometryGatheringBuildStep : IBuildStepHandler
    {
        [SerializeField]
        Collider2D[] inputGeometry;

        CollisionGeometrySet collisionGeometrySet;
        ExpandedTree expandedTree;

        [SerializeField]
        NavigationData2dBuildWindow buildWindow;

        public CollisionGeometryGatheringBuildStep(NavigationData2dBuildWindow buildWindow)
        {
            inputGeometry = new Collider2D[] { };
            collisionGeometrySet = null;
            this.buildWindow = buildWindow;
        }

        public string TabName
        {
            get
            {
                return "Create ExpandedField";
            }
        }

        public void Build()
        {
            GatherCollisionData();
            collisionGeometrySet = CollisionGeometrySetBuilder.Build(inputGeometry, buildWindow.rawNavData2d.circleVertCount);
            ContourTree contourTree = ContourTree.Build(collisionGeometrySet, buildWindow.rawNavData2d.nodeMergeDist, buildWindow.rawNavData2d.maxEdgeDeviation);
            if (contourTree != null)
            {
                expandedTree = ExpandedTree.Build(contourTree, buildWindow.groundWalkerSettings.height);
                if (expandedTree != null)
                {
                    new RawNavigationData2DBuilder(buildWindow.groundWalkerSettings).Build(expandedTree, buildWindow.rawNavData2d);
                }
            }
            EditorUtility.SetDirty(buildWindow.rawNavData2d);
            SceneView.RepaintAll();
        }

        void ToogleHide(bool hide)
        {
            if (inputGeometry != null)
            {
                foreach (Collider2D col in inputGeometry)
                {
                    col.gameObject.SetActive(!hide);
                }
            }
        }

        public void OnGUI()
        {
            if (buildWindow.groundWalkerSettings == null)
            {
                EditorGUILayout.HelpBox("Please assign a Setting.", MessageType.Error);
                return;
            }

            if (buildWindow.rawNavData2d == null)
            {
                EditorGUILayout.HelpBox("Please assign a RawNavData or create one.", MessageType.Error);

                if (GUILayout.Button("Create New"))
                {
                    buildWindow.rawNavData2d = ScriptableObject.CreateInstance<RawNavigationData2D>();
                    buildWindow.rawNavData2d.SaveToAsset();
                }
                return;
            }

            EditorGUI.BeginChangeCheck();
            buildWindow.rawNavData2d.selectAllStatic = EditorGUILayout.Toggle("Select all static", buildWindow.rawNavData2d.selectAllStatic);
            buildWindow.rawNavData2d.selectionMask = CustomEditorFields.LayerMaskField("Selection Mask", buildWindow.rawNavData2d.selectionMask);
            buildWindow.rawNavData2d.useSelection = EditorGUILayout.Toggle("Use Selection", buildWindow.rawNavData2d.useSelection);

            EditorGUILayout.HelpBox("Size of current selection = " + inputGeometry.Length, MessageType.Info);
            buildWindow.rawNavData2d.circleVertCount = EditorGUILayout.IntSlider("Circle Vert Count", buildWindow.rawNavData2d.circleVertCount, 4, 64);

            buildWindow.rawNavData2d.nodeMergeDist = EditorGUILayout.Slider("Merge Distance", buildWindow.rawNavData2d.nodeMergeDist, 0.001f, .5f);
            buildWindow.rawNavData2d.maxEdgeDeviation = EditorGUILayout.Slider("Max Edge Deviation", buildWindow.rawNavData2d.maxEdgeDeviation, 0.0f, 5f);

            EditorGUILayout.Space();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(buildWindow.rawNavData2d);
            }

            EditorGUI.BeginChangeCheck();
            buildWindow.rawNavData2d.hideSelectedGeometry = EditorGUILayout.Toggle("Hide Selected Geometry", buildWindow.rawNavData2d.hideSelectedGeometry);
            if (EditorGUI.EndChangeCheck())
            {
                ToogleHide(buildWindow.rawNavData2d.hideSelectedGeometry);
                EditorUtility.SetDirty(buildWindow.rawNavData2d);
            }

            EditorGUI.BeginChangeCheck();
            buildWindow.rawNavData2d.showType = (RawNavigationData2D.ShowType)EditorGUILayout.EnumPopup("Show Type", buildWindow.rawNavData2d.showType);
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
                EditorUtility.SetDirty(buildWindow.rawNavData2d);
            }


            if (GUILayout.Button("Update Field"))
            {
                Build();
            }

            GUI.enabled = buildWindow.rawNavData2d != null;
            if (GUILayout.Button("Save as New"))
            {
                buildWindow.rawNavData2d.SaveToAsset();
            }
            GUI.enabled = true;
        }

        public void GatherCollisionData()
        {
            if (buildWindow.rawNavData2d.hideSelectedGeometry)
            {
                ToogleHide(false);
            }

            List<Collider2D> bufferedCollider;
            Collider2D[] allCollider = GameObject.FindObjectsOfType<Collider2D>();

            bufferedCollider = new List<Collider2D>(allCollider.Length);

            if (buildWindow.rawNavData2d.useSelection)
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
                if (buildWindow.rawNavData2d.selectionMask.IsLayerWithinMask(col.gameObject.layer))
                {
                    if (!buildWindow.rawNavData2d.useSelection || !bufferedCollider.Contains(col))
                        bufferedCollider.Add(col);
                }
                else if (buildWindow.rawNavData2d.selectAllStatic && col.gameObject.isStatic)
                {
                    if (!buildWindow.rawNavData2d.useSelection || !bufferedCollider.Contains(col))
                        bufferedCollider.Add(col);
                }

            }
            inputGeometry = bufferedCollider.ToArray();

            if (buildWindow.rawNavData2d.hideSelectedGeometry)
            {
                ToogleHide(true);
            }
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (buildWindow.rawNavData2d == null)
                return;
            if (buildWindow.rawNavData2d.showType == RawNavigationData2D.ShowType.Raw)
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
            else if (buildWindow.rawNavData2d.showType == RawNavigationData2D.ShowType.MergedAndExpanded)
            {
                if (expandedTree != null)
                {
                    foreach (ExpandedNode child in expandedTree.headNode.children)
                        DrawContourNode(child);
                }
            }
            else if (buildWindow.rawNavData2d.showType == RawNavigationData2D.ShowType.RawAndFilled)
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
            else if (buildWindow.rawNavData2d.showType == RawNavigationData2D.ShowType.NavData)
            {
                if (buildWindow.rawNavData2d != null)
                {
                    NavData2DVisualizerWindow.SceneDrawNavData2D(buildWindow.rawNavData2d);
                }
            }
        }

        void DrawContourNode(ExpandedNode node)
        {
            Vector3[] dummyArray = new Vector3[node.contour.pointNodeCount];
            int counter = 0;
            Handles.color = Color.black;
            foreach (PointNode pn in node.contour)
            {
                dummyArray[counter] = pn.pointB;
                Handles.DrawWireDisc(pn.pointB, Vector3.forward, 0.1f);
                counter++;
            }
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(4f, dummyArray);
            Handles.DrawAAPolyLine(4f, dummyArray[0], dummyArray[dummyArray.Length - 1]);

            PointNode.ObstructedSegment obstruction;
            Handles.color = Color.red;
            foreach (PointNode pn in node.contour)
            {
                obstruction = pn.FirstObstructedSegment;

                while (obstruction != null)
                {
                    Handles.DrawAAPolyLine(4, pn.tangentBC * obstruction.start + pn.pointB, pn.tangentBC * obstruction.end + pn.pointB);
                    obstruction = obstruction.next;
                }
            }

            foreach (ExpandedNode child in node.children)
                DrawContourNode(child);
        }

        public void OnSelectionChanges()
        {
        }

        public void OnDisable()
        {
            if (buildWindow.rawNavData2d == null)
                return;
            ToogleHide(false);
            inputGeometry = new Collider2D[] { };
            AssetDatabase.SaveAssets();
        }

        public void OnEnable()
        {
            if (buildWindow.rawNavData2d == null)
                return;
            ToogleHide(buildWindow.rawNavData2d.hideSelectedGeometry);
            SceneView.RepaintAll();
        }
    }

    [Serializable]
    class NavAgentUI : IBuildStepHandler
    {
        [SerializeField]
        NavigationData2dBuildWindow buildWindow;
        IMovementEntity iMovementActor;

        public NavAgentUI(NavigationData2dBuildWindow buildWindow)
        {
            this.buildWindow = buildWindow;
        }

        public string TabName
        {
            get
            {
                return "NavAgent";
            }
        }

        public void OnGUI()
        {
            if (buildWindow.groundWalkerSettings != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("NavAgent Settings:", EditorStyles.boldLabel);
                buildWindow.groundWalkerSettings.height = Mathf.Max(EditorGUILayout.FloatField("Height", buildWindow.groundWalkerSettings.height), 0.01f);
                buildWindow.groundWalkerSettings.width = Mathf.Max(EditorGUILayout.FloatField("Width", buildWindow.groundWalkerSettings.width), 0.01f);
                buildWindow.groundWalkerSettings.maxXVel = Mathf.Max(EditorGUILayout.FloatField("Max X Vel", buildWindow.groundWalkerSettings.maxXVel), 0.01f);
                buildWindow.groundWalkerSettings.slopeLimit = Mathf.Clamp(EditorGUILayout.FloatField("Slope Limit", buildWindow.groundWalkerSettings.slopeLimit), 0, 60);
                EditorGUILayout.Space();
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(buildWindow.groundWalkerSettings);
            }

            GUI.enabled = iMovementActor != null && buildWindow.groundWalkerSettings != null;
            if (GUILayout.Button("Copy from selected MovingActor"))
            {
                CopyValuesFromCC2DMotor();
                EditorUtility.SetDirty(buildWindow.groundWalkerSettings);
            }
            GUI.enabled = true;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save As New"))
            {
                Save();
            }
            if (GUILayout.Button("Create New"))
            {
                CreateNew();
            }
            GUILayout.EndHorizontal();
        }

        public void OnSceneGUI(SceneView sceneView)
        {

        }

        public void OnSelectionChanges()
        {
            if (Selection.activeGameObject != null)
            {
                iMovementActor = Selection.activeGameObject.GetComponent<IMovementEntity>();
                buildWindow.RepaintThisWindow();
            }
        }

        public void OnDisable()
        {
            AssetDatabase.SaveAssets();
        }

        public void OnEnable()
        {
            if (Selection.activeGameObject != null)
            {
                iMovementActor = Selection.activeGameObject.GetComponent<HumanMovementEntity>();
                buildWindow.RepaintThisWindow();
            }
            SceneView.RepaintAll();
        }

        void CopyValuesFromCC2DMotor()
        {
            Renderer[] renderer = iMovementActor.GetComponentsInChildren<Renderer>();
            float minHeight = renderer[0].bounds.min.y;
            float maxHeight = renderer[0].bounds.max.y;
            float minWidth = renderer[0].bounds.min.x;
            float maxWidth = renderer[0].bounds.max.x;
            foreach (Renderer r in renderer)
            {
                maxHeight = Mathf.Max(maxHeight, r.bounds.max.y);
                minHeight = Mathf.Min(minHeight, r.bounds.min.y);
                maxWidth = Mathf.Max(maxWidth, r.bounds.max.x);
                minWidth = Mathf.Min(minWidth, r.bounds.min.x);
            }
            buildWindow.groundWalkerSettings.height = Mathf.Max(0.01f, maxHeight - minHeight);
            buildWindow.groundWalkerSettings.width = Mathf.Max(0.01f, maxWidth - minWidth);

            SerializedObject spMotor = new SerializedObject(iMovementActor.CC2DMotor);
            buildWindow.groundWalkerSettings.maxXVel = spMotor.FindProperty("walkHMaxSpeed").floatValue;
            buildWindow.groundWalkerSettings.gravity = spMotor.FindProperty("gravityAcceleration").floatValue;
            buildWindow.groundWalkerSettings.jumpForce = Mathf.Sqrt(spMotor.FindProperty("jumpMaxHeight").floatValue * buildWindow.groundWalkerSettings.gravity * 2);

            SerializedObject spCC2D = new SerializedObject(iMovementActor.CharacterController2D);
            buildWindow.groundWalkerSettings.slopeLimit = spCC2D.FindProperty("slopeLimit").floatValue;

            buildWindow.RepaintThisWindow();
        }

        void Save()
        {
            string path = EditorUtility.SaveFilePanel("Save NavAgentSettings", "Assets", "GroundWalkerSettings", "asset");
            if (path == null || path.Length == 0)
                return;
            path = path.Substring(path.IndexOf("Assets"));
            Debug.Log(path);
            AssetDatabase.CreateAsset(buildWindow.groundWalkerSettings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = buildWindow.groundWalkerSettings;
        }

        void CreateNew()
        {
            buildWindow.groundWalkerSettings = ScriptableObject.CreateInstance<NavAgentGroundWalkerSettings>();

        }
    }

    [Serializable]
    class JumpLinkPlacerUI : IBuildStepHandler
    {
        [SerializeField]
        NavigationData2dBuildWindow buildWindow;
        Vector2 scrollPos;
        [SerializeField]
        bool showDetailsGlobal;
        [SerializeField]
        bool showInSceneGlobal;
        [SerializeField]
        bool showInSceneDetailedGlobal;

        public JumpLinkPlacerUI(NavigationData2dBuildWindow buildWindow)
        {
            this.buildWindow = buildWindow;
            OnEnable();
        }

        public string TabName
        {
            get
            {
                return "Jump Link Placer";
            }
        }

        public void OnDisable()
        {
            AssetDatabase.SaveAssets();
        }

        public void OnEnable()
        {
            if (buildWindow.jumpLinkPlacer == null)
                return;

            SceneView.RepaintAll();
        }

        public void OnGUI()
        {
            if (buildWindow.rawNavData2d == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a NavData2d", MessageType.Error);
            }
            else if (buildWindow.jumpLinkPlacer == null)
            {
                EditorGUILayout.HelpBox("Please assign a JumpLinkPlacer or create one.", MessageType.Error);

                if (GUILayout.Button("Create New"))
                {
                    buildWindow.jumpLinkPlacer = ScriptableObject.CreateInstance<JumpLinkPlacer>();
                    buildWindow.jumpLinkPlacer.Init();
                    buildWindow.jumpLinkPlacer.SaveToAsset();
                    OnEnable();
                }
            }
            else
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                showDetailsGlobal = EditorGUILayout.ToggleLeft("All Details", showDetailsGlobal, GUILayout.Width(75));
                if (EditorGUI.EndChangeCheck())
                {
                    ToogleShowDetailAll(showDetailsGlobal);
                    EditorUtility.SetDirty(buildWindow.jumpLinkPlacer);
                }

                EditorGUI.BeginChangeCheck();
                showInSceneGlobal = EditorGUILayout.ToggleLeft("All Visibility", showInSceneGlobal, GUILayout.Width(80));
                if (EditorGUI.EndChangeCheck())
                {
                    ToogleShowInScene(showInSceneGlobal);
                    EditorUtility.SetDirty(buildWindow.jumpLinkPlacer);
                }

                EditorGUI.BeginChangeCheck();
                showInSceneDetailedGlobal = EditorGUILayout.ToggleLeft("All Scene Details", showInSceneDetailedGlobal, GUILayout.Width(120));
                if (EditorGUI.EndChangeCheck())
                {
                    ToogleShowInSceneDetailed(showInSceneDetailedGlobal);
                    EditorUtility.SetDirty(buildWindow.jumpLinkPlacer);
                }

                if (GUILayout.Button("Select Invalid Links", GUILayout.Width(120)))
                {
                    ShowInvalidLinks();
                }

                EditorGUILayout.EndHorizontal();

                JumpLinkPlacer.JumpLink link;
                for (int iLink = 0; iLink < buildWindow.jumpLinkPlacer.jumpLinks.Count; iLink++)
                {
                    EditorGUI.BeginChangeCheck();

                    link = buildWindow.jumpLinkPlacer.jumpLinks[iLink];
                    EditorGUILayout.BeginHorizontal();
                    link.jumpLinkSettings.showDetails = EditorGUILayout.Foldout(link.jumpLinkSettings.showDetails, "Link");
                    link.isBiDirectional = EditorGUILayout.ToggleLeft(link.isBiDirectional ? "<->" : "->", link.isBiDirectional, GUILayout.Width(40));
                    link.jumpLinkSettings.showInScene = EditorGUILayout.ToggleLeft(link.jumpLinkSettings.showInScene ? "[o]" : "[-]", link.jumpLinkSettings.showInScene, GUILayout.Width(40));
                    link.jumpLinkSettings.showInSceneDetailed = EditorGUILayout.ToggleLeft("Details", link.jumpLinkSettings.showInSceneDetailed, GUILayout.Width(55));
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        buildWindow.jumpLinkPlacer.jumpLinks.RemoveAt(iLink);
                        buildWindow.RepaintThisWindow();
                        SceneView.RepaintAll();
                        iLink--;
                        EditorUtility.SetDirty(buildWindow.jumpLinkPlacer);
                        AssetDatabase.SaveAssets();
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (link.jumpLinkSettings.showDetails)
                    {
                        link.worldPointA = EditorGUILayout.Vector2Field("WorldPointA", link.worldPointA);
                        link.worldPointB = EditorGUILayout.Vector2Field("WorldPointB", link.worldPointB);
                        link.xSpeedScale = EditorGUILayout.Slider("Speed Percentage", link.xSpeedScale, 0, 1);
                        GUI.enabled = false;

                        EditorGUILayout.Vector2Field("NavPointA", link.navPointA);
                        EditorGUILayout.Vector2Field("NavPointB", link.navPointB);
                        EditorGUILayout.IntField("NodeIndexA", link.nodeIndexA);
                        EditorGUILayout.IntField("NodeIndexB", link.nodeIndexB);

                        GUI.enabled = true;
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        link.TryRemapPoints(buildWindow.rawNavData2d);
                        link.RecalculateJumpArc(buildWindow.groundWalkerSettings);
                        EditorUtility.SetDirty(buildWindow.jumpLinkPlacer);
                        SceneView.RepaintAll();
                    }
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Link"))
                {
                    ToogleShowInSceneDetailed(false);
                    Camera[] sceneViewCams = SceneView.GetAllSceneCameras();
                    if (sceneViewCams.Length > 0)
                    {
                        Vector2 screenCenter = sceneViewCams[0].ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                        buildWindow.jumpLinkPlacer.jumpLinks.Add(new JumpLinkPlacer.JumpLink(screenCenter, screenCenter + Vector2.right * 3));
                    }
                    else
                    {
                        buildWindow.jumpLinkPlacer.jumpLinks.Add(new JumpLinkPlacer.JumpLink());
                    }
                    buildWindow.jumpLinkPlacer.jumpLinks[buildWindow.jumpLinkPlacer.jumpLinks.Count - 1].TryRemapPoints(buildWindow.rawNavData2d);
                    buildWindow.jumpLinkPlacer.jumpLinks[buildWindow.jumpLinkPlacer.jumpLinks.Count - 1].RecalculateJumpArc(buildWindow.groundWalkerSettings);
                    buildWindow.RepaintThisWindow();
                    EditorUtility.SetDirty(buildWindow.jumpLinkPlacer);
                    SceneView.RepaintAll();
                }
                GUI.enabled = buildWindow.jumpLinkPlacer.jumpLinks.Count > 0;
                if (GUILayout.Button("Save as New"))
                {
                    buildWindow.jumpLinkPlacer.SaveToAsset();
                }
            }
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (buildWindow.jumpLinkPlacer == null)
                return;

            NavData2DVisualizerWindow.SceneDrawNavData2D(buildWindow.rawNavData2d);
            JumpLinkPlacer.JumpLink link;
            for (int iLink = 0; iLink < buildWindow.jumpLinkPlacer.jumpLinks.Count; iLink++)
            {
                if (!buildWindow.jumpLinkPlacer.jumpLinks[iLink].jumpLinkSettings.showInScene)
                    continue;


                link = buildWindow.jumpLinkPlacer.jumpLinks[iLink];
                EditorGUI.BeginChangeCheck();

                buildWindow.DrawJumpLink(link, link.jumpLinkSettings.showInSceneDetailed, link.isBiDirectional, true);

                if (EditorGUI.EndChangeCheck())
                {
                    link.TryRemapPoints(buildWindow.rawNavData2d);
                    link.RecalculateJumpArc(buildWindow.groundWalkerSettings);
                    EditorUtility.SetDirty(buildWindow.jumpLinkPlacer);
                    buildWindow.RepaintThisWindow();
                }
            }
        }

        public void OnSelectionChanges()
        {

        }

        void ToogleShowDetailAll(bool showDetail)
        {
            foreach (JumpLinkPlacer.JumpLink jl in buildWindow.jumpLinkPlacer.jumpLinks)
            {
                jl.jumpLinkSettings.showDetails = showDetail;
            }
        }

        void ToogleShowInScene(bool showInScene)
        {
            foreach (JumpLinkPlacer.JumpLink jl in buildWindow.jumpLinkPlacer.jumpLinks)
            {
                jl.jumpLinkSettings.showInScene = showInScene;
            }
            SceneView.RepaintAll();
        }

        void ToogleShowInSceneDetailed(bool showInSceneDetailed)
        {
            foreach (JumpLinkPlacer.JumpLink jl in buildWindow.jumpLinkPlacer.jumpLinks)
            {
                jl.jumpLinkSettings.showInSceneDetailed = showInSceneDetailed;
            }
            SceneView.RepaintAll();
        }

        void ShowInvalidLinks()
        {
            foreach (JumpLinkPlacer.JumpLink jl in buildWindow.jumpLinkPlacer.jumpLinks)
            {
                JumpLinkPlacer.JumpLink inverseLink = jl.InvertLink(buildWindow.groundWalkerSettings);
                if (jl.jumpArc.j <= 0 || inverseLink.jumpArc.j <= 0)
                    jl.jumpLinkSettings.showInSceneDetailed = true;
                else
                    jl.jumpLinkSettings.showInSceneDetailed = false;
            }
        }
    }

    [Serializable]
    class BakeNavData : IBuildStepHandler
    {
        [SerializeField]
        NavigationData2dBuildWindow buildWindow;
        [SerializeField]
        NavigationData2D existingField;

        public BakeNavData(NavigationData2dBuildWindow buildWindow)
        {
            this.buildWindow = buildWindow;
            OnEnable();
        }

        public string TabName
        {
            get
            {
                return "Bake";
            }
        }

        public void OnDisable()
        {

        }

        public void OnEnable()
        {
            SceneView.RepaintAll();
        }

        public void OnGUI()
        {
            bool allDataAviable = true;
            if (buildWindow.groundWalkerSettings == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a NavSetting", MessageType.Error);
                allDataAviable = false;
            }
            if (buildWindow.rawNavData2d == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a NavData2d", MessageType.Error);
                allDataAviable = false;
            }
            if (buildWindow.jumpLinkPlacer == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a JumpLinkPlacer", MessageType.Error);
                allDataAviable = false;
            }

            if (allDataAviable)
            {
                EditorGUILayout.LabelField("Manual Jumplinks:", buildWindow.jumpLinkPlacer.jumpLinks.Count.ToString());
                existingField = (NavigationData2D)EditorGUILayout.ObjectField("OldField", existingField, typeof(NavigationData2D), false);

                if (GUILayout.Button("Bake As New"))
                {
                    NavigationData2D navData = buildWindow.rawNavData2d.ToNavigationData2D();
                    Bake(navData, buildWindow.rawNavData2d);
                    navData.SaveToAsset();
                }

                GUI.enabled = existingField != null;
                if (GUILayout.Button("Update Existing File"))
                {
                    NavigationData2D navData = buildWindow.rawNavData2d.ToNavigationData2D();
                    Bake(navData, buildWindow.rawNavData2d);
                    existingField.nodes = navData.nodes;
                    existingField.name = navData.name;
                    existingField.navAgentSettings = navData.navAgentSettings;
                    existingField.version = navData.version;
                    EditorUtility.SetDirty(existingField);
                    AssetDatabase.SaveAssets();
                }
                GUI.enabled = true;
            }
        }

        void Bake(NavigationData2D navData, RawNavigationData2D rawNavData)
        {
            Dictionary<NavNode, List<JumpLinkPlacer.JumpLink>> linkTable = new Dictionary<NavNode, List<JumpLinkPlacer.JumpLink>>(navData.nodes.Length);
            List<JumpLinkPlacer.JumpLink> cList;
            foreach (JumpLinkPlacer.JumpLink link in buildWindow.jumpLinkPlacer.jumpLinks)
            {
                if (!link.isJumpLinkValid || !link.TryRemapPoints(rawNavData))
                    continue;

                Debug.Assert(link.jumpArc.j > 0);
                if (link.isBiDirectional)
                {
                    JumpLinkPlacer.JumpLink inverseLink = link.InvertLink(buildWindow.groundWalkerSettings);
                    if (!inverseLink.TryRemapPoints(rawNavData))
                    {
                        Debug.Assert(false); // Should nevewr happen!
                    }
                    Debug.Assert(inverseLink.jumpArc.j > 0);
                    if (linkTable.TryGetValue(navData.nodes[inverseLink.nodeIndexA], out cList))
                    {
                        cList.Add(inverseLink);
                    }
                    else
                    {
                        cList = new List<JumpLinkPlacer.JumpLink>(2);
                        cList.Add(inverseLink);
                        linkTable.Add(navData.nodes[inverseLink.nodeIndexA], cList);
                    }
                }

                if (linkTable.TryGetValue(navData.nodes[link.nodeIndexA], out cList))
                {
                    cList.Add(link);
                }
                else
                {
                    cList = new List<JumpLinkPlacer.JumpLink>(2);
                    cList.Add(link);
                    linkTable.Add(navData.nodes[link.nodeIndexA], cList);
                }
            }
            Dictionary<int, List<int>> vertLinkTable;
            List<int> cLinkList;
            JumpLinkPlacer.JumpLink cLink;
            foreach (KeyValuePair<NavNode, List<JumpLinkPlacer.JumpLink>> keyPair in linkTable)
            {
                IOffNodeLink[] allLinks = new IOffNodeLink[keyPair.Value.Count];
                vertLinkTable = new Dictionary<int, List<int>>(keyPair.Value.Count);
                for (int iLink = 0; iLink < keyPair.Value.Count; iLink++)
                {
                    cLink = keyPair.Value[iLink];
                    allLinks[iLink] = new JumpLink(cLink);
                    if (vertLinkTable.TryGetValue(cLink.nodeVertIndexA, out cLinkList))
                    {
                        cLinkList.Add(iLink);
                    }
                    else
                    {
                        cLinkList = new List<int>(2);
                        cLinkList.Add(iLink);
                        vertLinkTable.Add(cLink.nodeVertIndexA, cLinkList);
                    }
                }
                keyPair.Key.links = allLinks;

                foreach (KeyValuePair<int, List<int>> vertLink in vertLinkTable)
                {
                    keyPair.Key.verts[vertLink.Key].linkIndex = vertLink.Value.ToArray();
                }
            }
            navData.navAgentSettings = buildWindow.groundWalkerSettings;
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (buildWindow.groundWalkerSettings == null || buildWindow.rawNavData2d == null || buildWindow.jumpLinkPlacer == null)
            {
                return;
            }
            NavData2DVisualizerWindow.SceneDrawNavData2D(buildWindow.rawNavData2d);
            for (int iLink = 0; iLink < buildWindow.jumpLinkPlacer.jumpLinks.Count; iLink++)
            {
                buildWindow.DrawJumpLink(buildWindow.jumpLinkPlacer.jumpLinks[iLink], false, buildWindow.jumpLinkPlacer.jumpLinks[iLink].isBiDirectional, false);
            }
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
        void OnDisable();
        void OnEnable();
    }
}
