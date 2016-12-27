using UnityEngine;
using System.Collections;
using Combat;

namespace Entity
{
    public class HealthEntity : Entity
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
