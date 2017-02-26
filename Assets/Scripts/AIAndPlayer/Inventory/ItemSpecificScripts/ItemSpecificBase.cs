using UnityEngine;
using System.Collections;
using Entities;
using System;

public class ItemSpecificBase : MonoBehaviour {

    [SerializeField, AssignEntityAutomaticly, HideInInspector]
    protected TwoHandItemEntity entity;

    protected ActingEquipmentEntity equipedEntity;

    // Use this for initialization
    protected virtual void Start () {
        entity.EquipedHandler += Entity_EquipedHandler;
        entity.UnequipedHandler += Entity_UnequipedHandler;
        enabled = false;
    }

    protected virtual void Entity_UnequipedHandler()
    {
        enabled = false;
    }

    protected virtual void Entity_EquipedHandler(ActingEquipmentEntity equiper)
    {
        enabled = true;
        equipedEntity = equiper;
    }

    protected bool IsOnPlayer()
    {
        return equipedEntity.GetType().IsAssignableFrom(typeof(PlayerEntity));
    }
}
