using UnityEngine;
using System.Collections;
using Combat;

namespace Actors
{
    public class HealthActor : Actor
    {
        [SerializeField]
        IHealth iHealth;

        public IHealth IHealth { get { return iHealth; } }


#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            //Load components
            iHealth = LoadComponent<IHealth>(iHealth);
        }
#endif
    }
}
