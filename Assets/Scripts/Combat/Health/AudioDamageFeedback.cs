using UnityEngine;

/*
Author: Oribow
*/
namespace Combat
{
    public class AudioDamageFeedback : MonoBehaviour
    {
        [SerializeField]
        IHealth health;
        [SerializeField]
        AudioSource audioSource;
        [SerializeField]
        DamageFeedbackDefinition dmgDef;
        [SerializeField]
        bool waitTillClipFinished;

        void Start()
        {
            health.OnHealthChanged += Health_OnHealthChanged;
        }

        private void Health_OnHealthChanged(object sender, IDamageInfo e)
        {
            if(!waitTillClipFinished || !audioSource.isPlaying)
            audioSource.PlayOneShot(dmgDef.FindAudioClip(e.DmgTyp));
        }
    }
}
