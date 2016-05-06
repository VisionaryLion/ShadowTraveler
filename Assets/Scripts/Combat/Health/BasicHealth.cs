using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Oribow
*/
namespace Combat
{
    public class BasicHealth : IHealth
    {
        [SerializeField]
        protected float maxHealth;
        [SerializeField]
        protected float currentHealth;
        protected float lastHealth = 0;
        [SerializeField]
        protected Resistance resistance;

        private bool deathTriggered = false;
        private bool isSilent = false;
        private List<ITimedDamageInfo> dmgBuffer;
        private List<Coroutine> longTimeDamageHandler;


        public void Awake()
        {
            if (resistance == null || resistance.resistances == null || resistance.resistances.Length != IDamageInfo.DamageTypCount)
            {
                Debug.LogError("Please specify the resistances! Will fill them with 1 now...");
                resistance.resistances = new float[IDamageInfo.DamageTypCount];
                for (int i = 0; i < IDamageInfo.DamageTypCount; i++)
                {
                    resistance.resistances[i] = 1;
                }
            }
            dmgBuffer = new List<ITimedDamageInfo>(2);
            longTimeDamageHandler = new List<Coroutine>(2);
        }

        public override float CurrentHealth
        {
            get
            {
                return currentHealth;
            }
            /// <summary>
            /// Sets the currentHealth to the parameter. Will not fire events.
            /// </summary>
            set
            {
                currentHealth = value;
            }
        }

        public override float MaxHealth
        {
            get
            {
                return maxHealth;
            }
        }

        /// <summary>
        /// Returns true, if the health drops below zero
        /// </summary>
        public override bool IsDeath
        {
            get
            {
                return currentHealth < 0;
            }
        }

        /// <summary>
        /// Returns true, if the health is bigger then the maximum allowed Health
        /// </summary>
        public override bool IsOverpowered
        {
            get
            {
                return currentHealth > maxHealth;
            }
        }

        /// <summary>
        /// When set to false, no event will be fired anymore.
        /// </summary>
        public override bool IsSilent
        {
            get
            {
                return isSilent;
            }
            set
            {
                isSilent = value;
            }
        }

        public override Resistance Resistance
        {
            get
            {
                return resistance;
            }

            set
            {
                resistance = value;
            }
        }

        /// <summary>
        /// Adds the parameter to the current health. Will fire events. (May better use at ChangeHealth)
        /// </summary>
        public override void ChangeHealth_NoClamping(IDamageInfo dmgInf)
        {
            currentHealth += dmgInf.Damage * resistance.resistances[(int)dmgInf.DmgTyp];
            FireEvents(dmgInf);
        }

        /// <summary>
        /// Adds the parameter to the current health. Clamps the health value between 0 - maxHealth. Will fire events.
        /// </summary>
        public override void ChangeHealth(IDamageInfo dmgInf)
        {
            currentHealth += dmgInf.Damage * resistance.resistances[(int)dmgInf.DmgTyp];
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            FireEvents(dmgInf);
        }

        /// <summary>
        /// Adds the parameter to the current health. Clamps the health value between 0 - maxHealth. Will fire events.
        /// No resistance is applied.
        /// </summary>
        public override void ChangeHealthRaw(IDamageInfo dmgInf)
        {
            currentHealth += dmgInf.Damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            FireEvents(dmgInf);
        }

        /// <summary>
        /// Sets the max health to the parameter and adjust the current health to be percentage wise as high as before.
        /// </summary>
        public override void ChangeMaxHealth_Percent(float newMaxHealth)
        {
            float percent = currentHealth / maxHealth;
            maxHealth = newMaxHealth;
            currentHealth = maxHealth * percent;
            FireEvents(new BasicDamageInfo(IDamageInfo.DamageTyp.Undefined, 0));
        }

        /// <summary>
        /// Sets the max health to the parameter, but dont clamp the current health to the new max health.
        /// </summary>
        public override void ChangeMaxHealth_NoClamping(float newMaxHealth)
        {
            maxHealth = newMaxHealth;
            FireEvents(new BasicDamageInfo(IDamageInfo.DamageTyp.Undefined, 0));
        }

        /// <summary>
        /// Sets the max health to the parameter and clamp the current health to the new max health.
        /// </summary>
        public override void ChangeMaxHealth_Clamp(float newMaxHealth)
        {
            maxHealth = newMaxHealth;
            if (currentHealth > maxHealth)
            {
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                FireEvents(new BasicDamageInfo(IDamageInfo.DamageTyp.Undefined, 0));
            }
        }

        /// <summary>
        /// Puts the health to zero and fires a death event.
        /// </summary>
        public override void Kill(IDamageInfo.DamageTyp dmgTyp)
        {
            BasicDamageInfo dmgInf = new BasicDamageInfo(dmgTyp, -currentHealth);
            currentHealth = 0;
            FireEvents(dmgInf);
        }

        /// <summary>
        /// Puts the health to max health and fires events. Will use the damagetyp healing.
        /// </summary>
        public override void HealFull()
        {
            HealFull(IDamageInfo.DamageTyp.Healing);
        }

        /// <summary>
        /// Puts the health to max health and fires events.
        /// </summary>
        public override void HealFull(IDamageInfo.DamageTyp dmgTyp)
        {
            BasicDamageInfo dmgInf = new BasicDamageInfo(dmgTyp, maxHealth - currentHealth);
            currentHealth = maxHealth;
            FireEvents(dmgInf);

        }

