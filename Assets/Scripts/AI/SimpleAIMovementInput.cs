using UnityEngine;
using System.Collections;
using Actors;


namespace CC2D
{
    public class SimpleAIMovementInput : MonoBehaviour
    {

        [SerializeField]
        [HideInInspector]
        [AssignActorAutomaticly]
        BasicEntityActor actor;

        [SerializeField]
        float timeRunLeft;
        [SerializeField]
        float timeRunRight;
        [SerializeField]
        float deviation;
        [SerializeField]
        int randomJumpChance;
        [SerializeField]
        bool runRight;

        MovementInput bufferedInput;

        bool resetOnce = false;
        float timeLeft;
        float maxJumpExecutionDelay = .5f;

        void Awake()
        {
            bufferedInput = new MovementInput();
            actor.CC2DMotor.CurrentMovementInput = bufferedInput;
            timeLeft = (runRight) ? timeRunRight : timeRunLeft;
        }
       
        // Update is called once per frame
        void Update()
        {
            if (actor.IHealth.IsDeath)
            {
                if (!resetOnce)
                {
                    actor.CC2DMotor.ResetPlayerMovementInput();
                    resetOnce = true;
                }
                return;
            }
            if (runRight)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0 || actor.CharacterController2D.collisionState.right)
                {
                    timeLeft = timeRunLeft + Random.Range(-deviation, deviation);
                    runRight = false;
                }
                bufferedInput.horizontalRaw = 1;
                bufferedInput.horizontal = 1;
                if (actor.CharacterController2D.collisionState.right || Random.Range(0, randomJumpChance) == 0)
                {
                    bufferedInput.AddEvent(new JumpEvent(maxJumpExecutionDelay));
                }
            }
            else
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0 || actor.CharacterController2D.collisionState.left)
                {
                    timeLeft = timeRunRight + Random.Range(-deviation, deviation);
                    runRight = true;
                }
                bufferedInput.horizontalRaw = -1;
                bufferedInput.horizontal = -1;
                if (actor.CharacterController2D.collisionState.left || Random.Range(0, randomJumpChance) == 0)
                {
                    bufferedInput.AddEvent(new JumpEvent(maxJumpExecutionDelay));
                }
            }
        }
    }
}
