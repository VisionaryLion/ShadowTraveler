using System;
using UnityEngine;

/*
Author: Oribow
*/
namespace Combat
{
    public class BasicDamageReceptor : IDamageReciever
    {
        [SerializeField]
        IHealth health;
        [SerializeField]
        float multiplikator = 1;
        [SerializeField]
        bool debug = false;

        public override IHealth BaseHealth
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
            }
        }

        public override void TakeDamage(IDamageInfo dmgInf)
        {
            if (health != null)
            {
if(debug == true)
                Debug.Log(name + " recieved " + dmgInf + ", resulting in " + (dmgInf.Damage * multiplikator) + " damage.");
                dmgInf.Damage *= multiplikator;
                health.ChangeHealth(dmgInf);
            }
        }

        public override void TakeDamage(IDamageInfo dmgInf, IHealth.HealthChangeTyp changeTyp)
        {
            switch (changeTyp)
            {
                case IHealth.HealthChangeTyp.Clamping:
                    TakeDamage(dmgInf);
                    break;
                case IHealth.HealthChangeTyp.Raw:
                    TakeDamageIgnoreResistance(dmgInf);
                    break;
                case IHealth.HealthChangeTyp.NoClamping:
                    TakeDamageDontClamp(dmgInf);
                    break;
            }
        }

        public override void TakeDamageDontClamp(IDamageInfo dmgInf)
        {
            if (health != null)
            {
                dmgInf.Damage *= multiplikator;
                health.ChangeHealth_NoClamping(dmgInf);
            }
        }

        public override void TakeDamageDontClampIgnoreMultiplier(IDamageInfo dmgInf)
        {
            if (health != null)
            {
                health.ChangeHealth_NoClamping(dmgInf);
            }
        }

        public override void TakeDamageIgnoreMultiplier(IDamageInfo dmgInf)
        {
            if (health != null)
                health.ChangeHealth(dmgInf);
        }

        public override void TakeDamageIgnoreMultiplier(IDamageInfo dmgInf, IHealth.HealthChangeTyp changeTyp)
        {
            switch (changeTyp)
            {
                case IHealth.HealthChangeTyp.Clamping:
                    TakeDamageIgnoreMultiplier(dmgInf);
                    break;
                case IHealth.HealthChangeTyp.Raw:
                    TakeDamageRaw(dmgInf);
                    break;
                case IHealth.HealthChangeTyp.NoClamping:
                    TakeDamageDontClampIgnoreMultiplier(dmgInf);
                    break;
            }
        }

        public override void TakeDamageIgnoreResistance(IDamageInfo dmgInf)
        {
            if (health != null)
            {
                dmgInf.Damage *= multiplikator;
                health.ChangeHealthRaw(dmgInf);
            }
        }

        public override void TakeDamageRaw(IDamageInfo dmgInf)
        {
            if (health != null)
                health.ChangeHealthRaw(dmgInf);
        }
    }
}
