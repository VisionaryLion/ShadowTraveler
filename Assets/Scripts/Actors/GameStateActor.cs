using UnityEngine;
using Manager;
using System.Collections;

namespace Actors
{
    [ExecuteInEditMode]
    public class GameStateActor : Actor
    {

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            
        }
#endif
    }
}
