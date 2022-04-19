using UnityEngine;
using UnityEditor;
using System.IO;

namespace MTAssets.UltimateLODSystem.Editor
{

    /*
     * This class is responsible for creating the menu for this asset. 
     */

    public class Menu : MonoBehaviour
    {
        //Components context click menu items

        [MenuItem("CONTEXT/UltimateLevelOfDetail/Convert This To Unity LODs", false, 0)]
        static void ForceUpdateCanvasOfMinimapRenderer(MenuCommand command)
        {
            //Get the component reference
            UltimateLevelOfDetail ulod = (UltimateLevelOfDetail)command.context;

            //If not have a scan, ask to make a scan first
            if (ulod.isMeshesCurrentScannedAndLodsWorkingInThisComponent() == false)
            {
                EditorUtility.DisplayDialog("Do a scan before!", "To convert to Unity LODs, please do a scan using this Ultimate Level Of Detail component first.", "Ok");
                return;
            }

            //Display the warning
            if (EditorUtility.DisplayDialog("Are you sure you want to continue?", "The Ultimate LOD System will convert all the Meshes that have been scanned by this component, to the standard Unity LODs system, that is, all the scanned meshes will no longer be managed by this \"Ultimate Level Of Detail\" component and will be managed only by the \"LOD Group\" components Unity standards.\n\nThe low quality meshes that have been generated, will be maintained. New GameObjects will also be created for each scanned mesh, as Unity's LOD Groups depend on it.\n\nPlease also note that it is not possible to convert distance information from ULOD to Unity LODs, so this information will be recalculated for Unity.\n\nThis action CANNOT be undone. Do you wish to continue?", "Yes", "No") == false)
                return;

            //Do conversion
            ulod.ConvertThisToDefaultUnityLods();
            AssetDatabase.Refresh();
        }

        //Right click menu items

        [MenuItem("Assets/Open With Mesh Simplifier", false, 30)]
        static void OpenMeshSimplifierToolWithHierarchyNormal()
        {
            UltimateMeshSimplifier.OpenWindow(null, Selection.activeObject as Mesh);
        }

        [MenuItem("Assets/Open With Mesh Simplifier", true)]
        static bool OpenMeshSimplifierToolWithHierarchyValidation()
        {
            //Validate if selected item is a mesh
            if (Selection.objects.Length == 1)
                return Selection.activeObject is Mesh;
            if (Selection.objects.Length > 1)
                return false;
            return false;
        }

        [MenuItem("GameObject/Simplify Mesh", false, 30)]
        static void GoSimplifierToolWithHierarchyNormal()
        {
            UltimateMeshSimplifier.OpenWindow(Selection.activeObject as GameObject, null);
        }

        //Menu items

        [MenuItem("Tools/MT Assets/Ultimate LOD System/Mesh Simplifier Tool", false, 5)]
        static void OpenMeshSimplifierTool()
        {
            UltimateMeshSimplifier.OpenWindow(null, null);
        }

        [MenuItem("Tools/MT Assets/Ultimate LOD System/Editor LODs Simulation/Enable In This Scene", false, 10)]
        static void EnableLodsInThisScene()
        {
            //Show progress
            EditorUtility.DisplayProgressBar("Enabling LODs Simulation", "A moment....", 1.0f);

            //Enable the LODs simulation in all ULOD components
            UltimateLevelOfDetail[] ulods = Object.FindObjectsOfType<UltimateLevelOfDetail>();
            foreach (UltimateLevelOfDetail ulod in ulods)
            {
                if (ulod.gameObject == null)
                    continue;
                ulod.isLodSimulationEnabledInThisSceneForEditorSceneViewMode = true;
                ulod.ForceThisComponentToUpdateLodsRender();
            }
            Debug.Log("All \"Ultimate Level Of Detail\" components of this scene had their LOD simulation enabled. Components that already have simulation enabled will continue to function normally.");

            //Clear progress
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Tools/MT Assets/Ultimate LOD System/Editor LODs Simulation/Disable In This Scene", false, 10)]
        static void DisableLodsInThisScene()
        {
            //Cancel if user request
            if (EditorUtility.DisplayDialog("Continue?", "You are about to disable LOD simulation for all \"Ultimate Level Of Detail\" components currently in this scene. The simulation will continue to function normally when the game is run, it will only stop working in the scene view mode. This can be useful for generating Lightmaps accurately and etc.\n\nWould you like to continue disabling LOD simulation in this scene?", "Yes", "No") == false)
                return;

            //Show progress
            EditorUtility.DisplayProgressBar("Enabling LODs Simulation", "A moment....", 1.0f);

            //Disable the LODs simulation in all ULOD components
            UltimateLevelOfDetail[] ulods = Object.FindObjectsOfType<UltimateLevelOfDetail>();
            foreach (UltimateLevelOfDetail ulod in ulods)
            {
                if (ulod.gameObject == null)
                    continue;
                ulod.isLodSimulationEnabledInThisSceneForEditorSceneViewMode = false;
                ulod.ForceThisComponentToUpdateLodsRender();
            }
            Debug.Log("All \"Ultimate Level Of Detail\" components of this scene had their LOD simulation disabled. Even if they currently have a scan ready, LODs will not appear until you activate the simulation again. This only applies in edit mode, all components \"Ultimate Level Of Detail\" that have a scan, will work normally ingame, even if the simulation of LODs is disabled in this scene. If you add other \"Ultimate Level Of Detail\" components and want to keep them with the LOD simulation disabled, you will need to do this step again.");

            //Clear progress
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Tools/MT Assets/Ultimate LOD System/View Changelog", false, 15)]
        static void OpenChangeLog()
        {
            string filePath = Greetings.pathForThisAsset + "/List Of Changes.txt";

            if (File.Exists(filePath) == true)
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(filePath, typeof(TextAsset)));

            if (File.Exists(filePath) == false)
                EditorUtility.DisplayDialog(
                    "Error",
                    "Unable to open file. The file has been deleted, or moved. Please, to correct this problem and avoid future problems with this tool, remove the directory from this asset and install it again.",
                    "Ok");
        }

        [MenuItem("Tools/MT Assets/Ultimate LOD System/Read Documentation", false, 30)]
        static void ReadDocumentation()
        {
            EditorUtility.DisplayDialog(
                  "Read Documentation",
                  "The Documentation HTML file will open in your default application.",
                  "Cool!");

            string filePath = Greetings.pathForThisAsset + Greetings.pathForThisAssetDocumentation;

            if (File.Exists(filePath) == true)
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(filePath, typeof(TextAsset)));

            if (File.Exists(filePath) == false)
                EditorUtility.DisplayDialog(
                    "Error",
                    "Unable to open file. The file has been deleted, or moved. Please, to correct this problem and avoid future problems with this tool, remove the directory from this asset and install it again.",
                    "Ok");
        }

        [MenuItem("Tools/MT Assets/Ultimate LOD System/More Assets", false, 30)]
        static void MoreAssets()
        {
            Help.BrowseURL(Greetings.linkForAssetStorePage);
        }

        [MenuItem("Tools/MT Assets/Ultimate LOD System/Get Support", false, 30)]
        static void GetSupport()
        {
            EditorUtility.DisplayDialog(
                "Support",
                "If you have any questions, problems or want to contact me, just contact me by email (mtassets@windsoft.xyz).",
                "Got it!");
        }
    }
}