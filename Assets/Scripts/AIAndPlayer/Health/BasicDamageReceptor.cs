using System;
using UnityEngine;
using Entities;

/*
Author: Oribow
*/
namespace Combat
{
    public class BasicDamageReceptor : IDamageReciever
    {
        [AssignEntityAutomaticly, SerializeField, HideInInspector]
        HealthEntity actor;
        [SerializeField]
        float multiplikator = 1;

        public override IHealth Health
        {
            get
            {
                return actor.IHealth;
            }
        }

        public override void TakeDamage(IDamageInfo dmgInf)
        {
            Debug.Assert(actor.IHealth != null);
            Debug.Log(name + " recieved " + dmgInf + ", resulting in " + (dmgInf.Damage * multiplikator) + " damage.");
            dmgInf.Damage *= multiplikator;
            actor.IHealth.ChangeHealth(dmgInf);
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
            Debug.Assert(actor.IHealth != null);
            dmgInf.Damage *= multiplikator;
            actor.IHealth.ChangeHealth_NoClamping(dmgInf);
        }

        public override void TakeDamageDontClampIgnoreMultiplier(IDamageInfo dmgInf)
        {
            Debug.Assert(actor.IHealth != null);
            actor.IHealth.ChangeHealth_NoClamping(dmgInf);
        }

        public override void TakeDamageIgnoreMultiplier(IDamageInfo dmgInf)
        {
            Debug.Assert(actor.IHealth != null);
            actor.IHealth.ChangeHealth(dmgInf);
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
            Debug.Assert(actor.IHealth != null);
            dmgInf.Damage *= multiplikator;
            actor.IHealth.ChangeHealthRaw(dmgInf);
        }

        public override void TakeDamageRaw(IDamageInfo dmgInf)
        {
            Debug.Assert(actor.IHealth != null);
            actor.IHealth.ChangeHealthRaw(dmgInf);
        }
    }
}
