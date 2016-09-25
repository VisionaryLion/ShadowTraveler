using UnityEngine;
using System.Collections;
using Actors;

public class PlayerInteractiveInputHandler : InteractiveInputHandler
{
    public bool blockAllInput = false;

    public PlayerInteractiveInputHandler(BasicEntityActor actor) : base(actor)
    {
        UnityEventHog.GetInstance().AddUpdateListener(Update);
    }

    InteractiveInputDefintion currentDef;
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

}
