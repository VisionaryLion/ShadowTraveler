using UnityEngine;
using System.Collections;
using ItemHandler;
using Entities;
using System;

namespace Equipment
{
    public class Flashlight : MonoBehaviour
    {
        [SerializeField, AssignEntityAutomaticly, HideInInspector]
        TwoHandItemEntity entity;
        [SerializeField]
        SFLight lightSrc;

        void Start()
        {
            entity.EquipedHandler += Entity_EquipedHandler;
            entity.UnequipedHandler += Entity_UnequipedHandler;
        }

        private void Entity_UnequipedHandler()
        {
            enabled = false;
        }

        private void Entity_EquipedHandler(ActingEquipmentEntity equiper)
        {
            enabled = true;
        }

        void Update()
        {
            if (Input.GetButtonDown("Flashlight"))
            {
                lightSrc.enabled = !lightSrc.enabled;
            }
        }
    }
}
