using UnityEngine;
using System.Collections;

namespace ItemHandler
{
    public class TwoHandItemData : ItemData
    {
        public enum HandPreference { None, RightPrefered, LeftPrefered, MustBeRight, MustBeLeft }

        public HandPreference handPreference;
    }
}