        /// <summary>
        /// Puts the health to max health and fires events.
        /// </summary>
        public override string ToString()
        {
            return "Health = " + currentHealth + "; maxHealth = " + maxHealth;
        }

        public override event EventHandler<IDamageInfo> OnHealthChanged;
        public override event EventHandler<IDamageInfo> OnDeath;

        public override void FireEvents(IDamageInfo dmgInf)
        {
            if (isSilent)
                return;
            IDamageInfo overrideSaveDmg = (IDamageInfo)dmgInf.Clone();
            if (currentHealth <= 0)
            {
                if (OnDeath != null && !deathTriggered)
                {
                    OnDeath.Invoke(this, overrideSaveDmg);
                }
                deathTriggered = true;
            }
            else
                deathTriggered = false;
            if (OnHealthChanged != null && lastHealth != currentHealth)
            {
                lastHealth = currentHealth;
                OnHealthChanged(this, overrideSaveDmg);
            }
        }

        private bool ResolveLongTimeDamagerConflicts(ITimedDamageInfo dmgInf)
        {
            for (int i = 0; i < dmgBuffer.Count; i++)
            {
                switch (dmgInf.DamageTypEliminates(dmgBuffer[i].DmgTyp))
                {
                    case ITimedDamageInfo.ConflictResolve.EliminateOther:
                        RemoveLongTimeDamager(i);
                        i--;
                        break;
                    case ITimedDamageInfo.ConflictResolve.Substract:
                        float newDmg = dmgBuffer[i].Damage - dmgInf.Damage;
                        if (newDmg <= 0)
                        {
                            dmgInf.Damage -= dmgBuffer[i].Damage;
                            RemoveLongTimeDamager(i);
                            i--;
                        }
                        else
                        {
                            dmgBuffer[i].Damage = newDmg;
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        private void RemoveLongTimeDamager(int index)
        {
            dmgBuffer.RemoveAt(index);
            StopCoroutine(longTimeDamageHandler[index]);
            longTimeDamageHandler.RemoveAt(index);
        }

        private IEnumerator HandleLongTimeDamager(ITimedDamageInfo timedDmgInf, HealthChangeTyp healthChangeTyp)
        {
            IDamageInfo timeLessDmg = (IDamageInfo)timedDmgInf;
            while (timedDmgInf.RunTime > 0)
            {
                ChangeHealth(timeLessDmg);
                float secondsToWait = timedDmgInf.Frequency;
                yield return new WaitForSeconds(secondsToWait);
                timedDmgInf.RunTime -= secondsToWait;
            }
        }

        public override void AddLongTimeDamager(ITimedDamageInfo timedDmgInf, HealthChangeTyp healthChangeTyp)
        {
            if (!ResolveLongTimeDamagerConflicts(timedDmgInf))
                return;
            if (timedDmgInf.AddBehaivior == ITimedDamageInfo.AddingBehaivior.Replace)
            {
                int index = TryGetLongTimeDangerIndex(timedDmgInf.DmgTyp);
                if (index != -1)
                {
                    RemoveLongTimeDamager(index);
                }
                dmgBuffer.Add(timedDmgInf);
                longTimeDamageHandler.Add(StartCoroutine(HandleLongTimeDamager(timedDmgInf, healthChangeTyp)));
            }
            else if (timedDmgInf.AddBehaivior == ITimedDamageInfo.AddingBehaivior.AddMostDangerous)
            {
                ITimedDamageInfo output;
                int index = 0;
                while ((index = TryGetNextLongTimeDanger(timedDmgInf.DmgTyp, index, out output)) != -1)
                {
                    if (output.Damage < timedDmgInf.Damage)
                    {
                        output.Damage = timedDmgInf.Damage;
                        index = int.MaxValue;
                    }
                    if (output.Frequency < timedDmgInf.Frequency)
                    {
                        output.Frequency = timedDmgInf.Frequency;
                        index = int.MaxValue;
                    }
                }
            }
            else
            {
                dmgBuffer.Add(timedDmgInf);
                longTimeDamageHandler.Add(StartCoroutine(HandleLongTimeDamager(timedDmgInf, healthChangeTyp)));
            }

        }

        public override ITimedDamageInfo TryGetLongTimeDanger(IDamageInfo.DamageTyp typ)
        {
            int index = TryGetLongTimeDangerIndex(typ);
            if (index != -1)
                return dmgBuffer[index];
            return null;
        }

        public int TryGetLongTimeDangerIndex(IDamageInfo.DamageTyp typ)
        {
            for (int i = 0; i < dmgBuffer.Count; i++)
            {
                if (dmgBuffer[i].DmgTyp == typ)
                    return i;
            }
            return -1;
        }

        public override int TryGetNextLongTimeDanger(IDamageInfo.DamageTyp typ, int index, out ITimedDamageInfo timedDmgInf)
        {
            for (int i = index; i < dmgBuffer.Count; i++)
            {
                if (dmgBuffer[i].DmgTyp == typ)
                {
                    timedDmgInf = dmgBuffer[i];
                    return i;
                }
            }
            timedDmgInf = null;
            return -1;
        }
    }
}
