using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Utility
{
    public class OribowsUtilitys
    {
        public static T[][] DeepCopy<T>(T[][] objectToCopy)
        {
            T[][] result = new T[objectToCopy.Length][];
            for (int i = 0; i < objectToCopy.Length; i++)
            {
                result[i] = (T[])objectToCopy[i].Clone();
            }
            return result;
        }

        public static T[] DeepCopy<T>(T[] objectToCopy)
        {
            T[] result = new T[objectToCopy.Length];
            using (var stream = new MemoryStream())
            {
                for (int i = 0; i < objectToCopy.Length; i++)
                {
                    if (objectToCopy[i] == null)
                        result[i] = default(T);
                    else
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, objectToCopy[i]);
                        stream.Position = 0;
                        result[i] = (T)formatter.Deserialize(stream);
                        stream.Position = 0;
                    }
                }
            }
            return result;
        }

        public static void DrawLine(Vector2 p1, Vector2 p2, float width)
        {
            int count = Mathf.CeilToInt(width); // how many lines are needed.
            if (count == 1)
                Gizmos.DrawLine(p1, p2);
            else
            {
                Camera c = Camera.current;
                if (c == null)
                {
                    Debug.LogError("Camera.current is null");
                    return;
                }
                Vector2 v1 = (p2 - p1).normalized; // line direction
                Vector2 v2 = ((Vector2)c.transform.position - p1).normalized; // direction to camera
                Vector2 n = Vector3.Cross(v1, v2); // normal vector
                for (int i = 0; i < count; i++)
                {
                    Vector2 o = n * (0.1f / width) * i;
                    Gizmos.DrawLine(p1 + o, p2 + o);
                }
            }
        }
    }
}
