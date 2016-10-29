using UnityEngine;
using NavMesh2D.Core;
using System.Collections.Generic;
using System;
using NavMesh2D;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pathfinding2D
{
    public class JumpLinkPlacer : ScriptableObject
    {
        [SerializeField]
        public List<JumpLink> jumpLinks;

        public void Init()
        {
            jumpLinks = new List<JumpLink>(5);
        }

#if UNITY_EDITOR
        public void SaveToAsset()
        {
            string path = EditorUtility.SaveFilePanel("Save JumpLinks", "Assets", "JumpLinks", "asset");
            if (path == null || path.Length == 0)
                return;
            path = path.Substring(path.IndexOf("Assets"));
            Debug.Log(path);
            AssetDatabase.CreateAsset(this, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = this;
        }
#endif

        [Serializable]
        public class JumpLink
        {
            [SerializeField]
            public Vector2 worldPointA;
            [SerializeField]
            public Vector2 worldPointB;

            [SerializeField, Range(0, 1)]
            public float xSpeedScale = 1;

            [SerializeField, ReadOnly]
            public Vector2 navPointA;
            [SerializeField, ReadOnly]
            public Vector2 navPointB;

            [SerializeField, ReadOnly]
            public int nodeIndexA;
            [SerializeField, ReadOnly]
            public int nodeIndexB;

            [SerializeField, ReadOnly]
            public int nodeVertIndexA;
            [SerializeField, ReadOnly]
            public int nodeVertIndexB;

            [SerializeField]
            public JumpArcSegment jumpArc;

            [SerializeField]
            public bool isJumpLinkValid;

            [SerializeField]
            public bool isBiDirectional;

            [SerializeField]
            public JumpLinkSettings jumpLinkSettings;

            public JumpLink()
            {
                jumpLinkSettings = new JumpLinkSettings();
            }

            public JumpLink(Vector2 worldPointA, Vector2 worldPointB)
            {
                this.worldPointA = worldPointA;
                this.worldPointB = worldPointB;
                jumpLinkSettings = new JumpLinkSettings();
            }

            public JumpLink InvertLink(NavAgentGroundWalkerSettings groundWalkerSettings)
            {
                JumpLink inverseLink = new JumpLink();
                inverseLink.worldPointA = this.worldPointB;
                inverseLink.worldPointB = this.worldPointA;
                inverseLink.xSpeedScale = this.xSpeedScale;
                inverseLink.isBiDirectional = this.isBiDirectional;
                inverseLink.navPointA = this.navPointB;
                inverseLink.navPointB = this.navPointA;
                inverseLink.RecalculateJumpArc(groundWalkerSettings);
                return inverseLink;
            }

            public bool TryRemapPoints(RawNavigationData2D navData)
            {
                Vector2 navPoint;
                int mappedVertIndex;
                int mappedNodeIndex;
                if (navData.TryMapPoint(worldPointA, out navPoint, out mappedVertIndex, out mappedNodeIndex))
                {
                    this.navPointA = navPoint;
                    this.nodeIndexA = mappedNodeIndex;
                    this.nodeVertIndexA = mappedVertIndex;
                }
                else
                {
                    return false;
                }

                if (navData.TryMapPoint(worldPointB, out navPoint, out mappedVertIndex, out mappedNodeIndex))
                {
                    this.navPointB = navPoint;
                    this.nodeIndexB = mappedNodeIndex;
                    this.nodeVertIndexB = mappedVertIndex;
                }
                else
                {
                    return false;
                }
                return true;
            }

            public void RecalculateJumpArc(NavAgentGroundWalkerSettings groundWalkerSettings)
            {
                float t = Mathf.Abs(navPointB.x - navPointA.x) / (groundWalkerSettings.maxXVel * xSpeedScale);
                float arcTargetJ = (groundWalkerSettings.gravity * t * 0.5f) - ((navPointA.y - navPointB.y) / t);

                if (Mathf.Abs(arcTargetJ) > groundWalkerSettings.jumpForce || arcTargetJ < 0)
                    isJumpLinkValid = false;
                else
                    isJumpLinkValid = true;

                if (jumpArc == null)
                    jumpArc = new JumpArcSegment(arcTargetJ, groundWalkerSettings.gravity, groundWalkerSettings.maxXVel * xSpeedScale, navPointA.x, navPointB.x);
                else
                    jumpArc.UpdateArc(arcTargetJ, groundWalkerSettings.gravity, groundWalkerSettings.maxXVel * xSpeedScale, navPointA.x, navPointB.x);
            }
        }

        [Serializable]
        public class JumpLinkSettings
        {
            public bool showDetails = false;
            public bool showInScene = true;
            public bool showInSceneDetailed = true;
        }
    }
}
