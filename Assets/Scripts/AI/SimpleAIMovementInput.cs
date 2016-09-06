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
        bool runRight;

        MovementInput bufferedInput;

        bool resetOnce = false;
        float timeLeft;

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
                if (timeLeft < 0)
                {
                    timeLeft = timeRunLeft;
                    runRight = false;
                }
                bufferedInput.horizontalRaw = 1;
                bufferedInput.horizontal = 1;
            }
            else
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0)
                {
                    timeLeft = timeRunRight;
                    runRight = true;
                }
                bufferedInput.horizontalRaw = -1;
                bufferedInput.horizontal = -1;
            }
        }
    }
}
