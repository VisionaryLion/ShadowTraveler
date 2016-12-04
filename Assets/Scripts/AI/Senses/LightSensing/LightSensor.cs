using UnityEngine;
using System.Collections;

namespace LightSensing
{
    public class LightSensor : MonoBehaviour
    {
        public Color CurrentBrightness
        {
            get
            {
                if (lastUpdate != Time.frameCount)
                {
                    UpdateCurrentBrightness();
                    lastUpdate = Time.frameCount;
                }
                return _currentBrightness;

            }
        }

        public LightSkin LightSkin { get { return skin; } }

        [SerializeField]
        LightSkin skin;

        //Debug Only!
        [SerializeField, ReadOnly]
        string levelOfComfort;
        [SerializeField, ReadOnly]
        Color _currentBrightnessDebug;
        //-------------

        private Color _currentBrightness;
        private int lastUpdate; // in frames

        public bool ShouldBeStunt ()
        {
            return CurrentBrightness.grayscale >= skin.stuntBrightness;
        }

        public bool ShouldDie()
        {
            return CurrentBrightness.grayscale >= skin.deathBrightness;
        }

        public bool ShouldFlee()
        {
            return CurrentBrightness.grayscale >= skin.fleeBrigthness;
        }

        void UpdateCurrentBrightness()
        {
            _currentBrightness = GlobalLightSensor.Instance.GetDynamicLightAt(transform.position);
        }

        void Update()
        {
            if (ShouldDie())
                levelOfComfort = "Die";
            else if (ShouldFlee())
                levelOfComfort = "Flee";
            else if (ShouldBeStunt())
                levelOfComfort = "Stunt";
            else
                levelOfComfort = "Comfortable";

            _currentBrightnessDebug = CurrentBrightness;
        }
    }
}
