using UnityEngine;
using Utility;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    [System.Serializable]
    public class FootstepsDefinition : ScriptableObject
    {
        [SerializeField]
        AudioContainer[] audioClips;

        public AudioClip FindAudioClip(SurfaceTypes.SurfaceType surfaceType, int stepIndex)
        {
            int typIndex = (int)surfaceType;
            if (typIndex >= audioClips.Length || typIndex < 0 || stepIndex >= audioClips[typIndex].Length || stepIndex < 0)
            {
                typIndex = (int)SurfaceTypes.SurfaceType.Unknown;
                if (stepIndex >= audioClips[typIndex].Length || stepIndex < 0)
                {
                    Debug.LogError ("No audioclip assigned to  " + surfaceType + ". Returning NULL");
                    return null;
                }
                Debug.LogError("No audioclip could be found for " + surfaceType + ". Returning AudioClip for Unknown");
                return audioClips[typIndex][stepIndex];
            }
            return audioClips[typIndex][stepIndex];
        }

        public AudioClip FindAudioClip(SurfaceTypes.SurfaceType surfaceType)
        {
            return FindAudioClip(surfaceType, Random.Range(0, audioClips[(int)surfaceType].Length));
        }

        public void ResizeOrCreateAudioClips()
        {
            if (audioClips == null || audioClips.Length == 0)
            {
                audioClips = new AudioContainer[SurfaceTypes.SurfaceTypeCount];
                for (int i = 0; i < SurfaceTypes.SurfaceTypeCount; i++)
                {
                    audioClips[i] = new AudioContainer();
                }
            }
            else if (audioClips.Length != SurfaceTypes.SurfaceTypeCount)
            {
                AudioContainer[] tmpStor = OribowsUtilitys.DeepCopy(audioClips);
                audioClips = new AudioContainer[SurfaceTypes.SurfaceTypeCount];
                int smallerSize = Mathf.Min(SurfaceTypes.SurfaceTypeCount, tmpStor.Length);
                for (int i = 0; i < smallerSize; i++)
                {
                    audioClips[i] = new AudioContainer();
                    if (tmpStor[i] != null)
                    {
                        audioClips[i].audioClips = new AudioClip[tmpStor[i].Length];
                        for (int k = 0; k < tmpStor[i].Length; k++)
                        {
                            audioClips[i][k] = tmpStor[i][k];
                        }
                    }
                }
                for (int i = smallerSize; i < SurfaceTypes.SurfaceTypeCount; i++)
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

            public int Length
            {
                get { return audioClips.Length; }
            }
        }
    }
}
