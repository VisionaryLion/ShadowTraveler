using UnityEngine;
using System.Collections;
using ItemHandler;

namespace Actors
{
    public class ItemActor : Actor
    {
        [SerializeField]
        ItemHolder itemHolder;

        #region public
        public IItem Item { get { return itemHolder.item; } }
        #endregion

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            itemHolder = GetComponentInChildren<ItemHolder>();
        }
#endif
    }
}
