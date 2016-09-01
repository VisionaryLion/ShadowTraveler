using UnityEngine;
using System.Collections;
using Actors;

public class PlayerAnimationEventGrabberFront : MonoBehaviour {

    public delegate void OnDeathAnimFinished ();
    public event OnDeathAnimFinished DeathAnimFinishedHandler;

    public delegate void OnPickUpAnimFinished();
    public event OnPickUpAnimFinished PickUpFinishedHandler;

    public delegate void OnPickUpAnimnReachedItem();
    public event OnPickUpAnimnReachedItem PickUpReachedItemHandler;

    void OnDeathAnimationFinished()
    {
        if (DeathAnimFinishedHandler != null)
            DeathAnimFinishedHandler.Invoke();
    }

    void OnPickUpAnimationFinished()
    {
        if (PickUpFinishedHandler != null)
            PickUpFinishedHandler.Invoke();
    }

    void OnPickUpAnimationReachedItem()
    {
        if (PickUpReachedItemHandler != null)
            PickUpReachedItemHandler.Invoke();
    }
}
