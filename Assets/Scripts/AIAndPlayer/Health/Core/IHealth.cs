using System;
using UnityEngine;
/*
Author: Oribow
*/
namespace Combat
{
    //Its abstract because Unity doesnt handle interfaces well.
    public abstract class IHealth : MonoBehaviour
    {
        public enum HealthChangeTyp
        {
            NoClamping,
            Raw,
            Clamping
        }

        public abstract event EventHandler<IDamageInfo> OnHealthChanged;
        public abstract event EventHandler<IDamageInfo> OnDeath;

        public abstract void AddLongTimeDamager(ITimedDamageInfo timedDmgInf, HealthChangeTyp changeTyp);
        /// <summary>
        /// Adds the parameter to the current health, but dont clamp it to max health.
        /// </summary>
        public abstract void ChangeHealth_NoClamping(IDamageInfo dmgInf);

        /// <summary>
        /// Adds the parameter to the current health and clamp it to max health.
        /// NO modifications should be made to the amount of change!
        /// </summary>
        public abstract void ChangeHealthRaw(IDamageInfo dmgInf);

        /// <summary>
        /// Adds the parameter to the current health and clamp it to max health.
        /// </summary>
        public abstract void ChangeHealth(IDamageInfo dmgInf);

        /// <summary>
        /// Sets the max health to the parameter, but dont clamp the current health to the new max health.
        /// </summary>
        public abstract void ChangeMaxHealth_NoClamping(float newMaxHealth);

        /// <summary>
        /// Sets the max health to the parameter and adjust the current health to be percentage wise as high as before.
        /// </summary>
        public abstract void ChangeMaxHealth_Percent(float newMaxHealth);

        /// <summary>
        /// Sets the max health to the parameter and clamp the current health to the new max health.
        /// </summary>
        public abstract void ChangeMaxHealth_Clamp(float newMaxHealth);

        /// <summary>
        /// Sets the current health to max health. Will use the damagetyp healing.
        /// </summary>
        public abstract void HealFull();

        /// <summary>
        /// Sets the current health to max health.
        /// </summary>
        public abstract void HealFull(IDamageInfo.DamageTyp dmgTyp);

        /// <summary>
        /// Sets the current health to 0.
        /// </summary>
        public abstract void Kill(IDamageInfo.DamageTyp dmgTyp);

        /// <summary>
        /// Fires a event to all listeners.
        /// </summary>
        public abstract void FireEvents(IDamageInfo dmgInf);

        public abstract ITimedDamageInfo TryGetLongTimeDanger(IDamageInfo.DamageTyp typ);
        public abstract int TryGetNextLongTimeDanger(IDamageInfo.DamageTyp typ, int index, out ITimedDamageInfo timedDmgInf);

        public abstract Resistance Resistance { get; set; }

        public abstract float CurrentHealth { get; set; }
        public abstract float MaxHealth {get; }

        /// <summary>
        /// Returns true, if the health drops below zero
        /// </summary>
        public abstract bool IsDeath { get; }

        /// <summary>
        /// Returns true, if the current health is higher then the max health
        /// </summary>
        public abstract bool IsOverpowered { get; }

        /// <summary>
        /// When set to false, no event will be fired anymore.
        /// </summary>
        public abstract bool IsSilent { set; get; }
    }
}
