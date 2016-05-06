using UnityEngine;
/*
Author: Unknown
Edited by: Oribow
*/

namespace Utility
{
    [System.Serializable]
    public class Sway
    {

        public enum UpdateAxis { X, Y }

        [System.Serializable]
        public class VectorAxis
        {
            public Vector3 vector;
            public UpdateAxis updateOnX;
            public UpdateAxis updateOnY;
            public UpdateAxis updateOnZ;
        }

        public VectorAxis maxPosition;
        public VectorAxis maxRotation;
        public float positionLerpFactor = 3f;
        public float rotationLerpFactor = 3f;

        Transform target;
        Vector3 initialPosition;
        Quaternion initialRotation;


        public void Init(Transform target)
        {
            this.target = target;
            initialPosition = target.localPosition;
            initialRotation = target.localRotation;
        }

        public void Update(Vector2 input)
        {
            Vector3 positionVector = new Vector3();
            Vector3 rotationVector = new Vector3();

            positionVector.x = maxPosition.vector.x * (maxPosition.updateOnX == UpdateAxis.X ? input.x : input.y);
            positionVector.y = maxPosition.vector.y * (maxPosition.updateOnY == UpdateAxis.X ? input.x : input.y);
            positionVector.z = maxPosition.vector.z * (maxPosition.updateOnX == UpdateAxis.X ? input.x : input.y);
            rotationVector.x = maxRotation.vector.x * (maxRotation.updateOnX == UpdateAxis.X ? input.x : input.y);
            rotationVector.y = maxRotation.vector.y * (maxRotation.updateOnY == UpdateAxis.X ? input.x : input.y);
            rotationVector.z = maxRotation.vector.z * (maxRotation.updateOnX == UpdateAxis.X ? input.x : input.y);

            Vector3 finalPosition = initialPosition + positionVector;
            Quaternion finalRotation = initialRotation * Quaternion.Euler(rotationVector);

            target.localPosition = Vector3.Lerp(target.localPosition, finalPosition, Time.deltaTime * positionLerpFactor);
            target.localRotation = Quaternion.Lerp(target.localRotation, finalRotation, Time.deltaTime * rotationLerpFactor);
        }
    }
}
