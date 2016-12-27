using AI.Sensor;
using Entity;

namespace AI.Brain
{
    public class BlackboardHolder
    {
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
    }
}


