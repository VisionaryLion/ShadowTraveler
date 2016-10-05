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
        [SerializeField]
        IBuildStepHandler[] buildSteps;
        IBuildStepHandler currentBuildStep { get { return buildSteps[selectedTab]; } }

        [SerializeField]
        public NavAgentGroundWalkerSettings groundWalkerSettings;
        [SerializeField]
        public ExpandedTree expandedTree;
        [SerializeField]
        public JumpLinkPlacer jumpLinkPlacer;

        void OnGUI()
        {
            EditorGUILayout.LabelField("NavData Builder", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            groundWalkerSettings = (NavAgentGroundWalkerSettings)EditorGUILayout.ObjectField("GroundWalkerSetting", groundWalkerSettings, typeof(NavAgentGroundWalkerSettings), false);
            expandedTree = (ExpandedTree)EditorGUILayout.ObjectField("ExpandedTree", expandedTree, typeof(ExpandedTree), false);
            jumpLinkPlacer = (JumpLinkPlacer)EditorGUILayout.ObjectField("JumpLinkPlacer", jumpLinkPlacer, typeof(JumpLinkPlacer), false);

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            if (groundWalkerSettings == null)
            {
                EditorGUILayout.HelpBox("Please assign a Setting.", MessageType.Error);
            }
            else
            {
                selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
                currentBuildStep.OnGUI();
            }
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
                buildSteps = new IBuildStepHandler[] { new NavAgentUI(this), new CollisionGeometryGatheringBuildStep(this), new JumpLinkPlacerUI(this) };
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
    }

    [Serializable]
    class CollisionGeometryGatheringBuildStep : IBuildStepHandler
    {
        enum ShowType
        {
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

        CollisionGeometrySet collisionGeometrySet;
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
            ContourTree contourTree = ContourTree.Build(collisionGeometrySet);
            if (contourTree != null)
            {
                buildWindow.expandedTree = ExpandedTreeSetBuilder.Build(contourTree, new float[] { buildWindow.groundWalkerSettings.height })[0];
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
            EditorGUI.BeginChangeCheck();



            selectAllStatic = EditorGUILayout.Toggle("Select all static", selectAllStatic);
            selectionMask = CustomEditorFields.LayerMaskField("Selection Mask", selectionMask);
            useSelection = EditorGUILayout.Toggle("Use Selection", useSelection);

            EditorGUILayout.HelpBox("Size of current selection = " + inputGeometry.Length, MessageType.Info);
            circleVertCount = EditorGUILayout.IntSlider("Circle Vert Count", circleVertCount, 4, 64);

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

            GUI.enabled = buildWindow.expandedTree != null;
            if (GUILayout.Button("Save"))
            {
                SaveExpandedTree();
            }
            GUI.enabled = true;
        }

        void SaveExpandedTree()
        {
            string path = EditorUtility.SaveFilePanel("Save ExpandedTree", "Assets", "ExpandedTree", "asset");
            if (path == null || path.Length == 0)
                return;
            path = path.Substring(path.IndexOf("Assets"));
            Debug.Log(path);
            AssetDatabase.CreateAsset(buildWindow.expandedTree, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = buildWindow.expandedTree;
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
                if (buildWindow.expandedTree != null)
                {
                    foreach (ExpandedNode child in buildWindow.expandedTree.headNode.children)
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
        }

        void DrawContourNode(ExpandedNode node)
        {
            Vector3[] dummyArray = new Vector3[node.contour.pointNodeCount];
            int counter = 0;

            foreach (PointNode pn in node.contour)
            {
                dummyArray[counter] = pn.pointB;
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
            EditorGUILayout.LabelField("NavAgent Settings:", EditorStyles.boldLabel);
            buildWindow.groundWalkerSettings.height = Mathf.Max(EditorGUILayout.FloatField("Height", buildWindow.groundWalkerSettings.height), 0.01f);
            buildWindow.groundWalkerSettings.width = Mathf.Max(EditorGUILayout.FloatField("Width", buildWindow.groundWalkerSettings.width), 0.01f);
            buildWindow.groundWalkerSettings.maxXVel = Mathf.Max(EditorGUILayout.FloatField("Max X Vel", buildWindow.groundWalkerSettings.maxXVel), 0.01f);
            buildWindow.groundWalkerSettings.slopeLimit = Mathf.Clamp(EditorGUILayout.FloatField("Slope Limit", buildWindow.groundWalkerSettings.slopeLimit), 0, 60);
            EditorGUILayout.Space();

            GUI.enabled = humanMovementActor != null;
            if (GUILayout.Button("Copy from selected MovingActor"))
            {
                CopyValuesFromCC2DMotor();
            }
            GUI.enabled = true;
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
    }

    [Serializable]
    class JumpLinkPlacerUI : IBuildStepHandler
    {
        [SerializeField]
        NavigationData2dBuildWindow buildWindow;
        [SerializeField]
        List<bool> foldoutJumplink;
        Vector2 scrollPos;

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

        }

        public void OnEnable()
        {
            if (buildWindow.jumpLinkPlacer == null)
            {
                buildWindow.jumpLinkPlacer = ScriptableObject.CreateInstance<JumpLinkPlacer>();
                buildWindow.jumpLinkPlacer.Init();
            }
            if (foldoutJumplink == null)
            {
                foldoutJumplink = new List<bool>();
            }

            while (foldoutJumplink.Count > buildWindow.jumpLinkPlacer.jumpLinks.Count)
            {
                foldoutJumplink.RemoveAt(foldoutJumplink.Count - 1);
            }

            while (foldoutJumplink.Count < buildWindow.jumpLinkPlacer.jumpLinks.Count)
            {
                foldoutJumplink.Add(false);
            }
            SceneView.RepaintAll();
        }

        public void OnGUI()
        {
            if (buildWindow.expandedTree == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a ExpandTree", MessageType.Error);
            }
            else
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                JumpLinkPlacer.JumpLink link;
                for (int iLink = 0; iLink < foldoutJumplink.Count; iLink++)
                {
                    EditorGUI.BeginChangeCheck();

                    link = buildWindow.jumpLinkPlacer.jumpLinks[iLink];
                    EditorGUILayout.BeginHorizontal();
                    foldoutJumplink[iLink] = EditorGUILayout.Foldout(foldoutJumplink[iLink], "Link");
                    if (GUILayout.Button("X"))
                    {
                        buildWindow.jumpLinkPlacer.jumpLinks.RemoveAt(iLink);
                        foldoutJumplink.RemoveAt(iLink);
                        buildWindow.RepaintThisWindow();
                        SceneView.RepaintAll();
                        iLink--;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (foldoutJumplink[iLink])
                    {
                        link.worldPointA = EditorGUILayout.Vector2Field("WorldPointA", link.worldPointA);
                        link.worldPointB = EditorGUILayout.Vector2Field("WorldPointB", link.worldPointB);

                        GUI.enabled = false;

                        link.navPointA = EditorGUILayout.Vector2Field("NavPointA", link.navPointA);
                        link.navPointB = EditorGUILayout.Vector2Field("NavPointB", link.navPointB);
                        link.nodeIdA = EditorGUILayout.IntField("NodeIdA", link.nodeIdA);
                        link.nodeIdB = EditorGUILayout.IntField("NodeIdB", link.nodeIdB);

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
                    buildWindow.jumpLinkPlacer.jumpLinks.Add(new JumpLinkPlacer.JumpLink());
                    UpdateMappedPoints(buildWindow.jumpLinkPlacer.jumpLinks[buildWindow.jumpLinkPlacer.jumpLinks.Count -1]);
                    foldoutJumplink.Add(false);
                    buildWindow.RepaintThisWindow();
                    SceneView.RepaintAll();
                }
                GUI.enabled = foldoutJumplink.Count > 0;
                if (GUILayout.Button("Save"))
                {
                    SaveJumpLinks();
                }
            }
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            JumpLinkPlacer.JumpLink link;
            for (int iLink = 0; iLink < foldoutJumplink.Count; iLink++)
            {
                link = buildWindow.jumpLinkPlacer.jumpLinks[iLink];
                EditorGUI.BeginChangeCheck();

                link.worldPointA = Handles.PositionHandle(link.worldPointA, Quaternion.identity);
                link.worldPointB = Handles.PositionHandle(link.worldPointB, Quaternion.identity);

                Handles.DrawDottedLine(link.navPointA, link.navPointB, 10);
                Handles.DrawLine(link.navPointA, link.worldPointA);
                Handles.DrawLine(link.navPointB, link.worldPointB);
                Handles.DrawWireDisc(link.navPointA, Vector3.forward, 0.1f);
                Handles.DrawWireDisc(link.navPointB, Vector3.forward, 0.1f);

                Handles.color = link.isJumpLinkValid ? Color.green : Color.red;

                Vector2 swapPos;
                Vector2 prevPos = new Vector2(link.jumpArc.minX, link.jumpArc.Calc(link.jumpArc.minX));
                for (float x = link.jumpArc.minX; x + 0.1f < link.jumpArc.maxX; x += 0.1f)
                {
                    swapPos = new Vector2(x, link.jumpArc.Calc(x));
                    Handles.DrawLine(prevPos, swapPos);
                    prevPos = swapPos;
                }
                Handles.DrawLine(prevPos, new Vector2(link.jumpArc.maxX, link.jumpArc.Calc(link.jumpArc.maxX)));
                Handles.color = Color.white;


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
            if (buildWindow.expandedTree.TryMapPointToContour(link.worldPointA, out mappedPos))
            {
                link.navPointA = mappedPos;
            }
            if (buildWindow.expandedTree.TryMapPointToContour(link.worldPointB, out mappedPos))
            {
                link.navPointB = mappedPos;
            }

            float targetT = (link.navPointB.x - link.navPointA.x) / buildWindow.groundWalkerSettings.maxXVel;
            float arcTargetJ = ((link.navPointB.y - link.navPointA.y) / targetT) + buildWindow.groundWalkerSettings.gravity * targetT;
            if (Mathf.Abs(arcTargetJ) > buildWindow.groundWalkerSettings.jumpForce)
                link.isJumpLinkValid = false;
            else
                link.isJumpLinkValid = true;

            //debug this arc:
            float arcLowerBound = (link.navPointB.x < link.navPointA.x) ? link.navPointB.x : link.navPointA.x;
            float arcUpperBound = (link.navPointB.x < link.navPointA.x) ? link.navPointA.x : link.navPointB.x;

            link.jumpArc = new JumpArc(arcTargetJ, buildWindow.groundWalkerSettings.gravity, buildWindow.groundWalkerSettings.maxXVel, link.navPointA, arcLowerBound, arcUpperBound);
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
