using UnityEngine;
using System.Collections;
using Pada1.BBCore.Framework;
using Pada1.BBCore;

namespace BehaviorBrick.Conditions
{
    [Condition("Basic/CompareFloat")]
    [Help("Compares two float values")]
    public class CompareFloat : ConditionBase
    {
        public enum CompareMethod { AEqualB, ABiggerThenB, ABiggerOrEqualToB}

        [InParam("valueA")]
        [Help("First value to be compared")]
        public float valueA;

        [InParam("valueB")]
        [Help("Second value to be compared")]
        public float valueB;

        [InParam("compareMethod")]
        [Help("compare method used when comparing")]
        public CompareMethod compareMethod;

        public override bool Check()
        {
            switch (compareMethod)
            {
                case CompareMethod.AEqualB:
                    return valueA == valueB;
                case CompareMethod.ABiggerThenB:
                    return valueA > valueB;
                case CompareMethod.ABiggerOrEqualToB:
                    return valueA >= valueB;
            }
            return false;
        }
    }
}
