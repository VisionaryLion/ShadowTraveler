using Pada1.BBCore.Framework;
using Pada1.BBCore;
using Entities;
using LightSensing;
using System.Collections.Generic;

namespace BehaviorBrick.Conditions
{
    [Condition("Perception/IsLightLevelComfortable")]
    [Help("Checks if the current experienced light level is comfortable.")]
    public class IsLightLevelComfortable : ConditionBase
    {
        public enum LightLevelOfComfort { Comfort, Stunt, Flee, Die }

        [InParam("AIEntity")]
        AIEntity entity;

        [InParam("Inverse")]
        [Help("Inverse the result")]
        bool inverse;

        [OutParam("TouchedDynamicMarker")]
        List<LightMarker> touchedMarker;

        public IsLightLevelComfortable()
        {
            touchedMarker = new List<LightMarker>(4);
        }

        public override bool Check()
        {
            bool check = GetLevelOfComfort(
                GlobalLightSensor.Instance.GetDynamicLightAt(entity.transform.position, ref touchedMarker).grayscale) == LightLevelOfComfort.Comfort;
            DebugPanel.Log("IsLightLevelComfortable", entity.name, check);
            return inverse ^ GetLevelOfComfort(
                GlobalLightSensor.Instance.GetDynamicLightAt(entity.transform.position, ref touchedMarker).grayscale) == LightLevelOfComfort.Comfort;
        }

        LightLevelOfComfort GetLevelOfComfort(float brightness)
        {
            if (brightness >= entity.NavAgent.lightSkin.deathBrightness)
                return LightLevelOfComfort.Die;
            if (brightness >= entity.NavAgent.lightSkin.fleeBrigthness)
                return LightLevelOfComfort.Flee;
            if (brightness >= entity.NavAgent.lightSkin.stuntBrightness)
                return LightLevelOfComfort.Stunt;
            return LightLevelOfComfort.Comfort;
        }
    }
}