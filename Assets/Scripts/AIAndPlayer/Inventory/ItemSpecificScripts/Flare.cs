using UnityEngine;
using System.Collections;
using Entities;
using ItemHandler;
using System;

namespace Equipment
{
    public class Flare : ItemSpecificBase
    {
        [SerializeField]
        int noCC2DInteractionLayer;
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
        [SerializeField]
        ItemPickup itemPickUpper;

        bool isBurning = false;
        bool burnedOut = false;
        bool detachedAndBurning = false;
        DurableItem item;

        protected override void Start()
        {
            base.Start();
            Debug.Assert(entity.Item.GetType() == typeof(DurableItem));
            item = (DurableItem)entity.ItemHolder.Item;
            entity.PickUpHandler += Actor_PickUpHandler;
            entity.DropedHandler += Actor_DropedHandler;
            Debug.Assert(item.duration > 0);
        }

        private void Actor_DropedHandler()
        {
            if (!detachedAndBurning)
            {
                rigidbody2d.isKinematic = false;
                mainCollider.enabled = true;
                equipedEntity = null;
                flareParticle.Stop();
            }
        }

        private void Actor_PickUpHandler(ActingEquipmentEntity equiper)
        {
            equipedEntity = equiper;
            rigidbody2d.isKinematic = true;
            mainCollider.enabled = false;

        }

        protected override void Entity_UnequipedHandler()
        {
            base.Entity_UnequipedHandler();
            if (isBurning && !burnedOut)
            {
                //Throw away!!!
            }
            //enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsOnPlayer())
                return;

            if (!isBurning)
            {
                if (Input.GetMouseButtonDown(entity.EquipedWithRightHand ? 1 : 0))
                {
                    isBurning = true;
                    lightSrc.enabled = true;
                    flareParticle.Play();
                    HUDManager.hudManager.startBurn(entity.EquipedWithRightHand ? true : false, item);
                }
            }
            else if(!burnedOut)
            {     
                item.duration -= durationUsagePerSecond * Time.deltaTime;
                if (item.duration <= 0)
                {
                    burnedOut = true;
                    lightSrc.enabled = false;
                    flareParticle.Stop();
                    if(!detachedAndBurning)
                    DetachFlare();
                }
                else if (Input.GetMouseButtonDown(entity.EquipedWithRightHand ? 1 : 0))
                {
                    rigidbody2d.isKinematic = false;

                    Vector2 dirForce = throwForce;
                    dirForce.x *= equipedEntity.CC2DMotor.FacingDir;
                    rigidbody2d.AddForce(dirForce, ForceMode2D.Impulse);
                    DetachFlare();

                    if (entity.EquipedWithRightHand)
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

        void DetachFlare()
        {
            itemPickUpper.enabled = false;
            equipedEntity.TwoHandEquipmentManager.DepleteEquipedItem(entity.EquipedWithRightHand, 1);
            entity.transform.parent = OrganisationalTransforms.Instance.DroppedItemRoot;
            rigidbody2d.isKinematic = false;
            mainCollider.enabled = true;
            detachedAndBurning = true;
            equipedEntity = null;
            this.enabled = false;
            this.gameObject.layer = noCC2DInteractionLayer;
        }
    }
}
