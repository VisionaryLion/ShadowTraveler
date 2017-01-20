using UnityEngine;
using Actors;

/*
Author: Oribow
*/
namespace Combat
{
    public class DamageFeedback : MonoBehaviour
    {
        [AssignActorAutomaticly, SerializeField, HideInInspector]
        BasicEntityActor actor;

        [SerializeField]
        DamageFeedbackDefinition damgeFeedbackDef;
        [SerializeField]
        string hitAnimName;
        [SerializeField]
        bool waitTillClipFinished;

        void Start()
        {
            actor.IHealth.OnHealthChanged += Health_OnHealthChanged;
        }

        private void Health_OnHealthChanged(object sender, IDamageInfo e)
        {
            if (actor.IHealth.IsDeath)
            {
                actor.IHealth.OnHealthChanged -= Health_OnHealthChanged;
                return;
            }
            //if (!waitTillClipFinished || !actor.AudioSource.isPlaying)
            //    actor.AudioSource.PlayOneShot(damgeFeedbackDef.FindAudioClip(e.DmgTyp));
            if (!actor.Animator.GetCurrentAnimatorStateInfo(0).IsName(hitAnimName))
                actor.Animator.SetTrigger("TakeDamage");
        }
    }
}
