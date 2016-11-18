﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class EquipmentOffset {

    [SerializeField]
    Offset[] offsets;


    public Vector2 GetOffsetPos(bool rightHand, int itemId)
    {
        foreach (Offset o in offsets)
        {
            if (o.itemId == itemId)
            {
                if (rightHand)
                    return o.offsetPosRight;
                return o.offsetPosLeft;
            }
        }
        Debug.LogWarning("No offset defined for Item with id = "+itemId);
        return Vector2.zero;
    }

    public float GetOffsetRot(bool rightHand, int itemId)
    {
        foreach (Offset o in offsets)
        {
            if (o.itemId == itemId)
            {
                if (rightHand)
                    return o.offsetRotRight;
                return o.offsetRotLeft;
            }
        }
        Debug.LogWarning("No offset defined for Item with id = " + itemId);
        return 0;
    }

    [System.Serializable]
    public class Offset
    {
        public int itemId;
        public Vector2 offsetPosRight;
        public float offsetRotRight;
        public Vector2 offsetPosLeft;
        public float offsetRotLeft;
    }
}
