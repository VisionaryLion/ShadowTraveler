using UnityEngine;
using System.Collections;
using ItemHandler;

namespace Entity
{
    public class TwoHandItemEntity : ItemEntity
    {
        bool equipedRight;

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();
        }
#endif

        public void TriggerEquiped(ActingEquipmentEntity equiper, bool rightHand)
        {
            equipedRight = rightHand;
            TriggerEquiped(equiper);
        }

        public bool EquipedWithRightHand { get { return equipedRight; } }
    }
}
