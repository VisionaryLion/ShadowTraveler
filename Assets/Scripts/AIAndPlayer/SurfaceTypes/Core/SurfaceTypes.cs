using UnityEngine;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    //Add new surface typs here!
    [System.Serializable]
    public class SurfaceTypes : ScriptableObject
    {
        public SurfaceTypeItem[] items;

        //All surface types have to be predetermind here, for coding convinience.
        public enum SurfaceType
        {
            Unknown,
            Wood,
            Concret,
            Dirt,
            Metal
        }
        //translate the enum here
        public const string surfaceType_Wood = "wood";
        public const string surfaceType_Concrete = "concrete";
        public const string surfaceType_Dirt = "dirt";
        public const string surfaceType_Metal = "metal";
        public const string surfaceType_Unknown = "unknown";

        //How much surface types there are.
        public const int SurfaceTypeCount = 5;

        public static SurfaceType StringToSurfaceType(string val)
        {
            switch (val)
            {
                case surfaceType_Wood:
                    return SurfaceType.Wood;
                case surfaceType_Concrete:
                    return SurfaceType.Concret;
                case surfaceType_Dirt:
                    return SurfaceType.Dirt;
                case surfaceType_Metal:
                    return SurfaceType.Metal;
                default:
                    return SurfaceType.Unknown;
            }
        }

        public static string SurfaceTypeToString(SurfaceType val)
        {
            switch (val)
            {
                case SurfaceType.Wood:
                    return surfaceType_Wood;
                case SurfaceType.Concret:
                    return surfaceType_Concrete;
                case SurfaceType.Dirt:
                    return surfaceType_Dirt;
                case SurfaceType.Metal:
                    return surfaceType_Metal;
                default:
                    return surfaceType_Unknown;
            }
        }

        public static string SurfaceTypeToString(int val)
        {
            return SurfaceTypeToString((SurfaceType)val);
        }
    }

    [System.Serializable]
    public class SurfaceTypeItem
    {
        [SerializeField]
        public Texture texture;
        [SerializeField]
        public SurfaceTypes.SurfaceType surfaceType;

        public SurfaceTypeItem()
        {
            texture = null;
            surfaceType = SurfaceTypes.SurfaceType.Unknown;
        }

        public SurfaceTypeItem(Texture texture, SurfaceTypes.SurfaceType surfType)
        {
            this.texture = texture;
            surfaceType = surfType;
        }
    }
}
