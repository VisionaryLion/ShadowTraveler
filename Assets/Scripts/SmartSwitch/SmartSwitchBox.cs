using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SmartSwitchBox : MonoBehaviour
{

    [SerializeField]
    bool switchState;
    [SerializeField]
    bool isLocked;

    [SerializeField]
    bool _keepOff;
    [SerializeField]
    float _keepOffCooldown;
    [SerializeField]
    bool _keepOn;
    [SerializeField]
    float _keepOnCooldown;

    [SerializeField]
    SmartTriggerSwitch[] _connectedSwitches;
    [SerializeField]
    bool _allHaveToBeOn;

    [SerializeField]
    UnityEvent _switchedOn;
    [SerializeField]
    UnityEvent _switchedOff;

    int switchesOn;

    void Awake()
    {
        foreach (var sSwitch in _connectedSwitches)
        {
            sSwitch.SwitchedOnHandler += OnSwitchedOn;
            sSwitch.SwitchedOffHandler += OnSwitchedOff;
            if (sSwitch.IsSwitchOn)
            {
                switchesOn++;
            }
        }
        ReconsiderState();
    }

    void OnDrawGizmos()
    {
        if (switchState)
            Gizmos.DrawIcon(transform.position, isLocked ? "SmartSwitch/SmartSwitchBoxOn_Locked.png" : "SmartSwitch/SmartSwitchBoxOn.png", true);
        else
            Gizmos.DrawIcon(transform.position, isLocked ? "SmartSwitch/SmartSwitchBoxOff_Locked.png" : "SmartSwitch/SmartSwitchBoxOff.png", true);

        Gizmos.color = Color.white;
        foreach (var sSwitch in _connectedSwitches)
        {
            if (sSwitch != null)
                Gizmos.DrawLine(transform.position, sSwitch.transform.position);
        }
    }

    void OnSwitchedOn()
    {
        switchesOn++;
        ReconsiderState();
    }

    void OnSwitchedOff()
    {
        switchesOn--;
        ReconsiderState();
    }

    void ReconsiderState()
    {
        if (isLocked)
            return;

        if (!_allHaveToBeOn && switchesOn > 0)
        {
            TurnOn();
        }
        else if (_allHaveToBeOn && switchesOn == _connectedSwitches.Length)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    }

    void ReconsiderStateNoKeep()
    {
        if (isLocked)
            return;

        if (!_allHaveToBeOn && switchesOn > 0)
        {
            NoCheckTurnOn();
        }
        else if (_allHaveToBeOn && switchesOn == _connectedSwitches.Length)
        {
            NoCheckTurnOn();
        }
        else
        {
            NoCheckTurnOff();
        }
    }

    void TurnOn()
    {
        if (switchState)
            return;

        if (_keepOff)
        {
            if (_keepOffCooldown > 0)
            {
                StartCoroutine(DelayActionForSeconds(_keepOffCooldown, ReconsiderStateNoKeep));
            }
            return;
        }

        NoCheckTurnOn();
    }

    void NoCheckTurnOn()
    {
        switchState = true;
        _switchedOn.Invoke();
    }

    void TurnOff()
    {
        if (!switchState)
            return;

        if (_keepOn)
        {
            if (_keepOnCooldown > 0)
            {
                StartCoroutine(DelayActionForSeconds(_keepOnCooldown, ReconsiderStateNoKeep));
            }
            return;
        }

        NoCheckTurnOff();
    }

    void NoCheckTurnOff()
    {
        switchState = false;
        _switchedOff.Invoke();
    }

    delegate void DelayedAction();
    IEnumerator DelayActionForSeconds(float seconds, DelayedAction action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
}
