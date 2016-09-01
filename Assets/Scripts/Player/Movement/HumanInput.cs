using UnityEngine;
using Actors;

namespace CC2D
{
    public class HumanInput : MonoBehaviour
    {
        [AssignActorAutomaticly]
        PlayerActor actor;

        MovementInput bufferedInput;
        bool allowInput;

        void Awake()
        {
            bufferedInput = new MovementInput();
        }

        void Start()
        {
            actor.CC2DMotor.CurrentMovementInput = bufferedInput;
        }

        void Update()
        {
            if (!allowInput)
                return;

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
            if (!allowInput)
                return;

            bufferedInput.horizontalRaw = Input.GetAxisRaw("Horizontal");
            bufferedInput.verticalRaw = Input.GetAxisRaw("Vertical");

            bufferedInput.horizontal = Input.GetAxis("Horizontal");
            bufferedInput.vertical = Input.GetAxis("Vertical");
        }

        public void SetAllowInput(bool enabled)
        {
            allowInput = enabled;
            if (!allowInput)
                ResetPlayerMovementInput();
        }

        public void ResetPlayerMovementInput()
        {
            bufferedInput.horizontal = 0;
            bufferedInput.horizontalRaw = 0;
            bufferedInput.vertical = 0;
            bufferedInput.verticalRaw = 0;
            bufferedInput.jump = false;
            bufferedInput.isJumpConsumed = false;
            bufferedInput.timeOfLastJumpStateChange = 0;
        }
    }
}
