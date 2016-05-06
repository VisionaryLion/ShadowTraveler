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
    }
}
