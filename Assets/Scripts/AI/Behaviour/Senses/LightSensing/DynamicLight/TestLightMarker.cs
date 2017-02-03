using UnityEngine;
using System.Collections;

namespace LightSensing
{
    public class TestLightMarker : MonoBehaviour
    {
        [SerializeField]
        Vector2 position;
        [SerializeField, ReadOnly]
        Vector4 currentColor;
        [SerializeField]
        LightMarker[] lightMarker;
        

        void OnDrawGizmos()
        {
            if (lightMarker == null)
                lightMarker = new LightMarker[0];

            currentColor = GetColorAtPos();
            Gizmos.color = currentColor;
            Gizmos.DrawSphere(position, 0.1f);
        }

        Color GetColorAtPos()
        {
            Color finalColor = new Color(0, 0, 0, 1);
            foreach (LightMarker m in lightMarker)
            {
                if (m.IsPointInsideMarker(position))
                {
                    finalColor += m.SampleColorAt(position);
                }
            }
            finalColor.a = 1;
            return finalColor;
        }
    }
}
