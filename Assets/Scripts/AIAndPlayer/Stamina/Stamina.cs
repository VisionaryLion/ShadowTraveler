using UnityEngine;
using System.Collections;
using System;
/*
Author: Unkown (if you are the author, please fill in your name!)
Edited by: Oribow
*/
namespace Combat
{
    public class Stamina : MonoBehaviour
    {
        [SerializeField]
        float currentStamina = 100f;
        [SerializeField]
        float maxStamina = 100f;
        [SerializeField]
        float boostDepletionRate = 1;

        bool isBoosted = false;
        bool isSilent = false;
        public event EventHandler<EventArgs> OnStaminaChanges;
        public event EventHandler<EventArgs> OnStaminaDepleted;
        public event EventHandler<EventArgs> OnStaminaRegenerates;
        public event EventHandler<EventArgs> OnStaminaDrains;
        public event EventHandler<EventArgs> OnStaminaOverflow;
        public event EventHandler<EventArgs> OnStaminaOverflowEnds;

        public void ChangeStamina(float amount)
        {
            currentStamina += amount;
            if (amount < 0)
            {
                if (OnStaminaDrains != null && !isSilent)
                    OnStaminaDrains.Invoke(this, null);
            }
            else if (amount > 0)
            {
                if (OnStaminaRegenerates != null && !isSilent)
                    OnStaminaRegenerates.Invoke(this, null);
            }
            else
                return; //amount = 0;

            if (OnStaminaChanges != null && !isSilent)
                OnStaminaChanges.Invoke(this, null);

            if (currentStamina > maxStamina)
            {
                isBoosted = true;
                if (OnStaminaOverflow != null && !isSilent)
                    OnStaminaOverflow.Invoke(this, null);
            }
            if (currentStamina < 0)
            {
                currentStamina = 0;
                if (OnStaminaChanges != null && !isSilent)
                    OnStaminaChanges.Invoke(this, null);
            }
        }

        public void ChangeStamina_NoBoosting(float amount)
        {
            if (currentStamina + amount > maxStamina)
                amount = maxStamina - currentStamina;
            ChangeStamina(amount);
        }

        void Update()
        {
            if (!isBoosted)
                return;
            currentStamina -= Time.deltaTime * boostDepletionRate;
            if (currentStamina <= maxStamina)
            {
                currentStamina = maxStamina;
                isBoosted = false;
                OnStaminaOverflowEnds.Invoke(this, null);
            }
        }

        public bool IsDepleted
        {
            get{ return currentStamina == 0; }
        }

        public bool IsOverloaded
        {
            get { return isBoosted; }
        }

        public float CurrentStamina
        {
            get { return currentStamina; }
            set { currentStamina = value; }
        }

        public float MaxStamina
        {
            get { return maxStamina; }
            set { maxStamina = value; }
        }
    }
}
