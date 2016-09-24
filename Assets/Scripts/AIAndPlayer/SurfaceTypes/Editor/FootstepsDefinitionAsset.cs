using UnityEditor;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    public class FootstepsDefinitionAsset
    {
        [MenuItem("Assets/Create/SurfaceDefinitions/FootstepsDefinition")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<FootstepsDefinition>("FootstepsDefinition");
        }
    }
}
