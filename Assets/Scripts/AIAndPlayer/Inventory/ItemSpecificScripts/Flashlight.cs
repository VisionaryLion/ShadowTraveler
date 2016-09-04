using UnityEngine;
using System.Collections;
using ItemHandler;
using Actors;
using System;

namespace Equipment
{
    public class Flashlight : MonoBehaviour, IEquipment
    {
        [SerializeField]
        Light lightSrc;

        public void OnEquiped()
        {
            enabled = true;
        }

        public void OnUnequiped()
        {
            enabled = false;
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
