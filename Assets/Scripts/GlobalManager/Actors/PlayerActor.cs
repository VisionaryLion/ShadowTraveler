using UnityEngine;
using CC2D;
using Combat;
using ItemHandler;

namespace Actors
{
    [ExecuteInEditMode]
    public class PlayerActor : Actor
    {
        [SerializeField]
        CharacterController2D characterController2D;
        [SerializeField]
        CC2DMotor cC2DMotor;
        [SerializeField]
        HumanInput humanInput;
        [SerializeField]
        BasicHealth basicHealth;
        [SerializeField]
        BasicDamageReceptor basicDamageReceptor;
        [SerializeField]
        Rigidbody2D rigidbody2d;
        [SerializeField]
        BoxCollider2D boxCollider2D;
        [SerializeField]
        Inventory inventory;

        #region public
        public CharacterController2D CharacterController2D { get { return characterController2D; } }
        public CC2DMotor CC2DMotor { get { return cC2DMotor; } }
        public HumanInput HumanInput { get { return humanInput; } }
        public BasicHealth BasicHealth { get { return basicHealth; } }
        public BasicDamageReceptor BasicDamageReceptor { get { return basicDamageReceptor; } }
        public Rigidbody2D Rigidbody2D { get { return rigidbody2d; } }
        public BoxCollider2D BoxCollider2D { get { return boxCollider2D; } }
        public Inventory Inventory { get { return inventory; } }
        #endregion



#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();
            
            //Load components
            rigidbody2d = GetComponentInChildren<Rigidbody2D>();
            characterController2D = GetComponentInChildren<CharacterController2D>();
            cC2DMotor = GetComponentInChildren<CC2DMotor>();
            humanInput = GetComponentInChildren<HumanInput>();
            basicHealth = GetComponentInChildren<BasicHealth>();
            basicDamageReceptor = GetComponentInChildren<BasicDamageReceptor>();
            boxCollider2D = GetComponentInChildren<BoxCollider2D>();
            inventory = GetComponentInChildren<Inventory>();

            //Setup some script vars automatically.
            this.tag = "Player"; //Built-in-Tag can't go wrong.
            rigidbody2d.isKinematic = true;
            basicDamageReceptor.BaseHealth = basicHealth;

            //Print some custom reminder.
            Debug.LogWarning(GenerateSetUpReminderShort("Layer"));
            Debug.LogWarning(GenerateSetUpReminder("Controller", "Animator"));
        }
#endif
    }
}
