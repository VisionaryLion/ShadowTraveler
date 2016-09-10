using UnityEngine;
using System.Collections.Generic;
using Pooling;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    public class DecalBuilder
    {
        public const float pushDistance = 0.01f;
        public const float maxAngle = 90;

        private static List<Vector3> bufVertices = new List<Vector3>();
        private static List<Vector3> bufNormals = new List<Vector3>();
        private static List<Vector2> bufTexCoords = new List<Vector2>();
        private static List<int> bufIndices = new List<int>();

        /// <summary>
        /// Will build a mesh for everything in bounds and parent the meshs to the origninal objects.
        /// </summary>
        public static void BuildSplittedDecalMesh(GameObject pool, Decal decal, Vector3 position, Vector3 normal, int layerMask)
        {
            Bounds worldBounds = new Bounds(position, decal.Scale);
            List<MeshFilter> affectedObjects = GetAffectedObjects(worldBounds, layerMask);

            foreach (MeshFilter go in affectedObjects)
            {
                GameObject newDecalObject = MonoBehaviour.Instantiate(pool) as GameObject;
                newDecalObject.transform.localScale = decal.Scale;
                newDecalObject.transform.position = position;
                newDecalObject.transform.rotation = Quaternion.LookRotation(-normal);
                newDecalObject.transform.SetParent(go.transform, true);
                BuildDecalForObject(decal.sprite, go.gameObject, go, newDecalObject.transform);
                Push(pushDistance);
                Mesh mesh = CreateMesh();
                if (mesh != null)
                {
                    mesh.name = "DecalMesh";
                    newDecalObject.GetComponent<MeshFilter>().mesh = mesh;
                    newDecalObject.GetComponent<MeshRenderer>().material.mainTexture = decal.sprite.texture;
                }
                else
                {
                    MonoBehaviour.Destroy(newDecalObject);
                    Debug.LogWarning("Couldnt create a decal for "+go.name);
                }
            }
        }

        private static bool IsLayerContained(int layer, int layerMask)
        {
            return (layerMask & 1 << layer) != 0;
        }

        private static List<MeshFilter> GetAffectedObjects(Bounds bounds, int layerMask)
        {
            Collider[] collider = (Collider[])GameObject.FindObjectsOfType<Collider>();
            List<MeshFilter> objects = new List<MeshFilter>();
            foreach (Collider c in collider)
            {
                if (!c.enabled) continue;
                if (!IsLayerContained(c.gameObject.layer, layerMask)) continue;

                if (bounds.Intersects(c.bounds))
                {
                    objects.AddRange(c.GetComponentsInChildren<MeshFilter>());
                }
            }
            return objects;
        }

        private static void BuildDecalForObject(Sprite sprite, GameObject affectedObject, MeshFilter affectedMeshfilter, Transform targetDecal)
        {
            Mesh affectedMesh = affectedMeshfilter.sharedMesh;
            if (affectedMesh == null) return;

            Plane right = new Plane(Vector3.right, Vector3.right / 2f);
            Plane left = new Plane(-Vector3.right, -Vector3.right / 2f);

            Plane top = new Plane(Vector3.up, Vector3.up / 2f);
            Plane bottom = new Plane(-Vector3.up, -Vector3.up / 2f);

            Plane front = new Plane(Vector3.forward, Vector3.forward / 2f);
            Plane back = new Plane(-Vector3.forward, -Vector3.forward / 2f);

            Vector3[] vertices = affectedMesh.vertices;
            int[] triangles = affectedMesh.triangles;
            int startVertexCount = bufVertices.Count;

            Matrix4x4 matrix = targetDecal.worldToLocalMatrix * affectedObject.transform.localToWorldMatrix;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                Vector3 v1 = matrix.MultiplyPoint(vertices[i1]);
                Vector3 v2 = matrix.MultiplyPoint(vertices[i2]);
                Vector3 v3 = matrix.MultiplyPoint(vertices[i3]);

                Vector3 side1 = v2 - v1;
                Vector3 side2 = v3 - v1;
                Vector3 normal = Vector3.Cross(side1, side2).normalized;

                if (Vector3.Angle(-Vector3.forward, normal) >= maxAngle) continue;


                DecalPolygon poly = new DecalPolygon(v1, v2, v3);

                poly = DecalPolygon.ClipPolygon(poly, right);
                if (poly == null) continue;
                poly = DecalPolygon.ClipPolygon(poly, left);
                if (poly == null) continue;

                poly = DecalPolygon.ClipPolygon(poly, top);
                if (poly == null) continue;
                poly = DecalPolygon.ClipPolygon(poly, bottom);
                if (poly == null) continue;

                poly = DecalPolygon.ClipPolygon(poly, front);
                if (poly == null) continue;
                poly = DecalPolygon.ClipPolygon(poly, back);
                if (poly == null) continue;

                AddPolygon(poly, normal);
            }

            GenerateTexCoords(startVertexCount, sprite);
        }

        private static void AddPolygon(DecalPolygon poly, Vector3 normal)
        {
            int ind1 = AddVertex(poly.vertices[0], normal);
            for (int i = 1; i < poly.vertices.Count - 1; i++)
            {
                int ind2 = AddVertex(poly.vertices[i], normal);
                int ind3 = AddVertex(poly.vertices[i + 1], normal);

                bufIndices.Add(ind1);
                bufIndices.Add(ind2);
                bufIndices.Add(ind3);
            }
        }

        private static int AddVertex(Vector3 vertex, Vector3 normal)
        {
            int index = FindVertex(vertex);
            if (index == -1)
            {
                bufVertices.Add(vertex);
                bufNormals.Add(normal);
                index = bufVertices.Count - 1;
            }
            else
            {
                Vector3 t = bufNormals[index] + normal;
                bufNormals[index] = t.normalized;
            }
            return (int)index;
        }

        private static int FindVertex(Vector3 vertex)
        {
            for (int i = 0; i < bufVertices.Count; i++)
            {
                if (Vector3.Distance(bufVertices[i], vertex) < 0.01f)
                {
                    return i;
                }
            }
            return -1;
        }

        private static void GenerateTexCoords(int start, Sprite sprite)
        {
            Rect rect = sprite.rect;
            rect.x /= sprite.texture.width;
            rect.y /= sprite.texture.height;
            rect.width /= sprite.texture.width;
            rect.height /= sprite.texture.height;

            for (int i = start; i < bufVertices.Count; i++)
            {
                Vector3 vertex = bufVertices[i];
                Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.y + 0.5f);
                uv.x = Mathf.Lerp(rect.xMin, rect.xMax, uv.x);
                uv.y = Mathf.Lerp(rect.yMin, rect.yMax, uv.y);

                bufTexCoords.Add(uv);
            }
        }

        private static void Push(float distance)
        {
            for (int i = 0; i < bufVertices.Count; i++)
            {
                Vector3 normal = bufNormals[i];
                bufVertices[i] += normal * distance;
            }
        }

        private static Mesh CreateMesh()
        {
            if (bufIndices.Count == 0)
            {
                return null;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = bufVertices.ToArray();
            mesh.normals = bufNormals.ToArray();
            mesh.uv = bufTexCoords.ToArray();
            mesh.uv2 = bufTexCoords.ToArray();
            mesh.triangles = bufIndices.ToArray();

            bufVertices.Clear();
            bufNormals.Clear();
            bufTexCoords.Clear();
            bufIndices.Clear();

            return mesh;
        }
    }
}
/*
Author: Oribow
*/
/*
namespace SurfaceTypUser
{
    public class DecalBuilder
    {
        public const float pushDistance = 0.01f;
        public const float maxAngle = 90;

        private static List<Vector3> bufVertices = new List<Vector3>();
        private static List<Vector3> bufNormals = new List<Vector3>();
        private static List<Vector2> bufTexCoords = new List<Vector2>();
        private static List<int> bufIndices = new List<int>();

        public static Mesh BuildDecalMesh(Sprite sprite, Transform point, int layerMask)
        {
            Bounds worldBounds = new Bounds(point.position, point.localScale);
            List<GameObject> affectedObjects = GetAffectedObjects(worldBounds, layerMask);

            foreach (GameObject go in affectedObjects)
            {
                //BuildDecalForObject(sprite, point.worldToLocalMatrix, go);
            }
            Push(pushDistance);
            Mesh mesh = CreateMesh();
            if (mesh != null)
                mesh.name = "DecalMesh";
            else
                Debug.Log("Failed to build a decal mesh at " + point.position.ToString());
            return mesh;
        }

        /// <summary>
        /// Will build a mesh for everything in bounds and parent the meshs to the origninal objects.
        /// </summary>
        public static void BuildSplittedDecalMesh(GameObject pool, Sprite sprite, Vector3 position, Vector3 normal, Vector3 scale, int layerMask)
        {
            Bounds worldBounds = new Bounds(position, scale);
            List<GameObject> affectedObjects = GetAffectedObjects(worldBounds, layerMask);

            foreach (GameObject go in affectedObjects)
            {
                BuildDecalForObject(sprite, go, position, normal, scale);
                Push(pushDistance);
                //SetOrigin(go.transform.worldToLocalMatrix);
                Mesh mesh = CreateMesh();
                if (mesh != null)
                {
                    mesh.name = "DecalMesh";
                    GameObject decal = MonoBehaviour.Instantiate(pool) as GameObject;

                    decal.GetComponent<MeshFilter>().mesh = mesh;
                    decal.GetComponent<MeshRenderer>().material.mainTexture = sprite.texture;
                    decal.transform.localScale = scale;
                    decal.transform.position = Vector3.zero;
                    decal.transform.rotation = Quaternion.identity;
                    decal.transform.SetParent(go.transform, false);
                    Debug.Log("Finished a decal mesh at " + position.ToString() + ", for " + go.name + ", verts = " + mesh.vertexCount);
                }
                else
                    Debug.Log("Failed to build a decal mesh at " + position.ToString() + ", for " + go.name);
            }
        }

        private static bool IsLayerContained(int layer, int layerMask)
        {
            return (layerMask & 1 << layer) != 0;
        }

        private static List<GameObject> GetAffectedObjects(Bounds bounds, int layerMask)
        {
            MeshRenderer[] renderers = (MeshRenderer[])GameObject.FindObjectsOfType<MeshRenderer>();
            List<GameObject> objects = new List<GameObject>();
            foreach (Renderer r in renderers)
            {
                if (!r.enabled) continue;
                Collider col = r.GetComponent<Collider>();
                if (col == null || !col.enabled) continue;
                if (!IsLayerContained(r.gameObject.layer, layerMask)) continue;

                if (bounds.Intersects(r.bounds))
                {
                    Debug.Log("Found " + r.gameObject.name + " for which a decal has to be build.");
                    objects.Add(r.gameObject);
                }
            }
            return objects;
        }

        private static void BuildDecalForObject(Sprite sprite, GameObject affectedObject, Vector3 point, Vector3 hitNormal, Vector3 scale)
        {
            Mesh affectedMesh = affectedObject.GetComponent<MeshFilter>().sharedMesh;
            if (affectedMesh == null) return;

            Plane right = new Plane(Vector3.right, affectedObject.transform.worldToLocalMatrix.MultiplyPoint(point + ((Vector3.right / 2f) * scale.z)));
            Plane left = new Plane(-Vector3.right, affectedObject.transform.worldToLocalMatrix.MultiplyPoint(point + ((-Vector3.right / 2f) * scale.z)));

            Plane top = new Plane(Vector3.up, affectedObject.transform.worldToLocalMatrix.MultiplyPoint(point + ((Vector3.up / 2f) * scale.y)));
            Debug.Log(affectedObject.transform.worldToLocalMatrix.MultiplyPoint(point + ((Vector3.up / 2f) * scale.y)));
            Plane bottom = new Plane(-Vector3.up, affectedObject.transform.worldToLocalMatrix.MultiplyPoint(point + ((-Vector3.up / 2f) * scale.y)));
            Debug.Log(affectedObject.transform.worldToLocalMatrix.MultiplyPoint(point + ((-Vector3.up / 2f) * scale.y)));
            Plane front = new Plane(Vector3.forward, affectedObject.transform.worldToLocalMatrix.MultiplyPoint(point + ((Vector3.forward / 2f) * scale.x)));
            Plane back = new Plane(-Vector3.forward, affectedObject.transform.worldToLocalMatrix.MultiplyPoint(point + ((-Vector3.forward / 2f) * scale.x)));

            Vector3[] vertices = affectedMesh.vertices;
            int[] triangles = affectedMesh.triangles;
            int startVertexCount = bufVertices.Count;

            Matrix4x4 matrix = /*point_worldToLocalMatrix * affectedObject.transform.localToWorldMatrix;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                //Vector3 v1 = matrix.MultiplyPoint(vertices[i1]);
                //Vector3 v2 = matrix.MultiplyPoint(vertices[i2]);
                //Vector3 v3 = matrix.MultiplyPoint(vertices[i3]);
                //Debug.Log("v1:"+v1+" v2:"+v2+" v3:"+v3);
                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector3 side1 = v2 - v1;
                Vector3 side2 = v3 - v1;
                Vector3 faceNormal = Vector3.Cross(side1, side2).normalized;

                if (Vector3.Angle(hitNormal, faceNormal) >= maxAngle) continue;


                DecalPolygon poly = new DecalPolygon(v1, v2, v3);

                poly = DecalPolygon.ClipPolygon(poly, right);
                if (poly == null) continue;
                poly = DecalPolygon.ClipPolygon(poly, left);
                if (poly == null) continue;

                poly = DecalPolygon.ClipPolygon(poly, top);
                if (poly == null) continue;
                poly = DecalPolygon.ClipPolygon(poly, bottom);
                if (poly == null) continue;

                poly = DecalPolygon.ClipPolygon(poly, front);
                if (poly == null) continue;
                poly = DecalPolygon.ClipPolygon(poly, back);
                if (poly == null) continue;

                AddPolygon(poly, faceNormal);
            }

            GenerateTexCoords(startVertexCount, sprite);
        }

        private static void AddPolygon(DecalPolygon poly, Vector3 normal)
        {
            int ind1 = AddVertex(poly.vertices[0], normal);
            for (int i = 1; i < poly.vertices.Count - 1; i++)
            {
                int ind2 = AddVertex(poly.vertices[i], normal);
                int ind3 = AddVertex(poly.vertices[i + 1], normal);

                bufIndices.Add(ind1);
                bufIndices.Add(ind2);
                bufIndices.Add(ind3);
            }
        }

        private static int AddVertex(Vector3 vertex, Vector3 normal)
        {
            int index = FindVertex(vertex);
            if (index == -1)
            {
                bufVertices.Add(vertex);
                bufNormals.Add(normal);
                index = bufVertices.Count - 1;
            }
            else
            {
                Vector3 t = bufNormals[index] + normal;
                bufNormals[index] = t.normalized;
            }
            return (int)index;
        }

        private static int FindVertex(Vector3 vertex)
        {
            for (int i = 0; i < bufVertices.Count; i++)
            {
                if (Vector3.Distance(bufVertices[i], vertex) < 0.01f)
                {
                    return i;
                }
            }
            return -1;
        }

        private static void GenerateTexCoords(int start, Sprite sprite)
        {
            Rect rect = sprite.rect;
            rect.x /= sprite.texture.width;
            rect.y /= sprite.texture.height;
            rect.width /= sprite.texture.width;
            rect.height /= sprite.texture.height;

            for (int i = start; i < bufVertices.Count; i++)
            {
                Vector3 vertex = bufVertices[i];
                Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.y + 0.5f);
                uv.x = Mathf.Lerp(rect.xMin, rect.xMax, uv.x);
                uv.y = Mathf.Lerp(rect.yMin, rect.yMax, uv.y);

                bufTexCoords.Add(uv);
            }
        }

        private static void Push(float distance)
        {
            for (int i = 0; i < bufVertices.Count; i++)
            {
                Vector3 normal = bufNormals[i];
                bufVertices[i] += normal * distance;
            }
        }

        private static Mesh CreateMesh()
        {
            if (bufIndices.Count == 0)
            {
                return null;
            }
            Mesh mesh = new Mesh();
            Vector3[] verts = bufVertices.ToArray();
            for (int i = 0; i < verts.Length; i++)
            {
                Debug.Log(i + ": " + verts[i]);
            }
            mesh.vertices = verts;
            mesh.normals = bufNormals.ToArray();
            mesh.uv = bufTexCoords.ToArray();
            mesh.uv2 = bufTexCoords.ToArray();
            mesh.triangles = bufIndices.ToArray();

            bufVertices.Clear();
            bufNormals.Clear();
            bufTexCoords.Clear();
            bufIndices.Clear();

            return mesh;
        }

        private static void SetOrigin(Matrix4x4 oldOrigin, Matrix4x4 newOrgin)
        {
            Matrix4x4 transform = newOrgin * oldOrigin;
            for (int i = 0; i < bufVertices.Count; i++)
            {
                Debug.Log("before : " + bufNormals[i]);
                bufNormals[i] = transform.MultiplyVector(bufNormals[i]);
                Debug.Log("after : " + bufNormals[i]);
            }
        }
    }*/