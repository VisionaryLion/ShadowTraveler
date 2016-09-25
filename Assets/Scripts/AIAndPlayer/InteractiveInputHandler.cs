using UnityEngine;
using System.Collections;
using Actors;
using System.Collections.Generic;
using System;

public class InteractiveInputHandler {

    protected List<InteractiveInputDefintion> inputListener;
    protected BasicEntityActor actor;

    public InteractiveInputHandler(BasicEntityActor actor)
    {
        this.actor = actor;
        inputListener = new List<InteractiveInputDefintion>();
    }

    public void AddInputListener(InteractiveInputDefintion def)
    {
        inputListener.Add(def);
    }

    public void RemoveInputListener(InteractiveInputDefintion def)
    {
        inputListener.Remove(def);
    }
}

[Serializable]
public class InteractiveInputDefintion
{
    public delegate void InputCallback(BasicEntityActor actor);

    public string button;
    public string description;
    public Texture icon;

    public InputCallback onButtonDown;
    public InputCallback onButtonUp;
    public InputCallback onButton;
}
