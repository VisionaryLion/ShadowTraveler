using UnityEngine;
using LightSensing;
using AI.Brain;

namespace AI.Sensor
{
    public enum LightLevelOfComfort { Comfort, Stunt, Flee, Die }
    public class LightSensor : MonoBehaviour, ISensor
    {
        public LightSkin LightSkin { get { return skin; } }

        [SerializeField]
        LightSkin skin;

        private Color _currentBrightness;
        private int lastUpdate; // in frames

        public LightLevelOfComfort GetLevelOfComfort(float brightness)
        {
            if (brightness >= skin.deathBrightness)
                return LightLevelOfComfort.Die;
            if (brightness >= skin.fleeBrigthness)
                return LightLevelOfComfort.Flee;
            if (brightness >= skin.stuntBrightness)
                return LightLevelOfComfort.Stunt;
            return LightLevelOfComfort.Comfort;
        }

        void UpdateCurrentBrightness()
        {
            _currentBrightness = GlobalLightSensor.Instance.GetDynamicLightAt(transform.position);
        }

        public void UpdateBlackboard(Blackboard board)
        {
            UpdateCurrentBrightness();
            board.brigthness = _currentBrightness.grayscale;
            board.lightLevelOfComfort = GetLevelOfComfort(board.brigthness);
        }
    }
}
