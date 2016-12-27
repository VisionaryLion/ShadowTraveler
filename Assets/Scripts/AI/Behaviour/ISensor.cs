using UnityEngine;
using System.Collections;
using AI.Brain;

namespace AI.Sensor
{
    public interface ISensor
    {
        void UpdateBlackboard(Blackboard board);
    }
}
