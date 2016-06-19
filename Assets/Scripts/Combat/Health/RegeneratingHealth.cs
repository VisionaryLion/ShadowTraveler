using UnityEngine;
using System.Collections;

/*
Author: Oribow
*/
namespace Combat
{
    public class RegeneratingHealth : BasicHealth
    {
        [SerializeField]
        [Tooltip("health per second")]
        float regeneratingRate;

        void Update()
        {
            ChangeHealth(new BasicDamageInfo(IDamageInfo.DamageTyp.Healing, regeneratingRate * Time.deltaTime));
        }
    }
}
