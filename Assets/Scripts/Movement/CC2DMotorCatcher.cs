using UnityEngine;
using Utility.ExtensionMethods;
using System.Collections;
using CC2D;

[RequireComponent(typeof(Collider2D))]
public class CC2DMotorCatcher : MonoBehaviour
{
    [SerializeField]
    LayerMask cc2dMask;
    [SerializeField]
    Vector2 normalOfCatchingSide;
    [SerializeField]
    float angleThreshold;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (cc2dMask.IsLayerWithinMask(other.gameObject.layer))
        {
            if (Mathf.Abs(Vector2.Angle(normalOfCatchingSide, other.contacts[0].normal)) <= angleThreshold)
            {
                CC2DMotor motor = other.collider.GetComponent<CC2DMotor>();
                motor.FakeTransformParent = transform;
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (cc2dMask.IsLayerWithinMask(other.gameObject.layer))
        {
            CC2DMotor motor = other.collider.GetComponent<CC2DMotor>();
            if (motor.FakeTransformParent == transform)
                motor.FakeTransformParent = null;
        }
    }
}
