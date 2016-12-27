using UnityEngine;
using System.Collections;
using CC2D;
using System;

namespace Entities
{
    public class ThightAIMovementEntity : IMovementEntity
    {
        [SerializeField]
        CC2DThightAIMotor cC2DThightAIMotor;
        [SerializeField]
        NavAgent navAgent;
        [SerializeField]
        AnimationEntity animationEntity;

        public CC2DThightAIMotor CC2DThightAIMotor { get { return cC2DThightAIMotor; } }
        public NavAgent NavAgent { get { return navAgent; } }
        public override CC2DMotor CC2DMotor { get { return cC2DThightAIMotor; } }


#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            cC2DThightAIMotor = GetComponentInChildren<CC2DThightAIMotor>();
            navAgent = GetComponentInChildren<NavAgent>();
            animationEntity = GetComponentInChildren<AnimationEntity>();
        }
#endif

        void FixedUpdate()
        {
            animationEntity.Animator.SetFloat("VelocityX", Mathf.Abs(cC2DThightAIMotor.Velocity.x));
            animationEntity.Animator.SetFloat("VelocityY", cC2DThightAIMotor.Velocity.y);

            animationEntity.Animator.SetBool("IsFalling", cC2DThightAIMotor.MotorState == CC2DMotor.MState.Fall);
            animationEntity.Animator.SetBool("IsWallSliding", cC2DThightAIMotor.MotorState == CC2DMotor.MState.WallSlide);
            animationEntity.Animator.SetBool("IsOnLadder", cC2DThightAIMotor.MotorState == CC2DMotor.MState.Climb);
            animationEntity.Animator.SetBool("IsGrounded", CharacterController2D.isGrounded);

            if (cC2DThightAIMotor.MotorState == CC2DMotor.MState.Jump && cC2DThightAIMotor.PrevMotorState != CC2DMotor.MState.Jump)
                animationEntity.Animator.SetTrigger("Jump");
        }
    }
}
