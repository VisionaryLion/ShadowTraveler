using UnityEngine;
using Utility;

/*
Author: Oribow
*/
namespace Combat
{
    [System.Serializable]
    public class DamageFeedbackDefinition : ScriptableObject
    {
        [SerializeField]
        AudioContainer[] audioClips;

        public AudioClip FindAudioClip(IDamageInfo.DamageTyp damageTyp, int stepIndex)
        {
            int typIndex = (int)damageTyp;
            if (typIndex >= audioClips.Length || typIndex < 0)
                return null;
            if (stepIndex >= audioClips[typIndex].audioClips.Length || stepIndex < 0)
            {
                Debug.LogError("No audioclip assigned to  " + damageTyp + ". Returning NULL");
                return null;
            }
            return audioClips[typIndex][stepIndex];
        }

        public AudioClip FindAudioClip(IDamageInfo.DamageTyp damageTyp)
        {
            return FindAudioClip(damageTyp, Random.Range(0, audioClips[(int)damageTyp].audioClips.Length));
        }

        public void ResizeOrCreateAudioClips()
        {
            if (audioClips == null || audioClips.Length == 0)
            {
                audioClips = new AudioContainer[IDamageInfo.DamageTypCount];
                for (int i = 0; i < IDamageInfo.DamageTypCount; i++)
                {
                    audioClips[i] = new AudioContainer();
                }
            }
            else if (audioClips.Length != IDamageInfo.DamageTypCount)
            {
                AudioContainer[] tmpStor = OribowsUtilitys.DeepCopy(audioClips);
                audioClips = new AudioContainer[IDamageInfo.DamageTypCount];
                int smallerSize = Mathf.Min(IDamageInfo.DamageTypCount, tmpStor.Length);
                for (int i = 0; i < smallerSize; i++)
                {
                    audioClips[i] = new AudioContainer();
                    if (tmpStor[i] != null)
                    {
                        audioClips[i].audioClips = new AudioClip[tmpStor[i].audioClips.Length];
                        for (int k = 0; k < tmpStor[i].audioClips.Length; k++)
                        {
                            audioClips[i][k] = tmpStor[i][k];
                        }
                    }
                }
                for (int i = smallerSize; i < IDamageInfo.DamageTypCount; i++)
                {
                    audioClips[i] = new AudioContainer();
                }
            }
        }

        public AudioContainer this[int key]
        {
            get
            {
                return audioClips[key];
            }
            set
            {
                audioClips[key] = value;
            }
        }

        //hack for unitys serialization system
        [System.Serializable]
        public class AudioContainer
        {
            [SerializeField]
            public AudioClip[] audioClips = new AudioClip[0];

            public AudioClip this[int key]
            {
                get
                {
                    return audioClips[key];
                }
                set
                {
                    audioClips[key] = value;
                }
            }
        }
    }
}
