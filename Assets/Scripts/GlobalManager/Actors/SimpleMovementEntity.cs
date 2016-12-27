using UnityEngine;
using System.Collections;
using CC2D;

namespace Entity
{
    public class SimpleMovingEntity : Entity
    {
        [SerializeField]
        CharacterController2D characterController2D;
        [SerializeField]
        Rigidbody2D rigidbody2d;
        [SerializeField]
        BoxCollider2D boxCollider2D;

        public Rigidbody2D Rigidbody2D { get { return rigidbody2d; } }
        public BoxCollider2D BoxCollider2D { get { return boxCollider2D; } }
        public CharacterController2D CharacterController2D { get { return characterController2D; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            characterController2D = LoadComponent<CharacterController2D>(characterController2D);
            rigidbody2d = LoadComponent<Rigidbody2D>(rigidbody2d);
            boxCollider2D = LoadComponent<BoxCollider2D>(boxCollider2D);

            rigidbody2d.isKinematic = true;
        }
#endif
    }
}
