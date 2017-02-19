using UnityEngine;
using System.Collections;
using Entities;

public class InteractiveInputDefinition : ScriptableObject
{
    public delegate void InputCallback(ActingEntity actor);

    public string button;
    public string description;
    public Sprite icon;

    public InputCallback onButtonDown;
    public InputCallback onButtonUp;
    public InputCallback onButton;
}
