using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class SmartTriggerSwitch : MonoBehaviour
{

    //State describing
    [SerializeField]
    bool _switchState;
    [SerializeField]
    bool _switchLocked;

    //Restrictors
    [SerializeField]
    bool _keepOff;
    [SerializeField]
    float _keepOffCooldown;
    [SerializeField]
    bool _keepOn;
    [SerializeField]
    float _keepOnCooldown;

    //Activators
    [SerializeField]
    SwitchActivatorGroup[] _activatorGroups;

    //Event Handeling
    [SerializeField]
    UnityEvent _switchedOn;
    [SerializeField]
    UnityEvent _switchedOff;

    public delegate void OnSwitchedOn();
    public delegate void OnSwitchedOff();
    public event OnSwitchedOn SwitchedOnHandler;
    public event OnSwitchedOff SwitchedOffHandler;

    public bool IsSwitchOn { get { return _switchState; } }

    List<GameObject> _cachedGameObjectInTrigger;

    void Awake()
    {
        _cachedGameObjectInTrigger = new List<GameObject>(2);
#if UNITY_EDITOR
        Collider col = GetComponent<Collider>();
        Collider2D col2D = GetComponent<Collider2D>();

        if (col == null && col2D == null)
            Debug.LogWarning(name + ": Switch has no collider attached and will thus never change its state.");
#endif
        ReconsiderSwitchState();
    }

    void OnDrawGizmos()
    {
        if (_switchLocked)
            Gizmos.DrawIcon(transform.position, _switchState ? "SmartSwitch/SmartSwitchOn_Locked.png" : "SmartSwitch/SmartSwitchOff_Locked.png", true);
        else
            Gizmos.DrawIcon(transform.position, _switchState ? "SmartSwitch/SmartSwitchOn.png" : "SmartSwitch/SmartSwitchOff.png", true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int iTarget = 0; iTarget < _switchedOn.GetPersistentEventCount(); iTarget++)
        {
            var target = _switchedOn.GetPersistentTarget(iTarget);
            var gameObj = target as GameObject;
            if (gameObj == null)
            {
                var comp = target as Component;
                if (comp == null)
                    return;
                Gizmos.DrawLine(transform.position, comp.transform.position);
            }
            Gizmos.DrawLine(transform.position, gameObj.transform.position);
        }

        Gizmos.color = Color.red;
        for (int iTarget = 0; iTarget < _switchedOff.GetPersistentEventCount(); iTarget++)
        {
            var target = _switchedOff.GetPersistentTarget(iTarget);
            var gameObj = target as GameObject;
            if (gameObj == null)
            {
                var comp = target as Component;
                if (comp == null)
                    return;
                Gizmos.DrawLine(transform.position, comp.transform.position);
            }
            Gizmos.DrawLine(transform.position, gameObj.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Assert(!_cachedGameObjectInTrigger.Contains(col.gameObject));
        _cachedGameObjectInTrigger.Add(col.gameObject);
        ReconsiderSwitchState(null);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Assert(_cachedGameObjectInTrigger.Contains(col.gameObject));
        _cachedGameObjectInTrigger.Remove(col.gameObject);
        ReconsiderSwitchState(col.gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Assert(!_cachedGameObjectInTrigger.Contains(col.gameObject));
        _cachedGameObjectInTrigger.Add(col.gameObject);
        ReconsiderSwitchState(null);
    }

    void OnTriggerExit(Collider col)
    {
        Debug.Assert(_cachedGameObjectInTrigger.Contains(col.gameObject));
        _cachedGameObjectInTrigger.Remove(col.gameObject);
        ReconsiderSwitchState(col.gameObject);
    }

    void ReconsiderSwitchState(GameObject leftObj = null)
    {
        if (_switchLocked)
            return;
        ReconsiderSwitchStateNoChecks(leftObj);
    }

    void ReconsiderSwitchStateNoKeep()
    {
        if (_switchLocked)
            return;

        CleanCache();
        for (int iGroup = 0; iGroup < _activatorGroups.Length; iGroup++)
        {
            if (_activatorGroups[iGroup].FullfillsCondition(_cachedGameObjectInTrigger, null))
            {
                if (!_switchState)
                    FlipSwitchOnNoChecks();
                return;
            }
        }
        if (_switchState)
            FlipSwitchOffNoChecks();
    }

    void ReconsiderSwitchStateNoChecks(GameObject leftObj = null)
    {
        CleanCache();
        for (int iGroup = 0; iGroup < _activatorGroups.Length; iGroup++)
        {
            if (_activatorGroups[iGroup].FullfillsCondition(_cachedGameObjectInTrigger, leftObj))
            {
                FlipSwitchOn();
                return;
            }
        }
        FlipSwitchOff();
    }

    void CleanCache()
    {
        for (int iObj = 0; iObj < _cachedGameObjectInTrigger.Count; iObj++)
        {
            if (_cachedGameObjectInTrigger[iObj] == null) //it was destroyed!
            {
                Debug.LogWarning("An object was destroyed in switch " + name + ". Could lead to bad behaivoir and shouldn't be allowed to happen!");
                _cachedGameObjectInTrigger.RemoveAt(iObj);
                iObj--;
            }

        }
    }

    public void FlipSwitchOn()
    {
        if (_switchState)
            return; //Already On

        if (_keepOff)
        {
            if (_keepOffCooldown > 0)
            {
                StartCoroutine(DelayActionForSeconds(_keepOffCooldown, ReconsiderSwitchStateNoKeep));
            }
            return;
        }
        FlipSwitchOnNoChecks();
    }

    public void FlipSwitchOff()
    {
        if (!_switchState)
            return; //Already Off

        if (_keepOn)
        {
            if (_keepOnCooldown > 0)
            {
                StartCoroutine(DelayActionForSeconds(_keepOnCooldown, ReconsiderSwitchStateNoKeep));
            }
            return;
        }
        FlipSwitchOffNoChecks();
    }

    void FlipSwitchOffNoChecks()
    {
        _switchState = false;
        _switchedOff.Invoke();
        if (SwitchedOffHandler != null)
            SwitchedOffHandler.Invoke();
    }

    void FlipSwitchOnNoChecks()
    {
        _switchState = true;
        _switchedOn.Invoke();
        if (SwitchedOnHandler != null)
            SwitchedOnHandler.Invoke();
    }

    delegate void DelayedAction();
    IEnumerator DelayActionForSeconds(float seconds, DelayedAction action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
}

interface ISwitchActivation
{
    bool FullfillsCondition(GameObject target, bool left);
}

[System.Serializable]
class SwitchActivatorGroup
{
    [SerializeField]
    SwitchActivator[] _activators;

    public bool FullfillsCondition(IEnumerable<GameObject> targets, GameObject leftObj)
    {
        for (int i = 0; i < _activators.Length; i++)
        {
            if (leftObj != null)
            {
                if (_activators[i].FullfillsCondition(leftObj, true))
                    goto NEXTACTIVATOR;
            }
            foreach (GameObject obj in targets)
            {
                if (_activators[i].FullfillsCondition(obj, false))
                    goto NEXTACTIVATOR;
            }

            return false;

            NEXTACTIVATOR:
            continue;
        }
        return true;
    }
}

[System.Serializable]
public class SwitchActivator : ISerializationCallbackReceiver, ISwitchActivation
{
    ISwitchActivation switchActivation;
    [SerializeField]
    bool _inverseCondition;

    //Only for serialization
    [SerializeField]
    SwitchActivateByGameObjectRecognition _activateGameObjectRec;
    [SerializeField]
    SwitchActivateByLayer _activateLayer;
    [SerializeField]
    SwitchActivateByTag _activateTag;
    [SerializeField]
    SwitchActivateByActor _activateActor;
    [SerializeField]
    SwitchActivateByItem _activateItem;
    [SerializeField]
    SwitchActivateByKey _activateKey;
    [SerializeField]
    int _activeActivator;

    public bool FullfillsCondition(GameObject target, bool left)
    {
        return switchActivation.FullfillsCondition(target, left) ^ _inverseCondition;
    }

    public void OnBeforeSerialize()
    {
        if (switchActivation == null)
            return;

        Type type = switchActivation.GetType();

        if (type == typeof(SwitchActivateByGameObjectRecognition))
        {
            _activateGameObjectRec = (SwitchActivateByGameObjectRecognition)switchActivation;
            _activeActivator = 0;
        }
        else if (type == typeof(SwitchActivateByLayer))
        {
            _activateLayer = (SwitchActivateByLayer)switchActivation;
            _activeActivator = 1;
        }
        else if (type == typeof(SwitchActivateByTag))
        {
            _activateTag = (SwitchActivateByTag)switchActivation;
            _activeActivator = 2;
        }
        else if (type == typeof(SwitchActivateByActor))
        {
            _activateActor = (SwitchActivateByActor)switchActivation;
            _activeActivator = 3;
        }
        else if (type == typeof(SwitchActivateByItem))
        {
            _activateItem = (SwitchActivateByItem)switchActivation;
            _activeActivator = 4;
        }
        else if (type == typeof(SwitchActivateByKey))
        {
            _activateKey = (SwitchActivateByKey)switchActivation;
            _activeActivator = 5;
        }
    }

    public void OnAfterDeserialize()
    {
        if (_activeActivator == 0)
            switchActivation = _activateGameObjectRec;
        else if (_activeActivator == 1)
            switchActivation = _activateLayer;
        else if (_activeActivator == 2)
            switchActivation = _activateTag;
        else if (_activeActivator == 3)
            switchActivation = _activateActor;
        else if (_activeActivator == 4)
            switchActivation = _activateItem;
        else if (_activeActivator == 5)
            switchActivation = _activateKey;

        _activateGameObjectRec = null;
        _activateLayer = null;
        _activateTag = null;
        _activateActor = null;
        _activateItem = null;
        _activateKey = null;
    }
}

[System.Serializable]
public class SwitchActivateByGameObjectRecognition : ISwitchActivation
{
    [SerializeField]
    GameObject _objectToRecognize;
    [SerializeField]
    bool _onLeave;

    public bool FullfillsCondition(GameObject target, bool left)
    {
        if (_onLeave != left)
            return false;
        return _objectToRecognize.Equals(target);
    }
}

[System.Serializable]
public class SwitchActivateByLayer : ISwitchActivation
{
    [SerializeField]
    LayerMask _layer;
    [SerializeField]
    bool _onLeave;

    public bool FullfillsCondition(GameObject target, bool left)
    {
        if (_onLeave != left)
            return false;
        return Utility.ExtensionMethods.ExtensionMethods.IsLayerWithinMask(_layer, target.layer);
    }
}

[System.Serializable]
public class SwitchActivateByTag : ISwitchActivation
{
    [SerializeField]
    string _tag;
    [SerializeField]
    bool _onLeave;

    public bool FullfillsCondition(GameObject target, bool left)
    {
        if (_onLeave != left)
            return false;
        return target.CompareTag(_tag);
    }
}

[System.Serializable]
public class SwitchActivateByActor : ISwitchActivation, ISerializationCallbackReceiver
{
    [SerializeField]
    int staticActorTypeIndex;
    [SerializeField]
    bool _onLeave;

    Type _ActorType;

    public bool FullfillsCondition(GameObject target, bool left)
    {
        if (_onLeave != left)
            return false;
        Entities.Entity[] actors = target.GetComponentsInChildren<Entities.Entity>();
        for (int iActor = 0; iActor < actors.Length; iActor++)
        {
            if (actors[iActor].GetType().Equals(_ActorType))
                return true;
        }
        return false;
    }

    public void OnAfterDeserialize()
    {
        _ActorType = Entities.Entity.EntitySubtypes[staticActorTypeIndex];
    }

    public void OnBeforeSerialize()
    {
        if (_ActorType == null)
        {
            staticActorTypeIndex = 0;
            return;
        }
        staticActorTypeIndex = Entities.Entity.EntityTypeToStaticIndex(_ActorType);
        if (staticActorTypeIndex == -1)
            staticActorTypeIndex = 0;
    }
}

[System.Serializable]
public class SwitchActivateByItem : ISwitchActivation
{
    [SerializeField]
    ItemHandler.ItemData item;
    [SerializeField]
    bool _onLeave;

    public bool FullfillsCondition(GameObject target, bool left)
    {
        if (_onLeave != left)
            return false;
        IInventoryEntity actor = target.GetComponentInChildren<IInventoryEntity>();
        if (actor == null)
            return false;
        return actor.Inventory.ContainsItem(item.itemID);
    }
}

[System.Serializable]
public class SwitchActivateByKey : ISwitchActivation
{
    public enum ButtonPressType
    {
        Hold,
        Down,
        Up
    }

    [SerializeField]
    InteractiveInputDefinition interactiveInputDef;
    [SerializeField]
    ButtonPressType pressType;
    [SerializeField]
    SmartTriggerSwitch switchCallback;

    bool initDone;

    public bool FullfillsCondition(GameObject target, bool left)
    {
        if (left)
        {
            Entities.ActingEntity actor = target.GetComponentInChildren<Entities.ActingEntity>();
            if (actor == null)
                return false;
            actor.InteractiveInputHandler.RemoveInputListener(interactiveInputDef);
        }
        else
        {
            Entities.ActingEntity actor = target.GetComponentInChildren<Entities.ActingEntity>();
            if (actor == null)
                return false;
            if (!initDone)
            {
                if (pressType == ButtonPressType.Hold)
                    interactiveInputDef.onButton = OnButton;
                else if (pressType == ButtonPressType.Up)
                    interactiveInputDef.onButtonUp = OnButton;
                else if (pressType == ButtonPressType.Down)
                    interactiveInputDef.onButtonDown = OnButton;
                initDone = true;
            }
            actor.InteractiveInputHandler.AddInputListener(interactiveInputDef);
        }
        return false;
    }

    void OnButton(Entities.ActingEntity actor)
    {
        switchCallback.FlipSwitchOn();
    }
}
