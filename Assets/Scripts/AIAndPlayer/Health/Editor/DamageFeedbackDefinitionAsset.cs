using UnityEditor;

/*
Author: Oribow
*/
namespace Combat
{
    public class DamageFeedbackDefinitionAsset
    {
        [MenuItem("Assets/Create/DamageFeedbackDefinition")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<DamageFeedbackDefinition>("DamageFeedbackDefinition");
        }
    }
}
