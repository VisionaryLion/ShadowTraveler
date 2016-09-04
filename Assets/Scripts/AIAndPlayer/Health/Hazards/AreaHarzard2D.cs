using UnityEngine;
using System.Collections.Generic;
using Utility.ExtensionMethods;
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
        bool onlyHitOnce;
        [SerializeField]
        bool ignoreMultiplier = true;
        [SerializeField]
        LayerMask layerMask;
        [SerializeField]
        IHealth.HealthChangeTyp healthChangeTyp = IHealth.HealthChangeTyp.Clamping;

        public bool dealDamage;

        public delegate void OnHit(IDamageReciever reciever);
        public event OnHit hitHandler;

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
            if (reciever == null)
                return;
            if (!layerMask.IsLayerWithinMask(collider.gameObject.layer))
                return;

            if (dealDamage)
            {
                if (instaKill)
                {
                    reciever.Health.Kill(damgeInfo.DmgTyp);
                    if (hitHandler != null)
                        hitHandler.Invoke(reciever);
                    return;
                }
                else if (onlyHitOnce)
                {
                    reciever.TakeDamage((IDamageInfo)damgeInfo.Clone(), healthChangeTyp);
                    if (hitHandler != null)
                        hitHandler.Invoke(reciever);
                    return;
                }

            }
            componentBuffer.Add(collider.GetInstanceID(), reciever);
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            if (!dealDamage)
                return;
            if (damgeInfo.Frequency != -1 && Time.time - lastTime >= damgeInfo.Frequency)
            {
                lastTime = Time.time;
                IDamageReciever reciever;
                if (componentBuffer.TryGetValue(collider.GetInstanceID(), out reciever))
                {
                    if (!ignoreMultiplier)
                        reciever.TakeDamage((IDamageInfo)damgeInfo.Clone(), healthChangeTyp);
                    else
                        reciever.TakeDamageIgnoreMultiplier((IDamageInfo)damgeInfo.Clone(), healthChangeTyp);
                    if (hitHandler != null)
                        hitHandler.Invoke(reciever);
                }
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {

            if (dealDamage)
            {
                if (damgeInfo.RunTime > 0)
                {
                    IDamageReciever reciever;
                    if (componentBuffer.TryGetValue(collider.GetInstanceID(), out reciever))
                    {
                        reciever.Health.AddLongTimeDamager((ITimedDamageInfo)damgeInfo.Clone(), healthChangeTyp);
                        if (hitHandler != null)
                            hitHandler.Invoke(reciever);
                    }
                }
            }
            componentBuffer.Remove(collider.GetInstanceID());

        }
    }
}
