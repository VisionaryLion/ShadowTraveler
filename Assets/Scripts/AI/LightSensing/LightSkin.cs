using UnityEngine;
using System.Collections;

public class LightSkin : ScriptableObject {

    public bool lovesLight;
    [Range(0, 1)]
    public float stuntBrightness;
    public float stuntTimeThreshold;
    [Range(0, 1)]
    public float deathBrightness;
    public float deathTimeThreshold;
    [Range(0, 1)]
    public float fleeBrigthness;
    public float fleeTimeThreshold;

    public AnimationCurve pathfindingCostFunction;

    public bool IsTraverable(Color color, out float traverseCostsMulitplier)
    {
        float brightness = color.grayscale;
        traverseCostsMulitplier = float.MaxValue;

        if (lovesLight)
        {
            brightness = 1 - brightness;
        }

        if (brightness >= fleeBrigthness || brightness >= stuntBrightness || brightness >= deathBrightness)
            return false;

        traverseCostsMulitplier = pathfindingCostFunction.Evaluate(brightness);
        return true;
    }
}
