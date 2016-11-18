using UnityEngine;
using System.Collections;
using System;

namespace ItemHandler
{
    [Serializable]
    public class DurableItem : StaticItem
    {
        public float duration;

        public DurableItem(ItemData dataSrc, bool canBePickedUp, bool canBeDropped, float duration) : base(dataSrc, canBePickedUp, canBeDropped)
        {
            this.duration = duration;
        }
    }
}
