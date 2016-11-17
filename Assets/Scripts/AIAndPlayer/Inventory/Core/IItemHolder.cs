using UnityEngine;
using System.Collections;

namespace ItemHandler
{
    public abstract class IItemHolder : MonoBehaviour {

        public abstract IItem Item { get; set; }
    }
}
