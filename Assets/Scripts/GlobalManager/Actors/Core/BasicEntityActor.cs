using UnityEngine;
using CC2D;
using Combat;

namespace Actors
{
    public class BasicEntityActor : Actor
    {
        [SerializeField]
        AudioSource audioSource;

        //other actors
        [SerializeField]
        HumanMovementActor movementActor;
        [SerializeField]
        HealthActor healthActor;
        [SerializeField]
        AnimationActor animationActor;

        #region public
        public AudioSource AudioSource { get { return audioSource; } }

        //---MovementActor
        public CharacterController2D CharacterController2D { get { return movementActor.CharacterController2D; } }
        public CC2DMotor CC2DMotor { get { return movementActor.CC2DMotor; } }
        public Rigidbody2D Rigidbody2D { get { return movementActor.Rigidbody2D; } }
        public BoxCollider2D BoxCollider2D { get { return movementActor.BoxCollider2D; } }
        //---HealthActor
        public IHealth IHealth { get { return healthActor.IHealth; } }
        //---AnimationActor
        public Animator Animator { get { return animationActor.Animator; } }
        public AnimationHandler AnimationHandler { get { return animationActor.AnimationHandler; } }
        #endregion


#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            //Load components
            audioSource = GetComponentInChildren<AudioSource>();
            movementActor = GetComponentInChildren<HumanMovementActor>();
            healthActor = GetComponentInChildren<HealthActor>();
            animationActor = GetComponentInChildren<AnimationActor>();

            //Print some custom reminder.
            Debug.LogWarning(GenerateSetUpReminderShort("Layer"));
        }
#endif

        public virtual void SetBlockAllInput(bool blockInput) { }
        public virtual void SetBlockAllNonMovement(bool blockInput) { }

        void FixedUpdate()
        {
            animationActor.Animator.SetFloat("VelocityX", Mathf.Abs(CC2DMotor.Velocity.x));
            animationActor.Animator.SetFloat("VelocityY", CC2DMotor.Velocity.y);

            animationActor.Animator.SetBool("IsFalling", CC2DMotor.MotorState == CC2DMotor.MState.Fall);
            animationActor.Animator.SetBool("IsWallSliding", CC2DMotor.MotorState == CC2DMotor.MState.WallSlide);
            animationActor.Animator.SetBool("IsOnLadder", CC2DMotor.MotorState == CC2DMotor.MState.Climb);
            animationActor.Animator.SetBool("IsGrounded", CharacterController2D.isGrounded);

            if (CC2DMotor.MotorState == CC2DMotor.MState.Jump && CC2DMotor.PrevMotorState != CC2DMotor.MState.Jump)
                animationActor.Animator.SetTrigger("Jump");
        }

        void Update()
        {

        }


    }
}
