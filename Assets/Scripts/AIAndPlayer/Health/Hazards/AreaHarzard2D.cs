using UnityEngine;
using System.Collections.Generic;
/*
Author: Oribow
*/
namespace Combat
{
    [RequireComponent(typeof(Collider2D))]
    public class AreaHarzard2D : MonoBehaviour
    {
        [SerializeField]
        BasicTimedDamageInfo dmgInf;
        [SerializeField]
        bool instaKill = false;
        [SerializeField]
        bool ignoreMultiplier = true;
        [SerializeField]
        IHealth.HealthChangeTyp healthChangeTyp = IHealth.HealthChangeTyp.Clamping;

        Dictionary<int, IDamageReciever> componentBuffer;
        float lastTime;

        void Awake()
        {
            componentBuffer = new Dictionary<int, IDamageReciever>();
            if (dmgInf.DmgTyp != IDamageInfo.DamageTyp.Healing)
                dmgInf.Damage *= -1;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            IDamageReciever reciever = collider.GetComponent<IDamageReciever>();
            componentBuffer.Add(collider.GetInstanceID(), reciever);

            if (instaKill)
            {
                if (reciever == null || reciever.BaseHealth == null)
                    return;
                reciever.BaseHealth.Kill(dmgInf.DmgTyp);
            }
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            if (Time.time - lastTime >= dmgInf.Frequency)
            {
                lastTime = Time.time;
                IDamageReciever reciever;
                if (componentBuffer.TryGetValue(collider.GetInstanceID(), out reciever))
                {
                    if (reciever == null)
                        return;
                    if (dmgInf.RunTime <= 0)
                    {
                        if (!ignoreMultiplier)
                            reciever.TakeDamage((IDamageInfo)dmgInf.Clone(), healthChangeTyp);
                        else
                            reciever.TakeDamageIgnoreMultiplier((IDamageInfo)dmgInf.Clone(), healthChangeTyp);
                    }
                    else
                        reciever.BaseHealth.AddLongTimeDamager((ITimedDamageInfo)dmgInf.Clone(), healthChangeTyp);
                }
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            componentBuffer.Remove(collider.GetInstanceID());
        }
    }
}
