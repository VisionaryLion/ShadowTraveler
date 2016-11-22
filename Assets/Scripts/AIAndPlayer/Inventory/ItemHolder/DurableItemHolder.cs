using UnityEngine;
using System.Collections;
using System;

namespace ItemHandler
{
    public class DurableItemHolder : IItemHolder
    {
        [SerializeField]
        DurableItem item;

        public override IItem Item
        {
            get
            {
                return item;
            }
            set
            {
                Debug.Assert(item != null);
                Debug.Assert(item.GetType() == typeof(DurableItem));
                item = (DurableItem)value;
            }
        }
    }
}
