using UnityEditor;

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
