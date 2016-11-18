using UnityEngine;
using System.Collections;
using ItemHandler;

namespace Actors
{
    public class ItemActor : Actor
    {
        [SerializeField]
        IItemHolder itemHolder;

        #region public
        public IItem Item { get { return itemHolder.Item; } set { itemHolder.Item = value; } }
        #endregion

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            itemHolder = LoadComponent<IItemHolder>(itemHolder);
        }
#endif
    }
}
