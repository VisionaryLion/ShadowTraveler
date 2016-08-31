// - AUTHOR : Oribow

using UnityEngine;
namespace SurfaceTypeUser
{
    public class FootstepManager : MonoBehaviour
    {
        [SerializeField]
        FootstepsDefinition footstepDefinition;

        [Tooltip("This is used to determine what distance has to be traveled in order to play the footstep sound.")]
        [SerializeField]
        float distanceBetweenSteps = 1.8f;

        [Tooltip("You need an audio source to play a footstep sound.")]
        [SerializeField]
        AudioSource audioSource;

        // Random volume between this limits
        [SerializeField]
        float minVolume = 0.3f;
        [SerializeField]
        float maxVolume = 0.5f;

        float stepCycleProgress;
        float lastPlayTime;

        public bool AdvanceStepCycle(float speed)
        {
            stepCycleProgress += speed * Time.deltaTime;

            if (stepCycleProgress > distanceBetweenSteps)
            {
                stepCycleProgress = 0f;
                return true;
            }
            return false;
        }

        public void PlayFootsteps(SurfaceTypes.SurfaceType ground)
        {
            AudioClip randomFootstep = footstepDefinition.FindAudioClip(ground);
            float randomVolume = Random.Range(minVolume, maxVolume);

            if (randomFootstep)
            {
                audioSource.PlayOneShot(randomFootstep, randomVolume);
            }
        }

        public void PlayLandingClip(SurfaceTypes.SurfaceType ground)
        {
            audioSource.PlayOneShot(footstepDefinition.FindAudioClip(ground));
        }
    }
}
