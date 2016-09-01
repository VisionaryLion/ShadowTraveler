using UnityEngine;
using System.Collections;
using Actors;

public class PlayerAnimationEventGrabberFront : MonoBehaviour {

    public delegate void OnPickUpAnimnReachedItem();
    public event OnPickUpAnimnReachedItem PickUpReachedItemHandler;

   
    void OnPickUpAnimationReachedItem()
    {
        if (PickUpReachedItemHandler != null)
            PickUpReachedItemHandler.Invoke();
    }
}
