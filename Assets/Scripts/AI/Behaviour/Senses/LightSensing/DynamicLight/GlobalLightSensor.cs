using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LightSensing
{
    public class GlobalLightSensor : MonoBehaviour {
        public static GlobalLightSensor Instance { get { return instance; } }
        static GlobalLightSensor instance;

        [SerializeField]
        LightMarker[] globalLightMarker;

        void Awake()
        {
            instance = this;
        }

        public Color GetDynamicLightAt(Vector2 pos, ref List<LightMarker> touchedMarkers)
        {
            touchedMarkers.Clear();
            Color result = new Color(0, 0, 0, 1);
            foreach (var marker in globalLightMarker)
            {
                if (marker.IsPointInsideMarker(pos))
                {
                    result += marker.SampleLightAt(pos);
                    touchedMarkers.Add(marker);
                }
            }
            return result;
        }
    }
}
