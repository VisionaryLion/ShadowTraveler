﻿using UnityEditor;
using Utility.Editor;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    public class SurfaceDefinitionAsset
    {
        [MenuItem("Assets/Create/SurfaceDefinitions/SurfaceDefintion")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<SurfaceTypes>("SurfaceDefintion");
        }
    }
}