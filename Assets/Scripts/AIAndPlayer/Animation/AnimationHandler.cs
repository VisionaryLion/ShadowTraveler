using UnityEngine;
using Entity;
using System;
using System.Collections.Generic;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField]
    [AssignEntityAutomaticly]
    [HideInInspector]
    AnimationEntity actor;

    public delegate void AnimationEvent();

    AnimationEventListener firstEventListener;
    AnimationEventListener firstEndListener;

    void Awake ()
    {
        AnimationEndHandler[] endHandler = actor.Animator.GetBehaviours<AnimationEndHandler>();
        for (int iHandler = 0; iHandler < endHandler.Length; iHandler++)
        {
            endHandler[iHandler].AnimationEndEventHandler += AnimationHandler_AnimationEndEventHandler;
        }
    }

    private void AnimationHandler_AnimationEndEventHandler(AnimatorStateInfo animatorStateInfo)
    {
        if (firstEndListener == null)
            return;
        AnimationEventListener currentListener = firstEndListener;
        AnimationEventListener prevListener = null;
        List<AnimationEvent> bufferedEvents = new List<AnimationEvent>();
        do
        {
            if (animatorStateInfo.IsName(currentListener.identifier))
            {
                bufferedEvents.Add(currentListener.callback);
                if (prevListener == null)
                    firstEndListener = currentListener.next;
                else
                    prevListener.next = currentListener.next;
                currentListener.callback = null;
                currentListener.next = null;
                currentListener.identifier = null;
            }
            prevListener = currentListener;
        } while ((currentListener = currentListener.next) != null);
        foreach (AnimationEvent ae in bufferedEvents)
        {
            ae.Invoke();
        }
    }

    public int GetAnyStateTransitionPriority(int layer)
    {
        return actor.Animator.GetInteger("AnyStatePriority" + layer);
    }

    public void SetAnyStateTransitionPriority(int layer, int priority)
    {
        actor.Animator.SetInteger("AnyStatePriority" + layer, Mathf.Max(priority, actor.Animator.GetInteger("AnyStatePriority" + layer)));
    }

    public void ResetAnyStateTransitionPriority(int layer)
    {
        actor.Animator.SetInteger("AnyStatePriority" + layer, 0);
    }

    public bool CanAquireAnyStateTransitionPriority(int layer, int priority)
    {
        return GetAnyStateTransitionPriority(layer) <= priority;
    }

    public void StartListenToAnimationEnd(string name, AnimationEvent callback)
    {
        AnimationEventListener newListener = new AnimationEventListener(name, callback);
        newListener.next = firstEndListener;
        firstEndListener = newListener;
    }

    public void StartListenToAnimationEvent(string identifier, AnimationEvent callback)
    {
        AnimationEventListener newListener = new AnimationEventListener(identifier, callback);
        newListener.next = firstEventListener;
        firstEventListener = newListener;
    }

    public void StopListenToAnimationEnd(AnimationEvent callback)
    {
        AnimationEventListener currentListener = firstEndListener;
        AnimationEventListener prevListener = null;
        do
        {
            if (currentListener.callback == callback)
            {
                if (prevListener == null)
                    firstEndListener = currentListener.next;
                else
                    prevListener.next = currentListener.next;
                currentListener.callback = null;
                currentListener.next = null;
                currentListener.identifier = null;
                return;
            }
            prevListener = currentListener;
        } while ((currentListener = currentListener.next) != null);

        Debug.LogError("You tried to remove an animationEventListener that doesnt exist!");
    }

    public void StopListenToAnimationEvent(AnimationEvent callback)
    {
        AnimationEventListener currentListener = firstEventListener;
        AnimationEventListener prevListener = null;
        do
        {
            if (currentListener.callback == callback)
            {
                if (prevListener == null)
                    firstEventListener = currentListener.next;
                else
                    prevListener.next = currentListener.next;
                currentListener.callback = null;
                currentListener.next = null;
                currentListener.identifier = null;
                return;
            }
            prevListener = currentListener;
        } while ((currentListener = currentListener.next) != null);

        Debug.LogError("You tried to remove an animationEventListener that doesnt exist!");
    }

    public void AnimationEventOccured(string identifier)
    {
        if (firstEventListener == null)
            return;
        AnimationEventListener currentListener = firstEventListener;
        AnimationEventListener prevListener = null;
        List<AnimationEvent> bufferedEvents = new List<AnimationEvent>();
        do
        {
            if (currentListener.identifier == identifier)
            {
                bufferedEvents.Add(currentListener.callback);
                if (prevListener == null)
                    firstEventListener = currentListener.next;
                else
                    prevListener.next = currentListener.next;
                currentListener.callback = null;
                currentListener.next = null;
                currentListener.identifier = null;
            }
            prevListener = currentListener;
        } while ((currentListener = currentListener.next) != null);

        foreach (AnimationEvent ae in bufferedEvents)
        {
            ae.Invoke();
        }

        if (bufferedEvents.Count == 0)
        Debug.LogWarning("The animationEvent with the identifier = \""+identifier+"\" happend, without a listener!");
    }

    class AnimationEventListener
    {
        public string identifier;
        public AnimationEvent callback;
        public AnimationEventListener next;

        public AnimationEventListener(string identifier, AnimationEvent callback)
        {
            this.identifier = identifier;
            this.callback = callback;
        }
    }
}
