using AI.Sensor;
using Entities;

namespace AI.Brain
{
    public class BlackboardHolder
    {
        public Blackboard Blackboard { get { return _blackboard; } }

        Blackboard _blackboard;
        ISensor[] _sensors;

        public BlackboardHolder(params ISensor[] sensors)
        {
            this._sensors = sensors;
            _blackboard = new Blackboard();
        }

        public void Update()
        {
            foreach (var sensor in _sensors)
            {
                sensor.UpdateBlackboard(_blackboard);
            }
        }
    }

    public class Blackboard
    {
        //Vision
        public class OtherEntity
        {
            ActingEntity otherActor;
            float distance;

            public OtherEntity(ActingEntity otherActor, float distance)
            {
                this.otherActor = otherActor;
                this.distance = distance;
            }
        }
        public OtherEntity[] visibleHostileEntities;
        //Light
        public float brigthness;
        public LightLevelOfComfort lightLevelOfComfort;

#if UNITY_EDITOR
        public void LogBlackboardContent(string handle)
        {
            DebugPanel.Log("Brigthness", handle + ": Blackboard", brigthness);
            DebugPanel.Log("Light Level Of Comfort", handle + ": Blackboard", lightLevelOfComfort);
            DebugPanel.Log("Visible Enemy Count", handle + ": Blackboard", visibleHostileEntities.Length);
        }
#endif
    }
}


