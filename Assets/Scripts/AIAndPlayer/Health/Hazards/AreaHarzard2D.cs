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
        BasicTimedDamageInfo damgeInfo;
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
            if (damgeInfo.DmgTyp != IDamageInfo.DamageTyp.Healing)
                damgeInfo.Damage *= -1;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            IDamageReciever reciever = collider.GetComponent<IDamageReciever>();
            componentBuffer.Add(collider.GetInstanceID(), reciever);

            if (instaKill)
            {
                if (reciever == null || reciever.BaseHealth == null)
                    return;
                reciever.BaseHealth.Kill(damgeInfo.DmgTyp);
            }
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            if (Time.time - lastTime >= damgeInfo.Frequency)
            {
                lastTime = Time.time;
                IDamageReciever reciever;
                if (componentBuffer.TryGetValue(collider.GetInstanceID(), out reciever))
                {
                    if (!ignoreMultiplier)
                        reciever.TakeDamage((IDamageInfo)damgeInfo.Clone(), healthChangeTyp);
                    else
                        reciever.TakeDamageIgnoreMultiplier((IDamageInfo)damgeInfo.Clone(), healthChangeTyp);
                }
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if (damgeInfo.RunTime > 0)
            {
                IDamageReciever reciever;
                if (componentBuffer.TryGetValue(collider.GetInstanceID(), out reciever))
                {
                    reciever.BaseHealth.AddLongTimeDamager((ITimedDamageInfo)damgeInfo.Clone(), healthChangeTyp);
                }
            }
            componentBuffer.Remove(collider.GetInstanceID());
            
        }
    }
}
