using System;
using UnityEngine;
/*
Author: Oribow
*/
namespace Combat
{
    [Serializable]
    public class BasicDamageInfo : IDamageInfo
    {
        [SerializeField]
        private DamageTyp damageTyp;
        [SerializeField]
        private float damage;

        public override float Damage
        {
            get
            {
                return damage;
            }

            set
            {
                damage = value;
            }
        }

        public override DamageTyp DmgTyp
        {
            get
            {
                return damageTyp;
            }
        }

        public BasicDamageInfo(DamageTyp damageTyp, float damage)
        {
            this.damageTyp = damageTyp;
            this.damage = damage;
        }

        public override string ToString()
        {
            return damageTyp + ", " + damage;
        }

        public override object Clone()
        {
            return new BasicDamageInfo(damageTyp, damage);
        }
    }
}
