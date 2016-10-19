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
using Actors;

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
        public NavigationData2D navData2d;

        [SerializeField]
        public JumpLinkPlacer jumpLinkPlacer;

        void OnGUI()
        {
            EditorGUILayout.LabelField("NavData Builder", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            groundWalkerSettings = (NavAgentGroundWalkerSettings)EditorGUILayout.ObjectField("GroundWalkerSetting", groundWalkerSettings, typeof(NavAgentGroundWalkerSettings), false);
            navData2d = (NavigationData2D)EditorGUILayout.ObjectField("NavData2d", navData2d, typeof(NavigationData2D), false);
            jumpLinkPlacer = (JumpLinkPlacer)EditorGUILayout.ObjectField("JumpLinkPlacer", jumpLinkPlacer, typeof(JumpLinkPlacer), false);

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

        public void DrawJumpLink(JumpLinkPlacer.JumpLink link, bool showInSceneDetailed, bool editable)
        {
            Handles.color = Color.white;

            Handles.DrawLine(link.navPointA, link.navPointB);
            Vector2 tangent = (link.navPointB - link.navPointA);
            float dist = tangent.magnitude;
            tangent /= dist;
            Vector2 arrowOrigin = (tangent * (dist / 2)) + link.navPointA;


            Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
            Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
            arrowOrigin += tangent * 0.2f;
            Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
            Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
            arrowOrigin -= tangent * 0.4f;
            Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, 30) * -tangent * 0.2f) + (Vector3)arrowOrigin);
            Handles.DrawLine(arrowOrigin, (Quaternion.Euler(0, 0, -30) * -tangent * 0.2f) + (Vector3)arrowOrigin);


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

                    link.xSpeedScale = Handles.ScaleSlider( link.xSpeedScale, arrowOrigin, Vector3.up, Quaternion.identity, HandleUtility.GetHandleSize(arrowOrigin), 0.01f);
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

        void DrawJumpArc(JumpLinkPlacer.JumpLink link, Vector2 origin)
        {
            Vector2 swapPos;
            Vector2 prevPos = new Vector2(link.jumpArc.startX, link.jumpArc.Calc(0)) + origin;
            int steps = Mathf.FloorToInt((link.jumpArc.maxX - link.jumpArc.minX) / 0.1f);
            float dir = link.jumpArc.endX < link.jumpArc.startX ? -0.1f : 0.1f;
            float absDir = Mathf.Abs(dir);
            for (float n = 0; n < steps; n++)
            {
                swapPos = new Vector2(link.jumpArc.startX + n * dir, link.jumpArc.Calc(n * absDir)) + origin;
                Handles.DrawLine(prevPos, swapPos);
                prevPos = swapPos;
            }
            Handles.DrawLine(prevPos, new Vector2(link.jumpArc.endX, link.jumpArc.Calc(link.jumpArc.maxX - link.jumpArc.minX)) + origin);
        }

        public void SaveNavData(NavigationData2D navData)
        {
            string path = EditorUtility.SaveFilePanel("Save NavData2D", "Assets", "NavData2D", "asset");
            if (path == null || path.Length == 0)
                return;
            path = path.Substring(path.IndexOf("Assets"));
            Debug.Log(path);
            AssetDatabase.CreateAsset(navData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = navData;
        }
    }

    [Serializable]
    class CollisionGeometryGatheringBuildStep : IBuildStepHandler
    {
        enum ShowType
        {
            NavData,
            MergedAndExpanded,
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
        bool hideSelectedGeometry = false;
        [SerializeField]
        float nodeMergeDist;
        [SerializeField]
        float maxEdgeDeviation;


        CollisionGeometrySet collisionGeometrySet;
        ExpandedTree expandedTree;

        [SerializeField]
        NavigationData2dBuildWindow buildWindow;

        //Debug stuff
        [SerializeField]
        ShowType showType;

        public CollisionGeometryGatheringBuildStep(NavigationData2dBuildWindow buildWindow)
        {
            inputGeometry = new Collider2D[] { };
            selectAllStatic = true;
            selectionMask = 0;
            circleVertCount = 10;
            showType = ShowType.MergedAndExpanded;
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
            collisionGeometrySet = CollisionGeometrySetBuilder.Build(inputGeometry, circleVertCount);
            ContourTree contourTree = ContourTree.Build(collisionGeometrySet, nodeMergeDist, maxEdgeDeviation);
            if (contourTree != null)
            {
                expandedTree = ExpandedTree.Build(contourTree, buildWindow.groundWalkerSettings.height);
                if (expandedTree != null)
                {
                    buildWindow.navData2d = new NavigationData2DBuilder(buildWindow.groundWalkerSettings).Build(expandedTree);
                }
            }
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

            EditorGUI.BeginChangeCheck();
            selectAllStatic = EditorGUILayout.Toggle("Select all static", selectAllStatic);
            selectionMask = CustomEditorFields.LayerMaskField("Selection Mask", selectionMask);
            useSelection = EditorGUILayout.Toggle("Use Selection", useSelection);

            EditorGUILayout.HelpBox("Size of current selection = " + inputGeometry.Length, MessageType.Info);
            circleVertCount = EditorGUILayout.IntSlider("Circle Vert Count", circleVertCount, 4, 64);

            nodeMergeDist = EditorGUILayout.Slider("Merge Distance", nodeMergeDist, 0.001f, .5f);
            maxEdgeDeviation = EditorGUILayout.Slider("Max Edge Deviation", maxEdgeDeviation, 0.0f, 5f);

            bool change = EditorGUI.EndChangeCheck();
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            hideSelectedGeometry = EditorGUILayout.Toggle("Hide Selected Geometry", hideSelectedGeometry);
            if (EditorGUI.EndChangeCheck())
            {
                ToogleHide(hideSelectedGeometry);
            }

            EditorGUI.BeginChangeCheck();
            showType = (ShowType)EditorGUILayout.EnumPopup("Show Type", showType);
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();


            if ((change || GUILayout.Button("Update Field")))
                Build();

            GUI.enabled = buildWindow.navData2d != null;
            if (GUILayout.Button("Save as New"))
            {
                buildWindow.SaveNavData(buildWindow.navData2d);
            }
            GUI.enabled = true;
        }

        public void GatherCollisionData()
        {
            if (hideSelectedGeometry)
            {
                ToogleHide(false);
            }

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

            if (hideSelectedGeometry)
            {
                ToogleHide(true);
            }
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
            else if (showType == ShowType.MergedAndExpanded)
            {
                if (expandedTree != null)
                {
                    foreach (ExpandedNode child in expandedTree.headNode.children)
                        DrawContourNode(child);
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
            else if (showType == ShowType.NavData)
            {
                if (buildWindow.navData2d != null)
                {
                    NavData2DVisualizerWindow.SceneDrawNavData2D(buildWindow.navData2d);
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
            ToogleHide(false);
            AssetDatabase.SaveAssets();
        }

        public void OnEnable()
        {
            ToogleHide(hideSelectedGeometry);
            if (buildWindow.groundWalkerSettings != null)
                Build();
            SceneView.RepaintAll();
        }
    }

    [Serializable]
    class NavAgentUI : IBuildStepHandler
    {
        [SerializeField]
        NavigationData2dBuildWindow buildWindow;
        HumanMovementActor humanMovementActor;

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
                EditorGUILayout.LabelField("NavAgent Settings:", EditorStyles.boldLabel);
                buildWindow.groundWalkerSettings.height = Mathf.Max(EditorGUILayout.FloatField("Height", buildWindow.groundWalkerSettings.height), 0.01f);
                buildWindow.groundWalkerSettings.width = Mathf.Max(EditorGUILayout.FloatField("Width", buildWindow.groundWalkerSettings.width), 0.01f);
                buildWindow.groundWalkerSettings.maxXVel = Mathf.Max(EditorGUILayout.FloatField("Max X Vel", buildWindow.groundWalkerSettings.maxXVel), 0.01f);
                buildWindow.groundWalkerSettings.slopeLimit = Mathf.Clamp(EditorGUILayout.FloatField("Slope Limit", buildWindow.groundWalkerSettings.slopeLimit), 0, 60);
                EditorGUILayout.Space();
            }

            GUI.enabled = humanMovementActor != null && buildWindow.groundWalkerSettings != null;
            if (GUILayout.Button("Copy from selected MovingActor"))
            {
                CopyValuesFromCC2DMotor();
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
                humanMovementActor = Selection.activeGameObject.GetComponent<HumanMovementActor>();
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
                humanMovementActor = Selection.activeGameObject.GetComponent<HumanMovementActor>();
                buildWindow.RepaintThisWindow();
            }
            SceneView.RepaintAll();
        }

        void CopyValuesFromCC2DMotor()
        {
            Renderer[] renderer = humanMovementActor.GetComponentsInChildren<Renderer>();
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

            SerializedObject spMotor = new SerializedObject(humanMovementActor.CC2DMotor);
            buildWindow.groundWalkerSettings.maxXVel = spMotor.FindProperty("walkHMaxSpeed").floatValue;
            buildWindow.groundWalkerSettings.gravity = spMotor.FindProperty("gravityAcceleration").floatValue;
            buildWindow.groundWalkerSettings.jumpForce = spMotor.FindProperty("jumpVAcc").floatValue;

            SerializedObject spCC2D = new SerializedObject(humanMovementActor.CharacterController2D);
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
        [SerializeField]
        List<JumpLinkSettings> jumpLinkSettings;
        Vector2 scrollPos;
        [SerializeField]
        bool showDetailsGlobal;
        [SerializeField]
        bool showInSceneGlobal;
        [SerializeField]
        bool showInSceneDetailedGlobal;

        [Serializable]
        class JumpLinkSettings
        {
            public bool showDetails = false;
            public bool showInScene = true;
            public bool showInSceneDetailed = true;
        }

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
            {
                buildWindow.jumpLinkPlacer = ScriptableObject.CreateInstance<JumpLinkPlacer>();
                buildWindow.jumpLinkPlacer.Init();
            }
            if (jumpLinkSettings == null)
            {
                jumpLinkSettings = new List<JumpLinkSettings>();
            }

            while (jumpLinkSettings.Count > buildWindow.jumpLinkPlacer.jumpLinks.Count)
            {
                jumpLinkSettings.RemoveAt(jumpLinkSettings.Count - 1);
            }

            while (jumpLinkSettings.Count < buildWindow.jumpLinkPlacer.jumpLinks.Count)
            {
                jumpLinkSettings.Add(new JumpLinkSettings());
            }
            SceneView.RepaintAll();
        }

        public void OnGUI()
        {
            if (buildWindow.navData2d == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a NavData2d", MessageType.Error);
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
                }

                EditorGUI.BeginChangeCheck();
                showInSceneGlobal = EditorGUILayout.ToggleLeft("All Visibility", showInSceneGlobal, GUILayout.Width(80));
                if (EditorGUI.EndChangeCheck())
                {
                    ToogleShowInScene(showInSceneGlobal);
                }

                EditorGUI.BeginChangeCheck();
                showInSceneDetailedGlobal = EditorGUILayout.ToggleLeft("All Scene Details", showInSceneDetailedGlobal, GUILayout.Width(120));
                if (EditorGUI.EndChangeCheck())
                {
                    ToogleShowInSceneDetailed(showInSceneDetailedGlobal);
                }

                EditorGUILayout.EndHorizontal();

                JumpLinkPlacer.JumpLink link;
                for (int iLink = 0; iLink < jumpLinkSettings.Count; iLink++)
                {
                    EditorGUI.BeginChangeCheck();

                    link = buildWindow.jumpLinkPlacer.jumpLinks[iLink];
                    EditorGUILayout.BeginHorizontal();
                    jumpLinkSettings[iLink].showDetails = EditorGUILayout.Foldout(jumpLinkSettings[iLink].showDetails, "Link");
                    jumpLinkSettings[iLink].showInScene = EditorGUILayout.ToggleLeft(jumpLinkSettings[iLink].showInScene ? "<o>" : "<->", jumpLinkSettings[iLink].showInScene, GUILayout.Width(40));
                    jumpLinkSettings[iLink].showInSceneDetailed = EditorGUILayout.ToggleLeft("Details", jumpLinkSettings[iLink].showInSceneDetailed, GUILayout.Width(55));
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        buildWindow.jumpLinkPlacer.jumpLinks.RemoveAt(iLink);
                        jumpLinkSettings.RemoveAt(iLink);
                        buildWindow.RepaintThisWindow();
                        SceneView.RepaintAll();
                        iLink--;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (jumpLinkSettings[iLink].showDetails)
                    {
                        link.worldPointA = EditorGUILayout.Vector2Field("WorldPointA", link.worldPointA);
                        link.worldPointB = EditorGUILayout.Vector2Field("WorldPointB", link.worldPointB);
                        link.xSpeedScale = EditorGUILayout.Slider("Speed Percentage", link.xSpeedScale, 0 , 1);
                        GUI.enabled = false;
                        
                        EditorGUILayout.Vector2Field("NavPointA", link.navPointA);
                        EditorGUILayout.Vector2Field("NavPointB", link.navPointB);
                        EditorGUILayout.IntField("NodeIndexA", link.nodeIndexA);
                        EditorGUILayout.IntField("NodeIndexB", link.nodeIndexB);
                        EditorGUILayout.FloatField("JumpForce", link.jumpArc.j);

                        GUI.enabled = true;
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateMappedPoints(link);
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
                    UpdateMappedPoints(buildWindow.jumpLinkPlacer.jumpLinks[buildWindow.jumpLinkPlacer.jumpLinks.Count - 1]);
                    jumpLinkSettings.Add(new JumpLinkSettings());
                    buildWindow.RepaintThisWindow();
                    SceneView.RepaintAll();
                }
                GUI.enabled = jumpLinkSettings.Count > 0;
                if (GUILayout.Button("Save as New"))
                {
                    SaveJumpLinks();
                }
            }
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            NavData2DVisualizerWindow.SceneDrawNavData2D(buildWindow.navData2d);
            JumpLinkPlacer.JumpLink link;
            for (int iLink = 0; iLink < jumpLinkSettings.Count; iLink++)
            {
                if (!jumpLinkSettings[iLink].showInScene)
                    continue;


                link = buildWindow.jumpLinkPlacer.jumpLinks[iLink];
                EditorGUI.BeginChangeCheck();

                buildWindow.DrawJumpLink(link, jumpLinkSettings[iLink].showInSceneDetailed, true);

                if (EditorGUI.EndChangeCheck())
                {
                    UpdateMappedPoints(link);
                    buildWindow.RepaintThisWindow();
                }
            }
        }

        public void OnSelectionChanges()
        {

        }

        void UpdateMappedPoints(JumpLinkPlacer.JumpLink link)
        {
            Vector2 mappedPos;
            if (buildWindow.navData2d.TryMapPoint(link.worldPointA, out mappedPos))
            {
                link.navPointA = mappedPos;
            }
            if (buildWindow.navData2d.TryMapPoint(link.worldPointB, out mappedPos))
            {
                link.navPointB = mappedPos;
            }

            Vector2 leftPoint = (link.navPointB.x < link.navPointA.x) ? link.navPointB : link.navPointA;
            Vector2 rightPoint = (link.navPointB.x < link.navPointA.x) ? link.navPointA : link.navPointB;

            float t = (rightPoint.x - leftPoint.x) / (buildWindow.groundWalkerSettings.maxXVel * link.xSpeedScale);
            float arcTargetJ = (buildWindow.groundWalkerSettings.gravity * t * 0.5f) - ((link.navPointA.y - link.navPointB.y) / t);
            if (Mathf.Abs(arcTargetJ) > buildWindow.groundWalkerSettings.jumpForce)
                link.isJumpLinkValid = false;
            else
                link.isJumpLinkValid = true;

            link.jumpArc = new JumpArcSegment(arcTargetJ, buildWindow.groundWalkerSettings.gravity, buildWindow.groundWalkerSettings.maxXVel * link.xSpeedScale, link.navPointA.x, link.navPointB.x);
        }

        void SaveJumpLinks()
        {
            string path = EditorUtility.SaveFilePanel("Save JumpLinks", "Assets", "JumpLinks", "asset");
            if (path == null || path.Length == 0)
                return;
            path = path.Substring(path.IndexOf("Assets"));
            Debug.Log(path);
            AssetDatabase.CreateAsset(buildWindow.jumpLinkPlacer, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = buildWindow.jumpLinkPlacer;
        }

        void ToogleShowDetailAll(bool showDetail)
        {
            foreach (JumpLinkSettings set in jumpLinkSettings)
            {
                set.showDetails = showDetail;
            }
        }

        void ToogleShowInScene(bool showInScene)
        {
            foreach (JumpLinkSettings set in jumpLinkSettings)
            {
                set.showInScene = showInScene;
            }
            SceneView.RepaintAll();
        }

        void ToogleShowInSceneDetailed(bool showInSceneDetailed)
        {
            foreach (JumpLinkSettings set in jumpLinkSettings)
            {
                set.showInSceneDetailed = showInSceneDetailed;
            }
            SceneView.RepaintAll();
        }
    }

    [Serializable]
    class BakeNavData : IBuildStepHandler
    {
        [SerializeField]
        NavigationData2dBuildWindow buildWindow;

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
            if (buildWindow.navData2d == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a NavData2d", MessageType.Error);
            }
            else if (buildWindow.jumpLinkPlacer == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a JumpLinkPlacer", MessageType.Error);
            }
            else
            {
                EditorGUILayout.LabelField("Manual Jumplinks:", buildWindow.jumpLinkPlacer.jumpLinks.Count.ToString());
                if (GUILayout.Button("Bake"))
                {
                    Bake();
                }
            }
        }

        void Bake()
        {
            NavigationData2D navData = ScriptableObject.Instantiate<NavigationData2D>(buildWindow.navData2d);

            Dictionary<NavNode, List<JumpLinkPlacer.JumpLink>> linkTable = new Dictionary<NavNode, List<JumpLinkPlacer.JumpLink>>(navData.nodes.Length);
            List<JumpLinkPlacer.JumpLink> cList;
            foreach (JumpLinkPlacer.JumpLink link in buildWindow.jumpLinkPlacer.jumpLinks)
            {
                if (!link.isJumpLinkValid || !link.TryRemapPoints(navData))
                    continue;

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
            buildWindow.SaveNavData(navData);
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            NavData2DVisualizerWindow.SceneDrawNavData2D(buildWindow.navData2d);
            for (int iLink = 0; iLink < buildWindow.jumpLinkPlacer.jumpLinks.Count; iLink++)
            {
                buildWindow.DrawJumpLink(buildWindow.jumpLinkPlacer.jumpLinks[iLink], false, false);
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
