using UnityEngine;
using System.Collections;
using Entities;
using ItemHandler;
using System;

namespace Equipment
{
    public class Flare : MonoBehaviour
    {

        [SerializeField, HideInInspector, AssignEntityAutomaticly]
        TwoHandItemEntity actor;


        [SerializeField]
        SFLight lightSrc;
        [SerializeField]
        float durationUsagePerSecond;
        [SerializeField]
        Rigidbody2D rigidbody2d;
        [SerializeField]
        Collider2D mainCollider;
        [SerializeField]
        ParticleSystem flareParticle;
        [SerializeField]
        Vector2 throwForce;

        ActingEquipmentEntity equiperActor;
        bool isBurning = false;
        bool burnedOut = false;
        bool detachedAndBurning = false;
        DurableItem item;

        void Start()
        {
            Debug.Assert(actor.Item.GetType() == typeof(DurableItem));
            item = (DurableItem)actor.Item;
            actor.EquipedHandler += OnEquiped;
            actor.UnequipedHandler += OnUnequiped;
            actor.PickUpHandler += Actor_PickUpHandler;
            actor.DropedHandler += Actor_DropedHandler;
            Debug.Assert(item.duration > 0);
            enabled = false;
        }

        private void Actor_DropedHandler()
        {
            if (!detachedAndBurning)
            {
                rigidbody2d.isKinematic = false;
                mainCollider.enabled = true;
                equiperActor = null;
                flareParticle.Stop();
            }
        }

        private void Actor_PickUpHandler(ActingEquipmentEntity equiper)
        {
            equiperActor = equiper;
            rigidbody2d.isKinematic = true;
            mainCollider.enabled = false;

        }

        private void OnUnequiped()
        {
            if (isBurning && !burnedOut)
            {
                //Throw away!!!
            }
            //enabled = false;
        }

        private void OnEquiped(ActingEquipmentEntity equiper)
        {
            enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isBurning)
            {
                if (Input.GetMouseButtonDown(actor.EquipedWithRightHand ? 1 : 0))
                {
                    isBurning = true;
                    lightSrc.enabled = true;
                    flareParticle.Play();
                    HUDManager.hudManager.startBurn(actor.EquipedWithRightHand ? true : false, item);
                }
            }
            else if(!burnedOut)
            {     
                item.duration -= durationUsagePerSecond * Time.deltaTime;
                if (item.duration <= 0)
                {
                    burnedOut = true;
                    lightSrc.enabled = false;
                    if (!detachedAndBurning)
                    {
                        equiperActor.TwoHandEquipmentManager.DropEquipedItem(actor.EquipedWithRightHand);
                        actor.transform.parent = OrganisationalTransforms.Instance.DroppedItemRoot;
                    }
                    else
                    {
                        flareParticle.Stop();
                    }
                }
                else if (Input.GetMouseButtonDown(actor.EquipedWithRightHand ? 1 : 0))
                {
                    detachedAndBurning = true;
                    actor.transform.parent = null;
                    equiperActor.TwoHandEquipmentManager.DepleteEquipedItem(actor.EquipedWithRightHand, 1);                    
                    rigidbody2d.isKinematic = false;
                    mainCollider.enabled = true;
                    
                    Vector2 dirForce = throwForce;
                    dirForce.x *= equiperActor.CC2DMotor.FacingDir;
                    rigidbody2d.AddForce(dirForce, ForceMode2D.Impulse);
                    equiperActor = null;
                    this.enabled = false;


                    if(actor.EquipedWithRightHand)
                    {
                        HUDManager.hudManager.decreaseRight = false;
                        HUDManager.hudManager.EmptyRight();                        
                    } else
                    {
                        HUDManager.hudManager.decreaseLeft = false;
                        HUDManager.hudManager.EmptyLeft();
                    }
                }
            }
        }
    }
}