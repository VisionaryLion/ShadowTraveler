using UnityEngine;
using System.Collections;
using System;

namespace ItemHandler
{
    public class StaticItemHolder : IItemHolder
    {
        [SerializeField]
        StaticItem item;

        public override IItem Item
        {
            get
            {
                return item;
            }
            set
            {
                Debug.Assert(item != null);
                Debug.Assert(item.GetType() == typeof(StaticItem));
                item = (StaticItem)value;
            }
        }
    }
}
