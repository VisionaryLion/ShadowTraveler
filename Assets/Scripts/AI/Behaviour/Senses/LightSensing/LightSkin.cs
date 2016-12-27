using UnityEngine;
using System.Collections;

public class LightSkin : ScriptableObject {

    public bool lovesLight;
    [Range(0, 1)]
    public float stuntBrightness;
    [Range(0, 1)]
    public float deathBrightness;
    [Range(0, 1)]
    public float fleeBrigthness;

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
