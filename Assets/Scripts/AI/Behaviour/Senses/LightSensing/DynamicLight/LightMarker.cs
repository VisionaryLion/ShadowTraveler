using UnityEngine;
using System.Collections;

namespace LightSensing
{
    public abstract class LightMarker : MonoBehaviour
    {
       //public abstract Color SampleLightAt(Vector2 pos);
        public abstract bool IsPointInsideMarker(Vector2 pos);
        public abstract Bounds Bounds { get; }
        public abstract Vector2 Center { get; }
        public abstract bool IsTraversable(LightSkin skin, out float traverseCostsMulitplier);
        public abstract bool OverlapsSegment(Vector2 segmentA, Vector2 segmentB);
        public abstract Color SampleLightAt(Vector2 pos);
    }
}
