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

        //Debug ONLY!

        void Start()
        {
            DebugGraph.Instance.StartNewGraph(GetHashCode(), new Graph(new string[] { "Brightness", "State" }), "LightSensor");
        }
        void Update()
        {
            if (ShouldDie())
            {
                levelOfComfort = "Die";
                DebugGraph.Instance.FeedChannel(GetHashCode(), 1, 3);
            }
            else if (ShouldFlee())
            {
                levelOfComfort = "Flee";
                DebugGraph.Instance.FeedChannel(GetHashCode(), 1, 2);
            }
            else if (ShouldBeStunt())
            {
                levelOfComfort = "Stunt";
                DebugGraph.Instance.FeedChannel(GetHashCode(), 1, 1);
            }
            else
            {
                levelOfComfort = "Comfortable";
                DebugGraph.Instance.FeedChannel(GetHashCode(), 1, 0);
            }

            _currentBrightnessDebug = CurrentBrightness;
            DebugGraph.Instance.FeedChannel(GetHashCode(), 0, _currentBrightnessDebug.grayscale);

        }
        //-------------
    }
}
