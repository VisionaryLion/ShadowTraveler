using UnityEngine;
using System.Collections;
using Actors;

public class InteractiveInputDefinition : ScriptableObject
{
    public delegate void InputCallback(BasicEntityActor actor);

    public string button;
    public string description;
    public Sprite icon;

    public InputCallback onButtonDown;
    public InputCallback onButtonUp;
    public InputCallback onButton;
}
