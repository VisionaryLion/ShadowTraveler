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
                bufferedInput.timeOfLastJumpStateChange = Time.time;
                bufferedInput.jump = true;
                bufferedInput.isJumpConsumed = false;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                bufferedInput.timeOfLastJumpStateChange = Time.time;
            }
            else
                bufferedInput.jump = Input.GetButton("Jump");
        }

        void FixedUpdate()
        {
            bufferedInput.horizontalRaw = Input.GetAxisRaw("Horizontal");
            bufferedInput.verticalRaw = Input.GetAxisRaw("Vertical");

            bufferedInput.horizontal = Input.GetAxis("Horizontal");
            bufferedInput.vertical = Input.GetAxis("Vertical");

            _motor.CurrentMovementInput = bufferedInput;
        }
    }
}
