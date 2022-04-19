using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.UltimateLODSystem
{
    /*
     This class is responsible for global management of all ULOD components
    */
    /*
     * The Ultimate LOD System was developed by Marcos Tomaz in 2020.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class UltimateLevelOfDetailGlobal : MonoBehaviour
    {
        //Private static variables
        private static bool enableGlobalUlodSystem = true;
        private static float lodDistanceMultiplier = 1.0f;

        //Public static variables
        public static Camera currentCameraThatIsOnTopOfScreenInThisScene = null;

        //Public and static methods

        public static bool isGlobalULodSystemEnabled()
        {
            //Return if the lod auto management is enabled
            return enableGlobalUlodSystem;
        }

        public static void EnableGlobalULodSystem(bool enable)
        {
            //Enable or disable the lod auto management by ULOD
            enableGlobalUlodSystem = enable;
        }

        public static void SetGlobalLodDistanceMultiplier(float multiplier)
        {
            //Set a new LOD distance multiplier
            lodDistanceMultiplier = multiplier;
        }

        public static float GetGlobalLodDistanceMultiplier()
        {
            //Return the global lod multiplier
            return lodDistanceMultiplier;
        }

        public static Mesh GetSimplifiedVersionOfThisMesh(Mesh meshToSimplify, bool isSkinnedMesh, bool skinnedAnimsCompatibilityMode, bool simplificationDestroyerMode, bool preventArtifacts, float percentOfVerticesOfSimplifyiedVersion)
        {
            //Simplification multiplier
            float multiplier = 0.00001f;

            //Return the mesh converted to LOD
            MeshSimplifier.MeshSimplifier meshSimplifier = new MeshSimplifier.MeshSimplifier();
            MeshSimplifier.SimplificationOptions meshSimplificationSettings = new MeshSimplifier.SimplificationOptions();
            if (simplificationDestroyerMode == false)
            {
                meshSimplificationSettings.Agressiveness = 7.0f;
                multiplier = 1.0f;
            }
            if (simplificationDestroyerMode == true)
            {
                meshSimplificationSettings.Agressiveness = 12.0f;
                multiplier = 0.4f;
            }
            if (preventArtifacts == true)
                meshSimplificationSettings.EnableSmartLink = true;
            if (preventArtifacts == false)
                meshSimplificationSettings.EnableSmartLink = false;
            meshSimplificationSettings.MaxIterationCount = 100;
            meshSimplificationSettings.PreserveBorderEdges = false;
            meshSimplificationSettings.PreserveSurfaceCurvature = false;
            meshSimplificationSettings.PreserveUVFoldoverEdges = false;
            meshSimplificationSettings.PreserveUVSeamEdges = false;
            meshSimplificationSettings.VertexLinkDistance = double.Epsilon;
            meshSimplifier.SimplificationOptions = meshSimplificationSettings;
            meshSimplifier.Initialize(meshToSimplify);
            meshSimplifier.SimplifyMesh((percentOfVerticesOfSimplifyiedVersion / 100.0f) * multiplier);
            Mesh resultMesh = meshSimplifier.ToMesh();
            if (isSkinnedMesh == true && skinnedAnimsCompatibilityMode == true)
                resultMesh.bindposes = meshToSimplify.bindposes;
            return resultMesh;
        }
    }
}