using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
Author: Oribow
*/
namespace Combat
{
    public abstract class ITimedDamageInfo : IDamageInfo
    {
        public enum ConflictResolve
        {
            Substract,
            DoNothing,
            EliminateOther
        }
        public enum AddingBehaivior
        {
            Add,
            Replace,
            AddMostDangerous,
        }

        public static ConflictResolve StandartDamageTypEliminates(DamageTyp typ1, DamageTyp typ2)
        {
            switch (typ1)
            {
                case DamageTyp.Water:
                    switch (typ2)
                    {
                        case DamageTyp.Fire:
                            return ConflictResolve.Substract;
                        default:
                            return ConflictResolve.DoNothing;
                    }
                case DamageTyp.Healing:
                    return ConflictResolve.EliminateOther;
                default:
                    return ConflictResolve.DoNothing;
            }
        }

        public abstract float Frequency { get; set; }
        public abstract ConflictResolve DamageTypEliminates(DamageTyp typ2);
        public abstract AddingBehaivior AddBehaivior { get; }
        public abstract float RunTime { get; set; }
    }
}
