using UnityEditor;
using Utility.Editor;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    public class BulletHoleDefinitionAsset
    {
        [MenuItem("Assets/Create/SurfaceDefinitions/BulletHoleDefinition")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<BulletHoleDefinition>("BulletHoleDefinition");
        }
    }
}
