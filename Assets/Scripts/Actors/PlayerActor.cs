using UnityEngine;
using CC2D;
using Combat;

namespace Actors
{
    //Movement
    [RequireComponent(typeof(CharacterController2D))] //Requires for it self: Rigidbody, BoxCollider2D
    [RequireComponent(typeof(CC2DMotor))] //Requires for it self: Animator, CharacterController2D
    [RequireComponent(typeof(HumanInput))]

    //Health
    [RequireComponent(typeof(BasicHealth))]
    [RequireComponent(typeof(BasicDamageReceptor))]

    [ExecuteInEditMode]
    public class PlayerActor : Actor
    {
        [HideInInspector]
        [SerializeField]
        CharacterController2D characterController2D;
        [HideInInspector]
        [SerializeField]
        CC2DMotor cC2DMotor;
        [HideInInspector]
        [SerializeField]
        HumanInput humanInput;
        [HideInInspector]
        [SerializeField]
        BasicHealth basicHealth;
        [HideInInspector]
        [SerializeField]
        BasicDamageReceptor basicDamageReceptor;
        [HideInInspector]
        [SerializeField]
        new Rigidbody2D rigidbody2d;
        [HideInInspector]
        [SerializeField]
        BoxCollider2D boxCollider2D;
        [HideInInspector]
        [SerializeField]
        Animator animator;

        #region public
        public CharacterController2D CharacterController2D { get { return characterController2D; } }
        public CC2DMotor CC2DMotor { get { return cC2DMotor; } }
        public HumanInput HumanInput { get { return humanInput; } }
        public BasicHealth BasicHealth { get { return basicHealth; } }
        public BasicDamageReceptor BasicDamageReceptor { get { return basicDamageReceptor; } }
        public Rigidbody2D Rigidbody2D { get { return rigidbody2d; } }
        public BoxCollider2D BoxCollider2D { get { return boxCollider2D; } }
        public Animator Animator { get { return animator; } }
        #endregion

#if UNITY_EDITOR
        protected override void Awake()
        {
            if (_executOnce) //already executed this script. No need for setting things up
                return;
            base.Awake();

            //Load components
            rigidbody2d = GetComponent<Rigidbody2D>();
            characterController2D = GetComponent<CharacterController2D>();
            cC2DMotor = GetComponent<CC2DMotor>();
            humanInput = GetComponent<HumanInput>();
            basicHealth = GetComponent<BasicHealth>();
            basicDamageReceptor = GetComponent<BasicDamageReceptor>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();

            //Setup some script vars automatically.
            this.tag = "Player"; //Built-in-Tag can't go wrong.
            rigidbody2d.isKinematic = true;
            basicDamageReceptor.BaseHealth = GetComponent<BasicHealth>();

            //Print some custom reminder.
            Debug.LogWarning(GenerateSetUpReminderShort("Layer"));
            Debug.LogWarning(GenerateSetUpReminder("Controller", "Animator"));
            _executOnce = true;
        }
#endif
    }
}
