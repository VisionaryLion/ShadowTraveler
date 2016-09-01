using UnityEngine;
using System.Collections;
using ItemHandler;
using Actors;

namespace Equipment
{
    public class Flashlight : MonoBehaviour
    {
        [SerializeField]
        Light lightSrc;

        void Update()
        {
            if (Input.GetButtonDown("Flashlight"))
            {
                lightSrc.enabled = !lightSrc.enabled;
            }
        }
    }
}
