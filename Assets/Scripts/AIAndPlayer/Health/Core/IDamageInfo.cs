using System;

using UnityEngine;

/*
Author: Oribow
*/
namespace Combat
{
    public abstract class IDamageInfo : EventArgs, ICloneable
    {
        public const int DamageTypCount = 7;
        public enum DamageTyp
        {
            Fire,
            Explosion,
            Bullet,
            Collision,
            Healing,
            Melee,
            Water,
            Undefined //should be always the last one
        }
        public static string DamageTypToString(int typ)
        {
            switch (typ)
            {
                case 0:
                    return "Fire";
                case 1:
                    return "Explosion";
                case 2:
                    return "Bullet";
                case 3:
                    return "Collision";
                case 4:
                    return "Healing";
                case 5:
                    return "Melee";
                case 6:
                    return "Undefined";
            }
            return "Not Found!";
        }
        
        public abstract float Damage { get; set; }
        public abstract DamageTyp DmgTyp { get; }
        public abstract object Clone();

        public override string ToString()
        {
            return Damage + " " + DamageTypToString((int)DmgTyp);
        }
    }
}
