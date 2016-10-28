using UnityEngine;
using System.Collections;
using Actors;

public class PlayerInteractiveInputHandler : InteractiveInputHandler
{
    public bool blockAllInput = false;
    InteractiveInputUIMarker.UIInputItem[] itemQueue;

    public PlayerInteractiveInputHandler(PlayerActor actor) : base(actor)
    {
        UnityEventHog.GetInstance().AddUpdateListener(Update);
        itemQueue = actor.InteractiveInputUIMarker.uiItemQueue;
    }

    InteractiveInputDefinition currentDef;
    void Update()
    {
        if (blockAllInput)
            return;

        for (int i = 0; i < inputListener.Count; i++)
        {
            currentDef = inputListener[i];
            if (Input.GetButtonDown(currentDef.button))
            {
                if(currentDef.onButtonDown != null)
                currentDef.onButtonDown(actor);
            }
            else if (Input.GetButton(currentDef.button))
            {
                if (currentDef.onButton != null)
                    currentDef.onButton(actor);
            }
            else if (Input.GetButtonUp(currentDef.button))
            {
                if (currentDef.onButtonUp != null)
                    currentDef.onButtonUp(actor);
            }
        }
    }

    public override void AddInputListener(InteractiveInputDefinition def)
    {
        base.AddInputListener(def);

        if (inputListener.Count > itemQueue.Length)
            return;
        itemQueue[inputListener.Count - 1].UpdateContent(def);
        itemQueue[inputListener.Count - 1].SetVisible(true);
    }

    public override void RemoveInputListener(InteractiveInputDefinition def)
    {
        base.RemoveInputListener(def);

        if (inputListener.Count >= itemQueue.Length)
            return;
        itemQueue[inputListener.Count - 1].SetVisible(false);
    }
}
