using UnityEngine;
using System.Collections;

namespace LightSensing
{
    public abstract class LightMarker : MonoBehaviour
    {
        public abstract Color SampleColorAt(Vector2 pos);
        public abstract bool IsPointInsideMarker(Vector2 pos);
        public abstract Bounds Bounds { get; }
        public abstract Vector2 LightOrigin { get; }
    }
}
