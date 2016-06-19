using UnityEngine;
/*
Author: Oribow
*/
namespace Combat
{
    //Its abstract because Unity doesnt handle interfaces well.
    public abstract class IDamageReciever : MonoBehaviour
    {
        /// <summary>
        /// Will apply the damage.
        /// /// <summary>
        public abstract void TakeDamage(IDamageInfo dmgInf, IHealth.HealthChangeTyp changeTyp);

        /// <summary>
        /// Will apply the damage.
        /// /// <summary>
        public abstract void TakeDamage(IDamageInfo dmgInf);

        /// <summary>
        ///Will apply the damage without any modifications.
        /// /// <summary>
        public abstract void TakeDamageRaw(IDamageInfo dmgInf);

        /// <summary>
        ///Will apply the damage without any resistance applied.
        /// /// <summary>
        public abstract void TakeDamageIgnoreResistance(IDamageInfo dmgInf);

        /// <summary>
        ///Will apply the damage without clamp the health to between 0 and max.
        /// /// <summary>
        public abstract void TakeDamageDontClamp(IDamageInfo dmgInf);

        /// <summary>
        ///Will apply the damage without clamp the health to between 0 and max.
        /// /// <summary>
        public abstract void TakeDamageDontClampIgnoreMultiplier(IDamageInfo dmgInf);

        /// <summary>
        ///Will apply the damage without any multiplication.
        /// /// <summary>
        public abstract void TakeDamageIgnoreMultiplier(IDamageInfo dmgInf);

        /// <summary>
        ///Will apply the damage without any multiplication.
        /// /// <summary>
        public abstract void TakeDamageIgnoreMultiplier(IDamageInfo dmgInf, IHealth.HealthChangeTyp changeTyp);

        public abstract IHealth BaseHealth
        {
            get; set;
        }
    }
}
