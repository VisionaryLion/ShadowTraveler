using UnityEditor;
using Utility.Editor;

namespace Inventory
{
    public class ItemDataAsset
    {

        [MenuItem("Assets/Create/Items/BasicItem")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<ItemData>("BasicItem");
        }
    }
}
