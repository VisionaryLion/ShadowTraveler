using UnityEngine;
using System.Collections;
using Entities;
using System.Collections.Generic;
using System;

public class InteractiveInputHandler {

    protected List<InteractiveInputDefinition> inputListener;
    protected ActingEntity actor;

    public InteractiveInputHandler(ActingEntity actor)
    {
        this.actor = actor;
        inputListener = new List<InteractiveInputDefinition>();
    }

    public virtual void AddInputListener(InteractiveInputDefinition def)
    {
        inputListener.Add(def);
    }

    public virtual void RemoveInputListener(InteractiveInputDefinition def)
    {
        inputListener.Remove(def);
    }
}


