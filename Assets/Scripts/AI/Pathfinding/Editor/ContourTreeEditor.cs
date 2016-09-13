using UnityEngine;
using System.Collections;
using UnityEditor;
using NavMesh2D.Core;
using System;

[CustomEditor(typeof(ContourTree))]
public class ContourTreeEditor : Editor
{
    Material previewMat;

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        RenderTexture previewTex = RenderTexture.GetTemporary((int)r.width, (int)r.height, 0);
        RenderTexture prevActive = RenderTexture.active;

        RenderTexture.active = previewTex;

        if (!previewMat)
        {
            previewMat = new Material(Shader.Find("Lines/Colored Blended"));
            previewMat.hideFlags = HideFlags.HideAndDontSave;
            previewMat.shader.hideFlags = HideFlags.HideAndDontSave;
        }
        ContourTree tree = (ContourTree)target;
        if (tree.FirstNode.children.Count > 0)
        {
            Bounds bounds = tree.FirstNode.children[0].Bounds;
            foreach (ContourNode cn in tree.FirstNode.children)
            {
                bounds.Encapsulate(cn.Bounds.min);
                bounds.Encapsulate(cn.Bounds.max);
            }

            Matrix4x4 modelMat = new Matrix4x4();
            modelMat.SetTRS(-bounds.min, Quaternion.identity, -new Vector3((bounds.extents.x * 2) / r.width, (bounds.extents.y * 2) / r.height, 0));
                GL.PushMatrix();
            GL.LoadPixelMatrix(bounds.min.x, bounds.max.x, bounds.min.y, bounds.max.y);

            GL.Begin(GL.LINES);
            previewMat.SetPass(0);
            GL.Clear(true, true, Color.white);


            int counter = 0;
            foreach (ContourNode cn in tree.FirstNode.children)
            {
                DrawContourNode(cn, counter++);
            }

            GL.End();
            GL.PopMatrix();
        }
        RenderTexture.active = prevActive;
        EditorGUI.DrawPreviewTexture(r, previewTex);
        RenderTexture.ReleaseTemporary(previewTex);

    }

    void DrawContourNode(ContourNode node, int colorIndex)
    {
        GL.Color(Utility.DifferentColors.GetColor(colorIndex));

        Vector2 prevVert = node.contour.verticies[node.contour.verticies.Count - 1];
        foreach (Vector2 vert in node.contour.verticies)
        {
            GL.Vertex(prevVert);
            GL.Vertex(vert);
            prevVert = vert;
        }

        foreach (ContourNode child in node.children)
            DrawContourNode(child, colorIndex);
    }

    public override bool HasPreviewGUI()
    {
        return true;

    }
}
