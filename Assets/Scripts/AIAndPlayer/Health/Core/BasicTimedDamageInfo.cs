using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Combat
{
    [Serializable]
    public class BasicTimedDamageInfo : ITimedDamageInfo
    {
        [SerializeField]
        private DamageTyp damageTyp;
        [SerializeField]
        private float damage;
        [SerializeField]
        private float runTime;
        [SerializeField]
        private float frequency;
        [SerializeField]
        private AddingBehaivior addingBehaivior;

        public override float RunTime
        {
            get
            {
                return runTime;
            }
            set
            {
                runTime = value;
            }
        }

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

        public override float Frequency
        {
            get
            {
                return frequency;
            }

            set
            {
                frequency = value;
            }
        }

        public BasicTimedDamageInfo(DamageTyp damageTyp, float damage, float runTime, float frequency, AddingBehaivior addingBehaivior)
        {
            this.damageTyp = damageTyp;
            this.damage = damage;
            this.runTime = runTime;
            this.frequency = frequency;
            this.addingBehaivior = addingBehaivior;
        }

        public override ConflictResolve DamageTypEliminates(DamageTyp typ2)
        {
            return StandartDamageTypEliminates(damageTyp, typ2);
        }

        public override AddingBehaivior AddBehaivior
        {
            get {
                return addingBehaivior;
            }
        }

        public override object Clone()
        {
            return new BasicTimedDamageInfo(damageTyp, damage, runTime, frequency, addingBehaivior);
        }
    }
}
