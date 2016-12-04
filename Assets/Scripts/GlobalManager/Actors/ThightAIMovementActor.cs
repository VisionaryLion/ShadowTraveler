using UnityEngine;
using System.Collections;
using CC2D;
using System;

namespace Actors
{
    public class ThightAIMovementActor : IMovementActor
    {
        [SerializeField]
        CC2DThightAIMotor cC2DThightAIMotor;
        [SerializeField]
        NavAgent navAgent;
        [SerializeField]
        AnimationActor animationActor;

        public CC2DThightAIMotor CC2DThightAIMotor { get { return cC2DThightAIMotor; } }
        public NavAgent NavAgent { get { return navAgent; } }
        public override CC2DMotor CC2DMotor { get { return cC2DThightAIMotor; } }


#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            cC2DThightAIMotor = GetComponentInChildren<CC2DThightAIMotor>();
            navAgent = GetComponentInChildren<NavAgent>();
            animationActor = GetComponentInChildren<AnimationActor>();
        }
#endif

        void FixedUpdate()
        {
            animationActor.Animator.SetFloat("VelocityX", Mathf.Abs(cC2DThightAIMotor.Velocity.x));
            animationActor.Animator.SetFloat("VelocityY", cC2DThightAIMotor.Velocity.y);

            animationActor.Animator.SetBool("IsFalling", cC2DThightAIMotor.MotorState == CC2DMotor.MState.Fall);
            animationActor.Animator.SetBool("IsWallSliding", cC2DThightAIMotor.MotorState == CC2DMotor.MState.WallSlide);
            animationActor.Animator.SetBool("IsOnLadder", cC2DThightAIMotor.MotorState == CC2DMotor.MState.Climb);
            animationActor.Animator.SetBool("IsGrounded", CharacterController2D.isGrounded);

            if (cC2DThightAIMotor.MotorState == CC2DMotor.MState.Jump && cC2DThightAIMotor.PrevMotorState != CC2DMotor.MState.Jump)
                animationActor.Animator.SetTrigger("Jump");
        }
    }
}
