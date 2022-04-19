#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.UltimateLODSystem
{
    /*
     This class is responsible for the functioning of the "Ultimate Level Of Detail Meshes" component, and all its functions.
    */
    /*
     * The Ultimate LOD System was developed by Marcos Tomaz in 2020.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class UltimateLevelOfDetailMeshes : MonoBehaviour
    {
        //Public variables
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public UltimateLevelOfDetail responsibleUlod = null;
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public int idOfOriginalMeshItemOfThisInResponsibleUlod = -1;

#if UNITY_EDITOR
        //Public variables of Interface
        private bool gizmosOfThisComponentIsDisabled = false;
        [HideInInspector]
        private bool[] ulodMaterialsForEachLodFolded = new bool[] { false, false, false, false, false, false, false, false, false }; //9

        //Editor auto update on change meshes variables
        [HideInInspector]
        [SerializeField]
        private Mesh[] lastLods = new Mesh[9];

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(UltimateLevelOfDetailMeshes))]
        public class CustomInspector : UnityEditor.Editor
        {
            //Private variables of Editor Only
            private Vector2 gameObjectsToIgnoreScrollpos = Vector2.zero;
            private Vector2 gameObjectsFoundInLastScanScrollpos = Vector2.zero;
            private Vector2 ulodsListScrollpos = Vector2.zero;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                UltimateLevelOfDetailMeshes script = (UltimateLevelOfDetailMeshes)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");
                script.gizmosOfThisComponentIsDisabled = MTAssetsEditorUi.DisableGizmosInSceneView("UltimateLevelOfDetailMeshes", script.gizmosOfThisComponentIsDisabled);

                //If responsible ULOD not exists
                if (script.responsibleUlod == null || script.idOfOriginalMeshItemOfThisInResponsibleUlod == -1)
                {
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("It was not possible to find the ULOD component responsible for managing the LODs of this network. Apparently it has been deleted or no longer exists.", MessageType.Error);
                    GUILayout.Space(10);
                    if (GUILayout.Button("Delete This Component"))
                        DestroyImmediate(script);
                    GUILayout.Space(10);
                }
                //If responsible ULOD exists
                if (script.responsibleUlod != null && script.idOfOriginalMeshItemOfThisInResponsibleUlod != -1)
                {
                    GUILayout.Space(10);

                    GUIStyle titulo = new GUIStyle();
                    titulo.fontSize = 16;
                    titulo.normal.textColor = new Color(0, 79.0f / 250.0f, 3.0f / 250.0f);
                    titulo.alignment = TextAnchor.MiddleCenter;
                    EditorGUILayout.LabelField("This Mesh Have a LOD Group With " + script.responsibleUlod.levelsOfDetailToGenerate + " Meshes", titulo);

                    GUILayout.Space(10);

                    //About
                    EditorGUILayout.HelpBox("If you need to change the original mesh of a renderer, the most correct way to do this is to change the original mesh here in this component. That way the Ultimate LOD System will know that the new original mesh is the one you want, and then render it at the right times. You can define the new original mesh here in this component, or by C# API of this component.", MessageType.Info);

                    //Meshes
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("LOD Meshes For This Mesh", EditorStyles.boldLabel);
                    GUILayout.Space(10);

                    //Prepare string sufix for each mesh
                    string lodSuffix = " [Rendering]";
                    if (script.responsibleUlod.isScannedMeshesCurrentCulled() == true)
                        lodSuffix = "";

                    //Get id of this originalMeshItem
                    int id = script.idOfOriginalMeshItemOfThisInResponsibleUlod;

                    //Show meshes
                    for (int i = 0; i < 9; i++)
                    {
                        if (i > script.responsibleUlod.levelsOfDetailToGenerate)
                            continue;

                        script.responsibleUlod.currentScannedMeshesList[id].allMeshLods[i] = (Mesh)EditorGUILayout.ObjectField(new GUIContent("LOD " + i.ToString() + (script.responsibleUlod.GetCurrentLodLevel() == i ? lodSuffix : ""),
                                                    "Mesh file that will be rendered in Original Mesh."),
                                                    script.responsibleUlod.currentScannedMeshesList[id].allMeshLods[i], typeof(Mesh), true, GUILayout.Height(16));
                    }

                    GUILayout.Space(20);

                    //Restore all default meshes, with path saved
                    if (GUILayout.Button("Restore All Original Generated Meshes"))
                    {
                        bool problems = TryToRestoreAllOriginalGeneratedLodLevels(script, id);
                        if (problems == false)
                            Debug.Log("All standard LOD meshes, which were generated by ULOD, have been restored in this mesh.");
                        if (problems == true)
                            EditorUtility.DisplayDialog("Error", "It was not possible to load 1 or more LOD mesh files generated by ULOD, 1 or more mesh files missing in your project.", "Ok");
                    }
                    if (GUILayout.Button("Copy LOD 1 And Set As Default On LOD 0"))
                    {
                        script.responsibleUlod.currentScannedMeshesList[id].allMeshLods[0] = script.responsibleUlod.currentScannedMeshesList[id].allMeshLods[1];
                    }

                    //Materials
                    if (script.responsibleUlod.enableMaterialsChanges == true)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("LOD Materials For This Mesh", EditorStyles.boldLabel);
                        GUILayout.Space(10);

                        for (int i = 0; i < 9; i++)
                        {
                            if (i > script.responsibleUlod.levelsOfDetailToGenerate || script.responsibleUlod.currentScannedMeshesList[id].canChangeMaterialsOnThisMeshLods == false)
                                continue;

                            script.ulodMaterialsForEachLodFolded[i] = EditorGUILayout.Foldout(script.ulodMaterialsForEachLodFolded[i], (script.ulodMaterialsForEachLodFolded[i] == true ? "Hide Materials Of LOD " + i.ToString() : "Show Materials Of LOD " + i.ToString()));
                            if (script.ulodMaterialsForEachLodFolded[i] == true)
                            {
                                for (int x = 0; x < script.responsibleUlod.currentScannedMeshesList[id].allMeshLodsMaterials[i].materialArray.Length; x++)
                                {
                                    EditorGUILayout.IntField(new GUIContent("Size", "The size of this material array level."), script.responsibleUlod.currentScannedMeshesList[id].allMeshLodsMaterials[i].materialArray.Length);
                                    script.responsibleUlod.currentScannedMeshesList[id].allMeshLodsMaterials[i].materialArray[x] = (Material)EditorGUILayout.ObjectField(new GUIContent("Material " + x,
                                            "Material " + x + " of this material array of this level."),
                                            script.responsibleUlod.currentScannedMeshesList[id].allMeshLodsMaterials[i].materialArray[x], typeof(Material), true, GUILayout.Height(16));
                                }

                                //Render controls to manipulate material array
                                if (i < script.responsibleUlod.levelsOfDetailToGenerate)
                                    if (GUILayout.Button("Copy Materials To Next LOD"))
                                        for (int x = 0; x < script.responsibleUlod.currentScannedMeshesList[id].allMeshLodsMaterials[i].materialArray.Length; x++)
                                        {
                                            script.responsibleUlod.currentScannedMeshesList[id].allMeshLodsMaterials[i + 1].materialArray[x] = script.responsibleUlod.currentScannedMeshesList[id].allMeshLodsMaterials[i].materialArray[x];
                                            Debug.Log("The materials from this material matrix were copied to the material matrix of the next LOD.");
                                        }
                            }
                        }

                        GUILayout.Space(20);
                    }

                    //Run auto force update on change a mesh
                    AutoForceUpdateOfLodsOnChangeMesh(script);

                    //Go to responsible ULOD
                    if (GUILayout.Button("Go To Responsible ULOD Component"))
                        Selection.objects = new Object[] { script.responsibleUlod.gameObject };

                    //Force render update
                    if (GUILayout.Button("Force ULOD To Update LODs Renderization"))
                        script.responsibleUlod.ForceThisComponentToUpdateLodsRender();
                }

                //Apply changes on script, case is not playing in editor
                if (GUI.changed == true && Application.isPlaying == false)
                {
                    EditorUtility.SetDirty(script);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
                }
                if (EditorGUI.EndChangeCheck() == true)
                {

                }
            }

            public void AutoForceUpdateOfLodsOnChangeMesh(UltimateLevelOfDetailMeshes script)
            {
                bool force = false;

                //Check all LODs
                for (int i = 0; i < script.lastLods.Length; i++)
                    if (script.lastLods[i] != script.responsibleUlod.currentScannedMeshesList[script.idOfOriginalMeshItemOfThisInResponsibleUlod].allMeshLods[i])
                    {
                        force = true;
                        script.lastLods[i] = script.responsibleUlod.currentScannedMeshesList[script.idOfOriginalMeshItemOfThisInResponsibleUlod].allMeshLods[i];
                    }

                //Force if have changes
                if (force == true)
                {
                    script.responsibleUlod.ForceThisComponentToUpdateLodsRender();

                    //Notify the user if option "Editor Skinned LODs Change" is off
                    SkinnedMeshRenderer smr = script.GetComponent<SkinnedMeshRenderer>();
                    if (smr != null && script.responsibleUlod.forceChangeLodsOfSkinnedInEditor == false)
                        Debug.LogWarning("You just made a Skinned mesh change to an \"Ultimate Level Of Detail Meshes\" component. The mesh change will not be displayed because the \"Editor Skinned LODs Change\" option is disabled, in the parent \"Ultimate Level Of Detail\" component, which prevents the LOD simulations of the Ultimate LOD System from occurring in the Editor, however, the LOD changes will occur in the final game, compiled and built, without any problems. If you want to see these simulations in the Editor too, just activate the \"Editor Skinned LODs Change\" option in the \"Ultimate Level Of Detail\" component. Consult the documentation for more details.");
                }
            }

            public bool TryToRestoreAllOriginalGeneratedLodLevels(UltimateLevelOfDetailMeshes script, int id)
            {
                //Try to restore all original meshes by the path, and return if have problems
                bool problems = false;

                for (int i = 0; i < 9; i++)
                {
                    if (i > script.responsibleUlod.levelsOfDetailToGenerate)
                        continue;

                    script.responsibleUlod.currentScannedMeshesList[id].allMeshLods[i] = (Mesh)AssetDatabase.LoadAssetAtPath(script.responsibleUlod.currentScannedMeshesList[id].allMeshLodsPaths[i], typeof(Mesh));
                    if (script.responsibleUlod.currentScannedMeshesList[id].allMeshLods[i] == null)
                        problems = true;
                }

                return problems;
            }
        }
        #endregion
#endif
        //API Methods

        public UltimateLevelOfDetail GetResponsibleUlodComponent()
        {
            //Return the responsible ulod component
            return responsibleUlod;
        }

        public int GetQuantityOfLods()
        {
            //Return the quantity of LODs that this LOD group have
            return responsibleUlod.levelsOfDetailToGenerate;
        }

        public void SetMeshOfThisLodGroup(int level, Mesh newMesh)
        {
            //Check if is a valid level
            if (level < 0 || level > 8)
            {
                Debug.LogError("It was not possible to define a new mesh in this LOD group, the level informed is invalid.");
                return;
            }

            //Set a new mesh for a level of this LOD group
            responsibleUlod.currentScannedMeshesList[idOfOriginalMeshItemOfThisInResponsibleUlod].allMeshLods[level] = newMesh;

            //Update the renderization
            responsibleUlod.ForceThisComponentToUpdateLodsRender();
        }

        public Mesh GetMeshOfThisLodGroup(int level)
        {
            //Check if is a valid level
            if (level < 0 || level > 8)
            {
                Debug.LogError("It was not possible to get mesh of desired level, the level informed is invalid.");
                return null;
            }

            //Set a new mesh for a level of this LOD group
            return responsibleUlod.currentScannedMeshesList[idOfOriginalMeshItemOfThisInResponsibleUlod].allMeshLods[level];
        }

        public bool isMaterialChangesEnabledForThisMesh()
        {
            //Return true, if materials changes are allowed for this mesh
            if (responsibleUlod.enableMaterialsChanges == true)
                return true;
            if (responsibleUlod.enableMaterialsChanges == false)
                return false;
            return false;
        }

        public void SetMaterialArrayOfThisLodGroup(int level, Material[] newMaterialArray)
        {
            //If materials change is not allowed
            if (isMaterialChangesEnabledForThisMesh() == false)
            {
                Debug.LogError("It is not possible to supply or obtain a material array for an LOD of this mesh. Material change is disabled for this mesh and the Ultimate Level Of Detail component that manages it.");
                return;
            }

            //Check if is a valid level
            if (level < 0 || level > 8)
            {
                Debug.LogError("It was not possible to define a new material array in this LOD group, the level informed is invalid.");
                return;
            }

            //Set a new mesh for a level of this LOD group
            responsibleUlod.currentScannedMeshesList[idOfOriginalMeshItemOfThisInResponsibleUlod].allMeshLodsMaterials[level].materialArray = newMaterialArray;

            //Update the renderization
            responsibleUlod.ForceThisComponentToUpdateLodsRender();
        }

        public Material[] GetMaterialArrayOfThisLodGroup(int level)
        {
            //If materials change is not allowed
            if (isMaterialChangesEnabledForThisMesh() == false)
            {
                Debug.LogError("It is not possible to supply or obtain a material array for an LOD of this mesh. Material change is disabled for this mesh and the Ultimate Level Of Detail component that manages it.");
                return null;
            }

            //Check if is a valid level
            if (level < 0 || level > 8)
            {
                Debug.LogError("It was not possible to get mesh of desired level, the level informed is invalid.");
                return null;
            }

            //Set a new mesh for a level of this LOD group
            return responsibleUlod.currentScannedMeshesList[idOfOriginalMeshItemOfThisInResponsibleUlod].allMeshLodsMaterials[level].materialArray;
        }
    }
}