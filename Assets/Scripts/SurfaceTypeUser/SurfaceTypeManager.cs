using System.Collections.Generic;
using UnityEngine;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    public class SurfaceTypeManager : MonoBehaviour
    {
        [SerializeField]
        SurfaceTypes surfaceTypes;
        private static Dictionary<Texture, SurfaceTypes.SurfaceType> sDefinitions;

        void Awake()
        {
            sDefinitions = new Dictionary<Texture, SurfaceTypes.SurfaceType>(surfaceTypes.items.Length);
            for (int i = 0; i < surfaceTypes.items.Length; i++)
            {
                sDefinitions.Add(surfaceTypes.items[i].texture, surfaceTypes.items[i].surfaceType);
            }
            surfaceTypes = null;
        }

        ///<summary>
        ///Try to find the surface type of the collider. If nothing could be found SurfaceTyp.Unknown is returned.
        /// </summary>
        public static SurfaceTypes.SurfaceType GetSurfaceType(Collider surface, Vector3 hitPoint)
        {
            if (surface == null)
            {
                return SurfaceTypes.SurfaceType.Unknown;
            }
            //Handle terrain diffrent
            if (surface.GetType() == typeof(TerrainCollider))
            {
                return GetTerrainSurfaceType(surface, hitPoint, surface.transform.position);
            }
            else
            {
                return GetMeshSurfaceTyp(surface);
            }
        }

        private static SurfaceTypes.SurfaceType GetTerrainSurfaceType(Collider surface, Vector3 hitPoint, Vector3 surfPos)
        {
            TerrainData terrainData = surface.GetComponentInChildren<Terrain>(false).terrainData;
            // calculate which splat map cell the worldPos falls within (ignoring y)
            int mapX = (int)(((hitPoint.x - surfPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int mapZ = (int)(((hitPoint.z - surfPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
            //check for out of bounds coordinates
            if (mapX > terrainData.alphamapWidth || mapZ > terrainData.alphamapHeight)
            {
                Debug.LogError("The given coordinates " + hitPoint + " is out of bounds for the terrain. Returning \"Unknown\"");
                return SurfaceTypes.SurfaceType.Unknown;
            }
            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            //find the heigest weighted texture
            int highestWeightedTexIndex = 0;
            float comp = 0f;
            for (int i = 1; i < splatmapData.GetLength(2); i++)
            {
                if (comp < splatmapData[0, 0, i])
                    highestWeightedTexIndex = i;
            }
            return TryGetValueOrDefault(terrainData.alphamapTextures[highestWeightedTexIndex]);
        }

        private static SurfaceTypes.SurfaceType GetMeshSurfaceTyp(Collider surface)
        {
            Renderer renderer = surface.GetComponentInChildren<Renderer>(false);
            if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null)
            {
                Debug.LogWarning("Couldnt find surface typ for " + surface.name + ", because there are no textures attached!");
                return SurfaceTypes.SurfaceType.Unknown;
            }
            return TryGetValueOrDefault(renderer.sharedMaterial.mainTexture);
        }

        private static SurfaceTypes.SurfaceType TryGetValueOrDefault (Texture val)
        {
            SurfaceTypes.SurfaceType result;
            if (sDefinitions.TryGetValue(val, out result))
            {
                return result;
            }
            return SurfaceTypes.SurfaceType.Unknown;
        }
    }
}


