using UnityEngine;
using Utility;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    public class BulletHoleDefinition : ScriptableObject
    {

        [SerializeField]
        BulletHitEffect[] sprites;

        public BulletHitEffect GetBulletHitEffect(SurfaceTypes.SurfaceType surfaceType)
        {
            int typIndex = (int)surfaceType;
            if (typIndex >= sprites.Length || sprites[typIndex].Length == 0)
            {
                typIndex = (int)SurfaceTypes.SurfaceType.Unknown;
                return sprites[typIndex];
            }
            return sprites[typIndex];
        }

        public void ResizeOrCreateSprites()
        {
            if (sprites == null || sprites.Length == 0)
            {
                sprites = new BulletHitEffect[SurfaceTypes.SurfaceTypeCount];
                for (int i = 0; i < SurfaceTypes.SurfaceTypeCount; i++)
                {
                    sprites[i] = new BulletHitEffect();
                }
            }
            else if (sprites.Length != SurfaceTypes.SurfaceTypeCount)
            {
                BulletHitEffect[] tmpStor = OribowsUtilitys.DeepCopy(sprites);
                sprites = new BulletHitEffect[SurfaceTypes.SurfaceTypeCount];
                int smallerSize = Mathf.Min(SurfaceTypes.SurfaceTypeCount, tmpStor.Length);
                for (int i = 0; i < smallerSize; i++)
                {
                    sprites[i] = tmpStor[i];
                }
                for (int i = smallerSize; i < SurfaceTypes.SurfaceTypeCount; i++)
                {
                    sprites[i] = new BulletHitEffect();
                }
            }
        }
    }
    [System.Serializable]
    public class BulletHitEffect
    {
        [SerializeField]
        Decal[] decals = new Decal[0];
        [SerializeField]
        public GameObject effects;

        public Decal this[int index]
        {
            get
            {
                return decals[index];
            }
        }

        public Decal GetRandomDecal()
        {
            return decals[Random.Range(0, decals.Length)];
        }

        public int Length
        {
            get
            {
                return decals.Length;
            }
        }
    }
}
