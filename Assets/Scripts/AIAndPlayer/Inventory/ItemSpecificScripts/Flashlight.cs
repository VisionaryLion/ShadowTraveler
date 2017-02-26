using UnityEngine;
using System.Collections;
using ItemHandler;
using Entities;
using System;

namespace Equipment
{
    public class Flashlight : ItemSpecificBase
    {
        [SerializeField]
        SFLight lightSrc;

        void Update()
        {
            if (Input.GetButtonDown("Flashlight"))
            {
                lightSrc.enabled = !lightSrc.enabled;
            }
        }

        void LateUpdate()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 dir = (mousePos - (Vector2)transform.position).normalized * equipedEntity.CC2DMotor.FacingDir;
            transform.right = dir;
        }
    }
}
