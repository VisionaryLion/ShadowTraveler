using UnityEngine;
using CC2D;
using Combat;
using AI.Brain;

namespace Entity
{
    public class ActingEntity : Entity
    {
        [SerializeField]
        RelationshipMarker relationshipMarker;
        protected InteractiveInputHandler interactiveInputHandler;


        //other entitys
        [SerializeField]
        protected IMovementEntity movementEntity;
        [SerializeField]
        HealthEntity healthEntity;
        [SerializeField]
        AnimationEntity animationEntity;

        #region public
        public InteractiveInputHandler InteractiveInputHandler { get { return interactiveInputHandler; } }
        public RelationshipMarker RelationshipMarker { get { return relationshipMarker; } }

        //---MovementEntity
        public CharacterController2D CharacterController2D { get { return movementEntity.CharacterController2D; } }
        public CC2DMotor CC2DMotor { get { return movementEntity.CC2DMotor; } }
        public Rigidbody2D Rigidbody2D { get { return movementEntity.Rigidbody2D; } }
        public BoxCollider2D BoxCollider2D { get { return movementEntity.BoxCollider2D; } }
        //---HealthEntity
        public IHealth IHealth { get { return healthEntity.IHealth; } }
        //---AnimationEntity
        public Animator Animator { get { return animationEntity.Animator; } }
        public AnimationHandler AnimationHandler { get { return animationEntity.AnimationHandler; } }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            InitInteractiveInputHandler();
            IHealth.OnDeath += IHealth_OnDeath;
        }

        private void IHealth_OnDeath(object sender, IDamageInfo e)
        {
            Destroy(this);
        }

        protected virtual void InitInteractiveInputHandler()
        {
            interactiveInputHandler = new InteractiveInputHandler(this);
        }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            //Load components
            movementEntity = LoadComponent<IMovementEntity>(movementEntity);
            healthEntity = LoadComponent<HealthEntity>(healthEntity);
            animationEntity = LoadComponent<AnimationEntity>(animationEntity);
            relationshipMarker = LoadComponent<RelationshipMarker>(relationshipMarker);

            //Print some custom reminder.
            Debug.LogWarning(GenerateSetUpReminderShort("Layer"));
        }
#endif

        public virtual void SetBlockAllInput(bool blockInput) { }
        public virtual void SetBlockAllNonMovement(bool blockInput) { }

        void FixedUpdate()
        {
            animationEntity.Animator.SetFloat("VelocityX", Mathf.Abs(CC2DMotor.Velocity.x));
            animationEntity.Animator.SetFloat("VelocityY", CC2DMotor.Velocity.y);

            animationEntity.Animator.SetBool("IsFalling", CC2DMotor.MotorState == CC2DMotor.MState.Fall);
            animationEntity.Animator.SetBool("IsWallSliding", CC2DMotor.MotorState == CC2DMotor.MState.WallSlide);
            animationEntity.Animator.SetBool("IsOnLadder", CC2DMotor.MotorState == CC2DMotor.MState.Climb);
            animationEntity.Animator.SetBool("IsGrounded", CharacterController2D.isGrounded);
            animationEntity.Animator.SetBool("IsCrouching", CC2DMotor.MotorState == CC2DMotor.MState.Crouched);

            if (CC2DMotor.MotorState == CC2DMotor.MState.Jump && CC2DMotor.PrevMotorState != CC2DMotor.MState.Jump)
                animationEntity.Animator.SetTrigger("Jump");
        }
    }
}
