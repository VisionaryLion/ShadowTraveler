using UnityEngine;
using Actors;

/*
Author: Oribow
*/
namespace Combat
{
    public class DamageFeedback : MonoBehaviour
    {
        [AssignActorAutomaticly]
        PlayerActor actor;

        [SerializeField]
        DamageFeedbackDefinition damgeFeedbackDef;
        [SerializeField]
        string hitAnimName;
        [SerializeField]
        bool waitTillClipFinished;

        void Start()
        {
            actor.BasicHealth.OnHealthChanged += Health_OnHealthChanged;
        }

        private void Health_OnHealthChanged(object sender, IDamageInfo e)
        {
            if (actor.BasicHealth.IsDeath)
            {
                actor.BasicHealth.OnHealthChanged -= Health_OnHealthChanged;
                return;
            }
            //if (!waitTillClipFinished || !actor.AudioSource.isPlaying)
            //    actor.AudioSource.PlayOneShot(damgeFeedbackDef.FindAudioClip(e.DmgTyp));
            if (!actor.CC2DMotor.frontAnimator.GetCurrentAnimatorStateInfo(0).IsName(hitAnimName) && !actor.PlayerLimitationHandler.AreAnimationTriggerLocked())
                actor.CC2DMotor.frontAnimator.SetTrigger("TakeDamage");
        }
    }
}
