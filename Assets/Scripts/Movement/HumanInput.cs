using UnityEngine;

namespace CC2D
{
    [RequireComponent(typeof(CC2DMotor))]
    public class HumanInput : MonoBehaviour
    {
        CC2DMotor _motor;
        MovementInput bufferedInput;

        void Awake()
        {
            _motor = GetComponent<CC2DMotor>();
            bufferedInput = new MovementInput();
        }

        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                bufferedInput.timeOfJumpButtonDown = Time.time;
                bufferedInput.jump = true;
            }
            else
                bufferedInput.jump = Input.GetButton("Jump");
        }

        void FixedUpdate()
        {
            bufferedInput.horizontalRaw = Input.GetAxisRaw("Horizontal");
            bufferedInput.verticalRaw = Input.GetAxisRaw("Vertical");
           
            _motor.CurrentMovementInput = bufferedInput;
        }
    }
}
