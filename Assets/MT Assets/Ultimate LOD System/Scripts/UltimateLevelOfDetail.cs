#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using UnityEngine.Events;

namespace MTAssets.UltimateLODSystem
{
    /*
     This class is responsible for the functioning of the "Ultimate Level Of Detail" component, and all its functions.
    */
    /*
     * The Ultimate LOD System was developed by Marcos Tomaz in 2020.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Ultimate LOD System/Ultimate Level Of Detail")] //Add this component in a category of addComponent menu
    [ExecuteInEditMode]
    public class UltimateLevelOfDetail : MonoBehaviour
    {
        //Private constants of script
        private WaitForEndOfFrame WAIT_FOR_END_OF_FRAME = new WaitForEndOfFrame();

        //Caches of script
#if UNITY_EDITOR
        private Camera cacheOfLastActiveSceneViewCamera = null;
#endif
        private Camera cacheOfMainCamera = null;
        private GameObject cacheOfUlodData = null;
        private RuntimeInstancesDetector cacheOfUlodDataRuntimeInstancesDetector = null;

        //Private variables from script
        private float lastDistanceFromMainCamera = -1f;
        private int currentLodAccordingToDistance = -1;
        private float currentDistanceFromMainCamera = 0f;
        private float currentRealDistanceFromMainCamera = 0f;
        private bool forcedToDisableLodsOfThisComponent = false;

        //Classes of script
        public enum ScanMeshesMode
        {
            ScanInChildrenGameObjectsOnly,
            ScanInThisGameObjectOnly
        }
        public enum ForceOfSimplification
        {
            Normal,
            Strong,
            VeryStrong,
            ExtremelyStrong,
            Destroyer
        }
        public enum CullingMode
        {
            Disabled,
            CullingMeshes,
            CullingRenderer
        }
        public enum CameraDetectionMode
        {
            CurrentCamera,
            MainCamera,
            CustomCamera
        }
        [System.Serializable]
        public class ScannedMeshItem
        {
            [System.Serializable]
            public class MeshMaterials
            {
                //Class that stores a material array for a lod level
                public Material[] materialArray = new Material[0];
            }

            //Original mesh and simplifications info
            public GameObject originalGameObject;
            public SkinnedMeshRenderer originalSkinnedMeshRenderer;
            public MeshFilter originalMeshFilter;
            public MeshRenderer originalMeshRenderer;
            public Mesh[] allMeshLods = new Mesh[9];
            public string[] allMeshLodsPaths = new string[9];
            public bool canChangeMaterialsOnThisMeshLods = false;
            public MeshMaterials[] allMeshLodsMaterials = new MeshMaterials[9];
            public UltimateLevelOfDetailMeshes originalMeshLodsManager;

            //Before mesh culling info
            public Mesh beforeCullingData_lastMeshOfThis = null;
            public bool beforeCullingData_isForcedToRenderizationOff = false;

            //Methods of scanned mesh item
            public void InitializeAllMeshLodsMaterialsArray()
            {
                //This method instantiate the MeshMaterials object in all elements of array of materials, for each lod

                for (int i = 0; i < 9; i++)
                    if (allMeshLodsMaterials[i] != null)
                        return;

                for (int i = 0; i < 9; i++)
                    allMeshLodsMaterials[i] = new MeshMaterials();
            }
        }

        //Current scanned meshes list (database)
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<ScannedMeshItem> currentScannedMeshesList = new List<ScannedMeshItem>();

        //Scan settings
        [HideInInspector]
        public ScanMeshesMode modeOfMeshesScanning = ScanMeshesMode.ScanInChildrenGameObjectsOnly;
        [HideInInspector]
        public bool scanInactiveGameObjects = false;

        //Meshes to ignore settings
        [HideInInspector]
        public List<GameObject> gameObjectsToIgnore = new List<GameObject>();

        //LOD settings
        [HideInInspector]
        public int levelsOfDetailToGenerate = 3;
        [HideInInspector]
        public float[] percentOfVerticesForEachLod = new float[] { 100.0f, 75.0f, 65.0f, 45.0f, 35.0f, 25.0f, 15.0f, 10.0f, 5.0f }; //9
        [HideInInspector]
        public bool saveGeneratedLodsInAssets = true;
        [HideInInspector]
        public bool skinnedAnimsCompatibilityMode = true;
        [HideInInspector]
        public bool preventArtifacts = true;
        [HideInInspector]
        public bool optimizeResultingMeshes = false;
        [HideInInspector]
        public bool enableLightmapsSupport = false;
        [HideInInspector]
        public bool enableMaterialsChanges = false;
        [HideInInspector]
        public ForceOfSimplification forceOfSimplification = ForceOfSimplification.Normal;
        [HideInInspector]
        public CullingMode cullingMode = CullingMode.CullingMeshes;
        [HideInInspector]
        [SerializeField]
        private Transform _customPivotToSimulateLods = null;

        //Distance of view for each LOD
        [HideInInspector]
        public CameraDetectionMode cameraDetectionMode = CameraDetectionMode.CurrentCamera;
        [HideInInspector]
        public bool useCacheForMainCameraInDetection = true;
        [HideInInspector]
        public Camera customCameraForSimulationOfLods = null;
        [HideInInspector]
        public float[] minDistanceOfViewForEachLod = new float[] { 0.0f, 30.0f, 70.0f, 120.0f, 150.0f, 180.0f, 200.0f, 220.0f, 250.0f }; //9
        [HideInInspector]
        public float minDistanceOfViewForCull = 270f;

        //Event exclusive for runtime scan
        public UnityEvent onDoneScan;
        public UnityEvent onUndoScan;

        //Debug settings
        [HideInInspector]
        public bool forceChangeLodsOfSkinnedInEditor = false;
        [HideInInspector]
        public bool drawGizmoOnThisPivot = false;
        [HideInInspector]
        public Color colorOfGizmo = Color.blue;
        [HideInInspector]
        public float sizeOfGizmo = 0.2f;
        [HideInInspector]
        public bool forceShowHiddenSettings = false;

        //Getters to verify and validate needed variables
        public Transform customPivotToSimulateLods
        {
            get
            {
                return _customPivotToSimulateLods;
            }
            set
            {
                if (value == null)
                {
                    _customPivotToSimulateLods = null;
                    return;
                }
                if (value.IsChildOf(this.gameObject.transform) == true)
                    _customPivotToSimulateLods = value;
                else
                    Debug.LogError("We were unable to define a custom pivot. Make sure that the GameObject that will be the new personalized pivot is the child of the desired ULOD component.");
            }
        }

#if UNITY_EDITOR
        //Public variables of Interface
        private bool gizmosOfThisComponentIsDisabled = false;
        [HideInInspector]
        [SerializeField]
        private bool showScanEventsOptions = false;
        [HideInInspector]
        [SerializeField]
        private float[] last_minDistanceOfViewForEachLod = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }; //9
        [HideInInspector]
        [SerializeField]
        private float last_minDistanceOfViewForCull = 0f;
        [HideInInspector]
        [SerializeField]
        private bool setupRunned = false;

        //LODs simulation enabled in this scene, for editor scene view mode only
        ///<summary>[WARNING] This variable is only available in the Editor and will not be included in the compilation of your project, in the final Build.</summary> 
        [HideInInspector]
        public bool isLodSimulationEnabledInThisSceneForEditorSceneViewMode = true;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(UltimateLevelOfDetail))]
        public class CustomInspector : UnityEditor.Editor
        {
            //Private variables of Editor Only
            private Vector2 gameObjectsToIgnoreScrollpos = Vector2.zero;
            private Vector2 gameObjectsFoundInLastScanScrollpos = Vector2.zero;
            private Vector2 ulodsListScrollpos = Vector2.zero;
            private bool haveSkinnedMeshes_showWarningOfSkinnedLods = false;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                UltimateLevelOfDetail script = (UltimateLevelOfDetail)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");
                script.gizmosOfThisComponentIsDisabled = MTAssetsEditorUi.DisableGizmosInSceneView("UltimateLevelOfDetail", script.gizmosOfThisComponentIsDisabled);

                //Run setup, if is not runned yet
                if (script.setupRunned == false)
                {
                    script.customCameraForSimulationOfLods = Camera.main;
                    script.setupRunned = true;
                }

                //If already have a ULOD component here, show error
                UltimateLevelOfDetail[] ulodsInThisGameObject = script.GetComponents<UltimateLevelOfDetail>();
                if (ulodsInThisGameObject.Length > 1)
                {
                    int isWorkingWithScanInChildren = 0;
                    foreach (UltimateLevelOfDetail ulod in ulodsInThisGameObject)
                        if (ulod.modeOfMeshesScanning == ScanMeshesMode.ScanInChildrenGameObjectsOnly)
                            isWorkingWithScanInChildren += 1;
                    if (isWorkingWithScanInChildren > 1)
                    {
                        EditorGUILayout.HelpBox("It has been identified that there are more than 1 ULOD component in this GameObject, that is working on method of scan in children. For everything to work well, please keep only one ULOD per GameObject.", MessageType.Error);
                        script.modeOfMeshesScanning = (ScanMeshesMode)EditorGUILayout.EnumPopup(new GUIContent("Mode Of Meshes Scanning", "Please, change mode of meshes scanning of some of ULODs in this GameObject."), script.modeOfMeshesScanning);
                        return;
                    }
                }

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Ultimate LOD System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);

                //Try to find parent and children ULODs components
                List<UltimateLevelOfDetail> parentUlods = Finder_FindAllParentUlodsWhereWorkingModeScanChildrensEnabled(script);
                List<UltimateLevelOfDetail> childrenUlods = Finder_FindAllChildrenUlods(script);

                GUILayout.Space(10);

                //Show the main resume
                if (script.currentScannedMeshesList.Count == 0)
                {
                    GUIStyle titulo = new GUIStyle();
                    titulo.fontSize = 16;
                    titulo.normal.textColor = Color.red;
                    titulo.alignment = TextAnchor.MiddleCenter;
                    EditorGUILayout.LabelField("No Scanning Done Yet", titulo);
                }
                if (script.currentScannedMeshesList.Count > 0)
                {
                    GUIStyle titulo = new GUIStyle();
                    titulo.fontSize = 16;
                    titulo.normal.textColor = new Color(0, 79.0f / 250.0f, 3.0f / 250.0f);
                    titulo.alignment = TextAnchor.MiddleCenter;
                    EditorGUILayout.LabelField("Meshes Scanned And LODs Working", titulo);
                    if (script.isLodSimulationEnabledInThisSceneForEditorSceneViewMode == false)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("LOD simulation has been disabled for this scene, so even if this component has a scan, LODs will not be displayed in edit mode in this scene. The simulation of LODs will still work normally at runtime. You can activate the simulation again in \"Tools > Ultimate Level Of Detail > Editor LODs Simulation > Enable In This Scene\".", MessageType.Error);
                        if (GUILayout.Button("Enable LODs Simulation In This Scene Again"))
                        {
                            UltimateLevelOfDetail[] ulods = UnityEngine.Object.FindObjectsOfType<UltimateLevelOfDetail>();
                            foreach (UltimateLevelOfDetail ulod in ulods)
                            {
                                if (ulod.gameObject == null)
                                    continue;
                                ulod.isLodSimulationEnabledInThisSceneForEditorSceneViewMode = true;
                                ulod.ForceThisComponentToUpdateLodsRender();
                            }
                            Debug.Log("The LOD simulation was activated again for the Ultimate Level Of Detail components of this scene.");
                        }
                    }
                }

                //Scan settings
                Params_MeshesScanSettings(script, parentUlods);

                //List ignore GameObjects during scan
                Params_IgnoreGameObjectsDuringScan(script);

                //LODs
                Params_LodsSettings(script);

                //Distance of view for each LOD
                Params_DistanceOfView(script);

                //Managed meshes
                Debbug_OriginalMeshesFoundInLastScan(script);

                //Parent ULODs shower
                Debbug_ParendAndChildrenUlodsFound(script, parentUlods, childrenUlods);

                //Settings for "Scanning Events"
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Scanning Events", EditorStyles.boldLabel);
                GUILayout.Space(10);
                script.showScanEventsOptions = EditorGUILayout.Foldout(script.showScanEventsOptions, (script.showScanEventsOptions == true ? "Hide Scan Events Parameters" : "Show Scan Events Parameters"));
                if (script.showScanEventsOptions == true)
                    DrawDefaultInspector();

                //Debbuging
                Debbug_Settings(script);

                GUILayout.Space(20);

                //Only able to scan or generate LODs, if this is the parent ULOD, or if not have parent ULODs
                if (parentUlods.Count == 0)
                {
                    if (script.currentScannedMeshesList.Count == 0)
                        if (GUILayout.Button("Scan All Meshes And Generate LODs", GUILayout.Height(40)))
                        {
                            //Scan in this gameobject first
                            script.ScanForMeshesAndGenerateAllLodGroups_StartProcessing(true);
                            //Scan in all children GameObjects also (if have)
                            if (childrenUlods.Count > 0)
                            {
                                int index = 0;
                                foreach (UltimateLevelOfDetail ulod in childrenUlods)
                                {
                                    EditorUtility.DisplayProgressBar("Please wait...", "Generating LODs for children ULOD in GameObject \"" + ulod.gameObject.name + "\"...", ((float)index / (float)childrenUlods.Count));
                                    if (ulod.modeOfMeshesScanning == ScanMeshesMode.ScanInThisGameObjectOnly)
                                        if (ulod.currentScannedMeshesList.Count == 0)
                                            ulod.ScanForMeshesAndGenerateAllLodGroups_StartProcessing(false);
                                    index += 1;
                                }
                                EditorUtility.ClearProgressBar();
                            }
                            //Show warning if not clicked on don't show again
                            if (PlayerPrefs.GetInt("DontShowAgainWarningAboutRemoveComponent", 0) == 0)
                                if (EditorUtility.DisplayDialog("Warning", "Please do not remove the \"Ultimate Level Of Detail\" component without undoing the scan first (by clicking on \"Delete All Meshes Data Scanned And LODs\").\n\nThis will restore the original meshes and allow you to safely remove the component.", "Ok", "Don't Show Again") == false)
                                    PlayerPrefs.SetInt("DontShowAgainWarningAboutRemoveComponent", 1);
                        }
                    if (script.currentScannedMeshesList.Count > 0)
                        if (GUILayout.Button("Delete All Meshes Data Scanned And LODs", GUILayout.Height(40)))
                            if (EditorUtility.DisplayDialog("Continue?", "Are you ready to delete all groups of LODs created and restore all original meshes, continue?", "Yes", "No") == true)
                            {
                                //Delete scan in this gameobject first
                                script.UndoAllMeshesScannedAndAllLodGroups(true, true, true, true);
                                //Delete scan in all children GameObjects also (if have)
                                if (childrenUlods.Count > 0)
                                {
                                    int index = 0;
                                    foreach (UltimateLevelOfDetail ulod in childrenUlods)
                                    {
                                        EditorUtility.DisplayProgressBar("Please wait...", "Deleting LODs of children ULOD in GameObject \"" + ulod.gameObject.name + "\"...", ((float)index / (float)childrenUlods.Count));
                                        if (ulod.modeOfMeshesScanning == ScanMeshesMode.ScanInThisGameObjectOnly)
                                            if (ulod.currentScannedMeshesList.Count > 0)
                                                ulod.UndoAllMeshesScannedAndAllLodGroups(false, true, true, true);
                                        index += 1;
                                    }
                                    EditorUtility.ClearProgressBar();
                                }
                            }
                }
                //If have parent ULODs notify the user that this, will be managed by the parent ULOD
                if (parentUlods.Count > 0)
                {
                    EditorGUILayout.HelpBox("This ULOD component has identified that there is another parent ULOD component of this component. Therefore, you can no longer control when this component should or should not scan and create LODs. The scan of this component is now synchronized with the scan of the parent ULOD component. When the parent ULOD does a scan, this ULOD will also do it and when the parent ULOD deletes the scan it has, this ULOD will also delete it. Everything will be synchronized automatically and you can see here, which ULOD parent was identified and if you go to the ULOD parent, you can see all the child ULODs he identified. As the parent ULOD is already working with scanning all of the child meshes, this ULOD component can only work with scanning the mesh that is in this GameObject only. Consult the documentation for more details.", MessageType.Warning);
                }

                //Validate all parameters selected and clear lists of chidren and parent ulods
                script.ValidateAllParameters(false);
                childrenUlods.Clear();
                parentUlods.Clear();

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

            public List<UltimateLevelOfDetail> Finder_FindAllParentUlodsWhereWorkingModeScanChildrensEnabled(UltimateLevelOfDetail script)
            {
                List<UltimateLevelOfDetail> parentUlods = new List<UltimateLevelOfDetail>();
                //Find parent ulods where working mode is Scan in Children GameObjects only
                UltimateLevelOfDetail[] tempParentUlods = script.GetComponentsInParent<UltimateLevelOfDetail>();
                foreach (UltimateLevelOfDetail ulod in tempParentUlods)
                {
                    if (ulod == script)
                        continue;
                    if (ulod.gameObject == script.gameObject)
                        continue;
                    if (ulod.modeOfMeshesScanning == ScanMeshesMode.ScanInChildrenGameObjectsOnly)
                        parentUlods.Add(ulod);
                }
                return parentUlods;
            }

            public List<UltimateLevelOfDetail> Finder_FindAllChildrenUlods(UltimateLevelOfDetail script)
            {
                List<UltimateLevelOfDetail> childrenUlods = new List<UltimateLevelOfDetail>();
                UltimateLevelOfDetail[] tempChildrenUlods = null;
                //Try to find children ULODs components, if "ScanInChildrenGameObjectsOnly" is enabled on this ULOD and change all children ULODs to scan only your gameobjects
                if (script.modeOfMeshesScanning == ScanMeshesMode.ScanInChildrenGameObjectsOnly)
                {
                    tempChildrenUlods = script.GetComponentsInChildren<UltimateLevelOfDetail>();
                    foreach (UltimateLevelOfDetail ulod in tempChildrenUlods)
                    {
                        if (ulod == script)
                            continue;
                        if (ulod.gameObject == script.gameObject)
                            continue;
                        ulod.modeOfMeshesScanning = ScanMeshesMode.ScanInThisGameObjectOnly;
                        childrenUlods.Add(ulod);
                    }
                }
                return childrenUlods;
            }

            public void Params_MeshesScanSettings(UltimateLevelOfDetail script, List<UltimateLevelOfDetail> parentUlods)
            {
                //Scan settings
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Meshes Scan Settings", EditorStyles.boldLabel);
                GUILayout.Space(10);

                if (script.currentScannedMeshesList.Count == 0 || script.forceShowHiddenSettings == true)
                {
                    script.modeOfMeshesScanning = (ScanMeshesMode)EditorGUILayout.EnumPopup(new GUIContent("Mode Of Meshes Scanning",
                                            "Mesh scanning mode that ULOD will use, that is, how and where ULOD should look for meshes to generate LODs.\n\n" +
                                            "ScanInChildrenGameObjectsOnly - ULOD will only search for meshes in the GameObjects children of this GameObject. GameObjects children of this GameObject, which contain a ULOD component, will be automatically ignored. The mesh in this GameObject will also be ignored.\n\n" +
                                            "ScanInThisGameObjectOnly - ULOD will only search for meshes that are in THIS GameObject, ignoring child GameObjects, etc."),
                                            script.modeOfMeshesScanning);
                    if (script.modeOfMeshesScanning == ScanMeshesMode.ScanInChildrenGameObjectsOnly)
                    {
                        EditorGUI.indentLevel += 1;
                        script.scanInactiveGameObjects = (bool)EditorGUILayout.Toggle(new GUIContent("Scan Inactive Too",
                            "If this option is active, ULOD will also scan meshes that are disabled."),
                            script.scanInactiveGameObjects);
                        EditorGUI.indentLevel -= 1;

                        //If have parent ULODs working in scanning children, change it to scan in this GameObject only
                        if (parentUlods.Count > 0)
                        {
                            script.modeOfMeshesScanning = ScanMeshesMode.ScanInThisGameObjectOnly;
                            EditorUtility.DisplayDialog("Warning", "It has been identified that there is an Ultimate Level Of Detail component that is the parent of this GameObject and it is working in the scanning mode of child GameObjects. So, in order not to interfere with his work, this ULOD component can only operate in scan mode for this GameObject only. So all the settings you make in this ULOD will only work for this GameObject. The scans of this ULOD will also be synchronized with the scans of the parent ULOD.", "Ok");
                        }
                    }
                }
                if (script.currentScannedMeshesList.Count > 0 && script.forceShowHiddenSettings == false)
                {
                    EditorGUILayout.HelpBox("These parameters are only available before you perform a scan.", MessageType.Info);
                }
            }

            public void Params_IgnoreGameObjectsDuringScan(UltimateLevelOfDetail script)
            {
                //Only show ignore gameobjects settings if scan childres is enabled
                if (script.modeOfMeshesScanning == ScanMeshesMode.ScanInChildrenGameObjectsOnly)
                {
                    //Settings for "Meshes To Ignore"
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Ignore During Scan", EditorStyles.boldLabel);
                    GUILayout.Space(10);

                    if (script.currentScannedMeshesList.Count == 0 || script.forceShowHiddenSettings == true)
                    {
                        Texture2D removeItemIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Ultimate LOD System/Editor/Images/Remove.png", typeof(Texture2D));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("GameObjects To Ignore During Scan", GUILayout.Width(230));
                        GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 230);
                        EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                        EditorGUILayout.IntField(script.gameObjectsToIgnore.Count, GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                        GUILayout.BeginVertical("box");
                        gameObjectsToIgnoreScrollpos = EditorGUILayout.BeginScrollView(gameObjectsToIgnoreScrollpos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                        if (script.gameObjectsToIgnore.Count == 0)
                            EditorGUILayout.HelpBox("Oops! No GameObject to be ignored has been registered! If you want to subscribe any, click the button below!", MessageType.Info);
                        if (script.gameObjectsToIgnore.Count > 0)
                            for (int i = 0; i < script.gameObjectsToIgnore.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                if (script.currentScannedMeshesList.Count == 0)
                                    if (GUILayout.Button(removeItemIcon, GUILayout.Width(25), GUILayout.Height(16)))
                                        script.gameObjectsToIgnore.RemoveAt(i);
                                script.gameObjectsToIgnore[i] = (GameObject)EditorGUILayout.ObjectField(new GUIContent("GameObject " + i.ToString(), "This GameObject will be ignored during the scan, if it has any mesh, it will not be scanned to have LODs.\n\nClick the button to the left if you want to remove this GameObject from the list."), script.gameObjectsToIgnore[i], typeof(GameObject), true, GUILayout.Height(16));
                                GUILayout.EndHorizontal();
                            }
                        EditorGUILayout.EndScrollView();
                        GUILayout.EndVertical();
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add New Slot"))
                        {
                            script.gameObjectsToIgnore.Add(null);
                            gameObjectsToIgnoreScrollpos.y += 999999;
                        }
                        if (script.gameObjectsToIgnore.Count > 0)
                            if (GUILayout.Button("Remove Empty Slots", GUILayout.Width(Screen.width * 0.48f)))
                                for (int i = script.gameObjectsToIgnore.Count - 1; i >= 0; i--)
                                    if (script.gameObjectsToIgnore[i] == null)
                                        script.gameObjectsToIgnore.RemoveAt(i);
                        GUILayout.EndHorizontal();
                    }
                    if (script.currentScannedMeshesList.Count > 0 && script.forceShowHiddenSettings == false)
                    {
                        EditorGUILayout.HelpBox("These parameters are only available before you perform a scan.", MessageType.Info);
                    }
                }
            }

            public void Params_LodsSettings(UltimateLevelOfDetail script)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("LODs Generation Settings", EditorStyles.boldLabel);
                GUILayout.Space(10);

                if (script.currentScannedMeshesList.Count == 0 || script.forceShowHiddenSettings == true)
                {
                    script.levelsOfDetailToGenerate = EditorGUILayout.IntSlider(new GUIContent("Levels Of Details To Generate",
                               "The number of LODs that the Ultimate LOD System should generate."),
                               script.levelsOfDetailToGenerate, 1, 8);

                    EditorGUI.indentLevel += 1;

                    for (int i = 0; i <= script.levelsOfDetailToGenerate; i++)
                    {
                        if (i == 0)
                            continue;

                        script.percentOfVerticesForEachLod[i] = EditorGUILayout.Slider(new GUIContent("Percent Of Vertices in LOD " + i.ToString(),
                                            "The percentage of vertices it will contain in LOD " + i.ToString() + " of the meshes."),
                                            script.percentOfVerticesForEachLod[i], 1f, 100f);
                    }

                    EditorGUI.indentLevel -= 1;

                    script.saveGeneratedLodsInAssets = (bool)EditorGUILayout.Toggle(new GUIContent("Save LODs Meshes In Assets",
                                    "If this option is active, ULOD will save the mesh files of the LODs generated in your project."),
                                    script.saveGeneratedLodsInAssets);

                    script.skinnedAnimsCompatibilityMode = (bool)EditorGUILayout.Toggle(new GUIContent("Skinned Anims Compat Mode",
                                    "If this option is active, ULOD will use internal algorithms to improve the accuracy and compatibility with Skinned Mesh Renderers animations. If you have problems or artifacts, try disabling this. This only applies to meshes that are in Skinned Mesh Renderers."),
                                    script.skinnedAnimsCompatibilityMode);

                    script.preventArtifacts = (bool)EditorGUILayout.Toggle(new GUIContent("Prevent Artifacts Or Deform",
                                    "If this option is active, ULOD will use internal algorithms to prevent artifacts in the generated LOD meshes. If you still have problems with artifacts, try disabling this.\n\nNote that: Meshes that contain many vertices sharing the same space, may have problems or present artifacts."),
                                    script.preventArtifacts);

                    script.optimizeResultingMeshes = (bool)EditorGUILayout.Toggle(new GUIContent("Optimize Resulting Meshes",
                                                            "If this option is enabled, the ULOD will optimize the mesh resulting from the merge. This may lead to performance gains in rendering the mesh resulting from the merging, through the mechanism of Unity.\n\nThis can slightly increase the mesh processing time.\n\nMesh optimization can generate some minor visual problems with the mesh in some cases. Disable this option if this happens."),
                                                            script.optimizeResultingMeshes);

                    script.enableLightmapsSupport = (bool)EditorGUILayout.Toggle(new GUIContent("Enable Lightmaps Support",
                                    "If this option is active, lightmaps will be supported by simplified meshes, generated by ULOD. This function only works when generating LODs in the Editor.\n\n** This function can dramatically increase the creation time of the simplified mesh, so just activate this function, in case you need to use the simplified meshes and lightmaps. **"),
                                    script.enableLightmapsSupport);

                    script.enableMaterialsChanges = (bool)EditorGUILayout.Toggle(new GUIContent("Enable Materials Changes",
                    "If you activate this option, the Ultimate LOD System will scan the materials of the meshes as well, and will allow you to configure an array of materials for each level of detail of your meshes! You will be able to supply the individual materials for each level, in the \"Ultimate LOD Meshes\" component present in each scanned mesh."),
                    script.enableMaterialsChanges);

                    script.forceOfSimplification = (ForceOfSimplification)EditorGUILayout.EnumPopup(new GUIContent("Force Of Simplification",
                                    "Some meshes have such a large number of vertices, that even reducing their vertices to 1%, they will still have many vertices, or the algorithm can avoid a very large reduction of vertices, to maintain the original shape of the mesh. If you want to reduce even more the amount of vertices present in the meshes, increase the strength of the simplification. Very large forces in meshes with a moderate amount of vertices, you can completely deform them, so use this parameter while testing, and only use it if you really need a greater vertex reduction force.\n\nArtifact prevention is disabled if you use forces greater than the Normal force, as there is no way to prevent artifacts when greater aggressiveness is imposed in simplifying the mesh."),
                                    script.forceOfSimplification);

                    script.cullingMode = (CullingMode)EditorGUILayout.EnumPopup(new GUIContent("Culling Mode",
                                    "Culling will disable meshes that are too far apart, with a distance you define. By default, Culling is disabled, but you can choose a culling method and then you can set the minimum distance for culling to occur." +
                                    "\n\nDisabled - Culling will not occur, only the last LOD level will be displayed forever, regardless of the distance between the camera and this GameObject ULOD. Note that the standard camera culling will still be performed (The culling where Unity hides very distant meshes, as a standard behavior of cameras on Unity)." +
                                    "\n\nCullingMeshes - In this Culling method, the Ultimate LOD System will remove the mesh from the renderers, when reaching the Culling distance. If the camera is out of Culling's distance, the LOD meshes will be returned to the renderers." +
                                    "\n\nCullingRenderer - In this culling mode, when the camera moves away at the desired distance, the mesh renderers will be \"prohibited\" from working and will only work again if the camera approaches again. This \"prohibition\" will work even though the renderer/gameobject is still active."),
                                    script.cullingMode);
                }
                if (script.currentScannedMeshesList.Count > 0 && script.forceShowHiddenSettings == false)
                {
                    EditorGUILayout.HelpBox("These parameters are only available before you perform a scan.", MessageType.Info);
                }
            }

            public void Params_DistanceOfView(UltimateLevelOfDetail script)
            {
                //Distance of view
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Distance of View for Each LOD", EditorStyles.boldLabel);
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal("box");
                for (int i = 0; i <= script.levelsOfDetailToGenerate; i++)
                {
                    if (i == 0)
                        continue;

                    if (GUILayout.Button("LOD " + i.ToString()))
                        script.ShowMinDistanceToViewLodInSceneView(script.minDistanceOfViewForEachLod[i] + 1);
                }
                if (script.cullingMode != CullingMode.Disabled)
                    if (GUILayout.Button("Cull"))
                        script.ShowMinDistanceToViewLodInSceneView(script.minDistanceOfViewForCull + 1);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);

                for (int i = 0; i <= script.levelsOfDetailToGenerate; i++)
                {
                    if (i == 0)
                        continue;

                    script.minDistanceOfViewForEachLod[i] = EditorGUILayout.Slider(new GUIContent("Min Distance To View LOD " + i.ToString(),
                               "The minimum distance required (in Units) to view LOD " + i.ToString() + "."),
                               script.minDistanceOfViewForEachLod[i], 1f, 3000f);
                }

                if (script.cullingMode != CullingMode.Disabled)
                {
                    script.minDistanceOfViewForCull = EditorGUILayout.Slider(new GUIContent("Min Distance Of View For Cull",
                    "This represents the minimum distance between this ULOD and the camera that currently renders these objects, for the culling of the meshes to occur. When culling the meshes, the ULOD will hide and stop rendering the meshes until the distance of the last level or less, returns between the camera and this ULOD."),
                    script.minDistanceOfViewForCull, 1f, 3000f);
                }

                script.cameraDetectionMode = (CameraDetectionMode)EditorGUILayout.EnumPopup(new GUIContent("Camera Detection Mode",
                                                                    "The camera detection method that the Ultimate LOD System should use to detect the distance between the camera and this GameObject to simulate the change of LODs.\n\n" +
                                                                    "CurrentCamera - In this method the Ultimate LOD System will use a component called \"Ultimate LOD Data\" that will stay in your scene and will try to determine the camera that is currently appearing on the screen, using an automatic algorithm. The camera determined by the algorithm will be used to calculate the distance between this GameObject and the camera, for the simulation of LODs.\n\n" +
                                                                    "MainCamera - This method will use \"Camera.main\" to identify a camera to calculate the distance. It requires that the main camera of your game has the tag \"MainCamera\".\n\n" +
                                                                    "CustomCamera - In this method you can define a customized camera so that the ULOD calculates the distance and makes the simulation of LODs only in relation to it. This gives you more control over how the LOD simulation will be done, it can be very useful for multiplayer games for example."),
                                                                    script.cameraDetectionMode);
                if (script.cameraDetectionMode == CameraDetectionMode.MainCamera)
                {
                    script.useCacheForMainCameraInDetection = (bool)EditorGUILayout.Toggle(new GUIContent("Use Cache Of Main Camera",
                                    "If this option is active, the Ultimate LOD System will use a cache from the main camera, as soon as it is detected, so that it is not necessary to search for it in each frame.\n\nIf this option is active, performance may increase, however, the detection accuracy may be lower. Try disabling this if you want more precision."),
                                    script.useCacheForMainCameraInDetection);
                }
                if (script.cameraDetectionMode == CameraDetectionMode.CustomCamera)
                {
                    script.customCameraForSimulationOfLods = (Camera)EditorGUILayout.ObjectField(new GUIContent("Custom Camera For Simulate",
                                                        "The customized camera, which will be used to calculate the distance and simulation of the LODs."),
                                                        script.customCameraForSimulationOfLods, typeof(Camera), true, GUILayout.Height(16));
                }

                //Distance showers, on change a distance
                for (int i = 0; i <= script.levelsOfDetailToGenerate; i++)
                {
                    if (i == 0)
                        continue;

                    if (script.last_minDistanceOfViewForEachLod[i] != script.minDistanceOfViewForEachLod[i])
                    {
                        if (script.last_minDistanceOfViewForEachLod[i] > 0)
                            script.ShowMinDistanceToViewLodInSceneView(script.minDistanceOfViewForEachLod[i] + 1);
                        script.last_minDistanceOfViewForEachLod[i] = script.minDistanceOfViewForEachLod[i];
                    }
                }
                if (script.last_minDistanceOfViewForCull != script.minDistanceOfViewForCull)
                {
                    if (script.last_minDistanceOfViewForCull > 0)
                        script.ShowMinDistanceToViewLodInSceneView(script.minDistanceOfViewForCull + 1);
                    script.last_minDistanceOfViewForCull = script.minDistanceOfViewForCull;
                }
            }

            public void Debbug_OriginalMeshesFoundInLastScan(UltimateLevelOfDetail script)
            {
                //Managed meshes
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Scanned Meshes For LOD", EditorStyles.boldLabel);
                GUILayout.Space(10);

                if (script.currentScannedMeshesList.Count == 0)
                {
                    EditorGUILayout.HelpBox("There are no meshes scanned by this component yet. Click the button below to start scanning and generating LODs to begin. All meshes that are identified by ULOD, will appear here and have their LODs generated.", MessageType.Info);
                }
                if (script.currentScannedMeshesList.Count > 0)
                {
                    //Create font red
                    GUIStyle error = new GUIStyle();
                    error.normal.textColor = Color.red;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Meshes Found In Current Scan", GUILayout.Width(230));
                    GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 230);
                    EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                    EditorGUILayout.IntField(script.currentScannedMeshesList.Count, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    GUILayout.BeginVertical("box");
                    gameObjectsFoundInLastScanScrollpos = EditorGUILayout.BeginScrollView(gameObjectsFoundInLastScanScrollpos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                    //Original meshes list
                    foreach (ScannedMeshItem meshItem in script.currentScannedMeshesList)
                    {
                        GUILayout.Space(2);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical();
                        if (meshItem.originalGameObject != null)
                            EditorGUILayout.LabelField(meshItem.originalGameObject.name, EditorStyles.boldLabel);
                        if (meshItem.originalGameObject == null)
                            EditorGUILayout.LabelField("GameObject Not Found", error);
                        GUILayout.Space(-3);
                        if (meshItem.originalGameObject == null || meshItem.allMeshLods[0] == null)
                        {
                            EditorGUILayout.LabelField("Please, re-scan all meshes to fix this.", error);
                        }
                        if (meshItem.originalGameObject != null && meshItem.allMeshLods[0] != null)
                        {
                            //Check if have a missing LOD mesh
                            bool missingMeshes = false;
                            for (int i = 0; i < 9; i++)
                                if (meshItem.allMeshLods[i] == null && script.levelsOfDetailToGenerate >= i)
                                    missingMeshes = true;

                            if (meshItem.originalSkinnedMeshRenderer != null)
                            {
                                if (meshItem.allMeshLods[0] != null && missingMeshes == false)
                                    EditorGUILayout.LabelField("Skinned Mesh, " + meshItem.allMeshLods[0].name + Path.GetExtension(AssetDatabase.GetAssetPath(meshItem.allMeshLods[0])));
                                if (meshItem.allMeshLods[0] == null || missingMeshes == true)
                                    EditorGUILayout.LabelField("Missing mesh file or LODs. Redo the scan.", error);
                                haveSkinnedMeshes_showWarningOfSkinnedLods = true;
                            }
                            if (meshItem.originalMeshFilter != null)
                            {
                                if (meshItem.allMeshLods[0] != null && missingMeshes == false)
                                    EditorGUILayout.LabelField("Normal Mesh, " + meshItem.allMeshLods[0].name + Path.GetExtension(AssetDatabase.GetAssetPath(meshItem.allMeshLods[0])));
                                if (meshItem.allMeshLods[0] == null || missingMeshes == true)
                                    EditorGUILayout.LabelField("Missing mesh file or LODs. Redo the scan.", error);
                            }
                            if (meshItem.originalMeshFilter == null && meshItem.originalSkinnedMeshRenderer == null)
                                EditorGUILayout.LabelField("No renderer found. Please, re-scan to fix this.", error);
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(20);
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(8);
                        if (meshItem.originalGameObject != null)
                            if (GUILayout.Button("Game Object", GUILayout.Height(20)))
                                EditorGUIUtility.PingObject(meshItem.originalGameObject);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(2);
                    }
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    EditorGUILayout.HelpBox("All the meshs listed above were found in the last scan and their LODs were automatically generated based on your configuration.", MessageType.Info);
                }
            }

            public void Debbug_ParendAndChildrenUlodsFound(UltimateLevelOfDetail script, List<UltimateLevelOfDetail> parentUlods, List<UltimateLevelOfDetail> childrenUlods)
            {
                //Parent ULODs shower
                if (script.modeOfMeshesScanning == ScanMeshesMode.ScanInThisGameObjectOnly && parentUlods.Count > 0)
                {
                    //Parent ULODs
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Parent ULODs components", EditorStyles.boldLabel);
                    GUILayout.Space(10);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Parent ULODs Controlling This", GUILayout.Width(230));
                    GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 230);
                    EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                    EditorGUILayout.IntField(parentUlods.Count, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    GUILayout.BeginVertical("box");
                    ulodsListScrollpos = EditorGUILayout.BeginScrollView(ulodsListScrollpos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                    //Original meshes list
                    foreach (UltimateLevelOfDetail ulod in parentUlods)
                    {
                        //Skip if this not is the last parent (top parent)
                        if (ulod != parentUlods[parentUlods.Count - 1])
                            continue;

                        GUILayout.Space(2);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField(ulod.gameObject.name, EditorStyles.boldLabel);
                        GUILayout.Space(-3);
                        EditorGUILayout.LabelField("Is controlling this component scan.");
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(20);
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(8);
                        if (GUILayout.Button("Copy Distance Data", GUILayout.Height(20)))
                        {
                            for (int i = 0; i < 9; i++)
                                script.minDistanceOfViewForEachLod[i] = ulod.minDistanceOfViewForEachLod[i];
                            script.minDistanceOfViewForCull = ulod.minDistanceOfViewForCull;
                            script.cullingMode = ulod.cullingMode;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(8);
                        if (GUILayout.Button("Go To", GUILayout.Height(20)))
                            Selection.objects = new UnityEngine.Object[] { ulod.gameObject };
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(2);
                    }
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }

                //Childrens ULODs shower
                if (script.modeOfMeshesScanning == ScanMeshesMode.ScanInChildrenGameObjectsOnly && childrenUlods.Count > 0)
                {
                    //Children ULODs
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Children ULODs components", EditorStyles.boldLabel);
                    GUILayout.Space(10);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Children ULODs Being Controlled", GUILayout.Width(230));
                    GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 230);
                    EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                    EditorGUILayout.IntField(childrenUlods.Count, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    GUILayout.BeginVertical("box");
                    ulodsListScrollpos = EditorGUILayout.BeginScrollView(ulodsListScrollpos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                    //Original meshes list
                    foreach (UltimateLevelOfDetail ulod in childrenUlods)
                    {
                        GUILayout.Space(2);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField(ulod.gameObject.name + ((ulod.currentScannedMeshesList.Count > 0) ? " (Done)" : " (Not Scanning Done Yet)"), EditorStyles.boldLabel);
                        GUILayout.Space(-3);
                        EditorGUILayout.LabelField("The scanning of this ULOD is synchronized with it.");
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(20);
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(8);
                        if (GUILayout.Button("Go To", GUILayout.Height(20)))
                            Selection.objects = new UnityEngine.Object[] { ulod.gameObject };
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(2);
                    }
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
            }

            public void Debbug_Settings(UltimateLevelOfDetail script)
            {
                //Debbugging
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Debbug Settings", EditorStyles.boldLabel);
                GUILayout.Space(10);

                if (script.currentScannedMeshesList.Count == 0)
                {
                    EditorGUILayout.HelpBox("There are no meshes scanned by this component yet. Click the button below to start scanning and generating LODs to begin. All meshes that are identified by ULOD, will appear here and have their LODs generated.", MessageType.Info);
                }
                if (script.currentScannedMeshesList.Count > 0)
                {
                    //Show notification of unity lods skinned mesh renderer simulation, if have skinned meshes
                    if (haveSkinnedMeshes_showWarningOfSkinnedLods == true)
                        EditorGUILayout.HelpBox("This scan has Skinned Mesh Renderers. The simulation of LOD changes in Skinned Mesh Renderers will not take place in the Editor to avoid problems of crash of the Editor, due to limitations of some versions of Unity. The Ultimate LOD System will still change LODs for your Skinned Mesh Renderers in your Build game (final game in Android, Windows, iOS or other platform, for example) without any problems. If you want to force this simulation on the Editor too, activate the \"Editor Skinned LODs Change\" option below.", MessageType.Warning);

                    script.forceChangeLodsOfSkinnedInEditor = (bool)EditorGUILayout.Toggle(new GUIContent("Editor Skinned LODs Change",
                                               "Some versions of the Unity editor have problems with simulating Skinned mesh LODs. It turns out that in some versions of Unity, during the mesh change simulation in the Skinned Mesh Renderer, the Editor may crash. To avoid this, by default, the LOD mesh simulation in Skinned Mesh Renderers, in the editor, is disabled.\n\nEven so, the Ultimate LOD System will still change LODs in Skinned Mesh Renderers, in building your game, without any problems. The crash problem only occurs in the Editor.\n\nIf you want to force the mesh change simulation in the Editor, just activate this option, but be aware that the crash problem can occur depending on the version of your Editor, this is a limitation of Unity Editor in some versions."),
                                               script.forceChangeLodsOfSkinnedInEditor);

                    script.drawGizmoOnThisPivot = (bool)EditorGUILayout.Toggle(new GUIContent("Draw Gizmo On This Pivot",
                       "If this option is active, ULOD will draw a small Gizmo on the pivot of this GameObject to let you know that this GameObject has a group of LODs created."),
                       script.drawGizmoOnThisPivot);
                    if (script.drawGizmoOnThisPivot == true)
                    {
                        EditorGUI.indentLevel += 1;
                        script.colorOfGizmo = EditorGUILayout.ColorField(new GUIContent("Color Of Gizmo",
                       "The color of the gizmo that will be drawn."),
                       script.colorOfGizmo);

                        script.sizeOfGizmo = EditorGUILayout.Slider(new GUIContent("Size Of Gizmo",
                                                           "The size of the Gizmo that will be drawn."),
                                                           script.sizeOfGizmo, 0.01f, 10f);
                        EditorGUI.indentLevel -= 1;
                    }

                    script.forceShowHiddenSettings = (bool)EditorGUILayout.Toggle(new GUIContent("Force Show Hidden Settings",
                                               "Enable this option so that this Ultimate Level Of Detail component will always display all scan settings, even if there is an existing scan."),
                                               script.forceShowHiddenSettings);
                }
            }

            protected virtual void OnSceneGUI()
            {
                UltimateLevelOfDetail script = (UltimateLevelOfDetail)target;
                //If the script is null, cancel the render of this OnSceneGUI to prevent log errors, on remove or destroy this component when this have a scan
                if (script == null)
                    return;

                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Set the base color of gizmos
                Handles.color = Color.green;

                //If have a scan
                if (script.currentScannedMeshesList.Count > 0)
                {
                    //Current LOD stats
                    StringBuilder currentTextStr = new StringBuilder();
                    if (script.currentLodAccordingToDistance == -1 || script.currentLodAccordingToDistance == 0)
                    {
                        currentTextStr.Append("LOD 0 (100%)");
                    }
                    if (script.currentLodAccordingToDistance > 0 && script.currentLodAccordingToDistance < 9)
                    {
                        currentTextStr.Append("LOD ");
                        currentTextStr.Append(script.currentLodAccordingToDistance.ToString());
                        currentTextStr.Append(" (");
                        currentTextStr.Append(script.percentOfVerticesForEachLod[script.currentLodAccordingToDistance]);
                        currentTextStr.Append("%)");
                    }
                    if (script.currentLodAccordingToDistance == 9)
                    {
                        currentTextStr.Append("CULLED (0%)");
                    }
                    currentTextStr.Append("\n");
                    currentTextStr.Append(script.currentDistanceFromMainCamera.ToString("F0"));
                    currentTextStr.Append(" Units");
                    float multiplier = UltimateLevelOfDetailGlobal.GetGlobalLodDistanceMultiplier();
                    if (multiplier != 1.0f)
                    {
                        currentTextStr.Append(" (x");
                        currentTextStr.Append(multiplier);
                        currentTextStr.Append(")");
                    }
                    int vertices = 0;
                    bool identifiedNullMeshes = false;
                    foreach (ScannedMeshItem meshItem in script.currentScannedMeshesList)
                    {
                        //Calculate current count of vertices
                        if (script.currentLodAccordingToDistance == -1 && meshItem.allMeshLods[0] != null)
                            vertices += meshItem.allMeshLods[0].vertexCount;
                        for (int i = 0; i < 9; i++)
                            if (i == script.currentLodAccordingToDistance)
                                if (meshItem.allMeshLods[i] != null)
                                    vertices += meshItem.allMeshLods[i].vertexCount;

                        //Check if have a missing LOD mesh
                        for (int i = 0; i < 9; i++)
                            if (meshItem.allMeshLods[i] == null && script.levelsOfDetailToGenerate >= i)
                                identifiedNullMeshes = true;
                    }
                    currentTextStr.Append("\n");
                    currentTextStr.Append(vertices.ToString());
                    currentTextStr.Append(" Vertices");
                    if (identifiedNullMeshes == true)
                        currentTextStr.Append("\nSCAN ERRORS!");
                    if (script.isLodSimulationEnabledInThisSceneForEditorSceneViewMode == false)
                        currentTextStr.Append("\nSCENE LODs DISABLED");
                    string currentText = currentTextStr.ToString();

                    //Prepare the text
                    GUIStyle styleVerticeDetail = new GUIStyle();
                    styleVerticeDetail.normal.textColor = Color.white;
                    styleVerticeDetail.alignment = TextAnchor.MiddleCenter;
                    styleVerticeDetail.fontStyle = FontStyle.Bold;
                    styleVerticeDetail.contentOffset = new Vector2(-currentText.Substring(0, currentText.IndexOf("\n") + 1).Length * 1.2f, 24);

                    //Draw the LOD stats
                    if (script._customPivotToSimulateLods == null)
                        Handles.Label(script.gameObject.transform.position, currentText, styleVerticeDetail);
                    if (script._customPivotToSimulateLods != null)
                        Handles.Label(script._customPivotToSimulateLods.position, currentText, styleVerticeDetail);
                }

                //If not have a scan yet
                if (script.currentScannedMeshesList.Count == 0)
                {
                    //Current LOD stats
                    StringBuilder currentTextStr = new StringBuilder();
                    currentTextStr.Append("No Scanning\n");
                    currentTextStr.Append("Done Yet!");
                    string currentText = currentTextStr.ToString();

                    //Prepare the text
                    GUIStyle styleVerticeDetail = new GUIStyle();
                    styleVerticeDetail.normal.textColor = Color.red;
                    styleVerticeDetail.alignment = TextAnchor.MiddleCenter;
                    styleVerticeDetail.fontStyle = FontStyle.Bold;
                    styleVerticeDetail.contentOffset = new Vector2(-currentText.Substring(0, currentText.IndexOf("\n") + 1).Length * 1.2f, 24);

                    //Draw the LOD stats
                    if (script._customPivotToSimulateLods == null)
                        Handles.Label(script.gameObject.transform.position, currentText, styleVerticeDetail);
                    if (script._customPivotToSimulateLods != null)
                        Handles.Label(script._customPivotToSimulateLods.position, currentText, styleVerticeDetail);
                }

                //Apply changes on script, case is not playing in editor
                if (GUI.changed == true && Application.isPlaying == false)
                {
                    EditorUtility.SetDirty(script);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
                }
                if (EditorGUI.EndChangeCheck() == true)
                {
                    //Apply the change, if moved the handle
                    //script.transform.position = teste;
                }
                Repaint();
            }
        }
        #endregion

        //Gizmos parameters

        public void OnDrawGizmos()
        {
            //If is not desired to draw gizmo
            if (drawGizmoOnThisPivot == false || currentScannedMeshesList.Count == 0)
                return;

            //Draw gizmo to show pivot of this
            Gizmos.color = colorOfGizmo;
            if (_customPivotToSimulateLods == null)
                Gizmos.DrawSphere(this.gameObject.transform.position, sizeOfGizmo);
            if (_customPivotToSimulateLods != null)
                Gizmos.DrawSphere(_customPivotToSimulateLods.position, sizeOfGizmo);
        }

        //Interface methods

        public void ShowMinDistanceToViewLodInSceneView(float distance)
        {
            //Calculate the distance of camera of scene view, set distance to this gameobject, and rotate scene view to this gameobject
            GameObject baseGameObject = new GameObject("tempObjBase");
            baseGameObject.transform.SetParent(this.gameObject.transform);
            if (_customPivotToSimulateLods == null)
                baseGameObject.transform.localPosition = Vector3.zero;
            if (_customPivotToSimulateLods != null)
                baseGameObject.transform.position = _customPivotToSimulateLods.position;
            GameObject distanceGameObject = new GameObject("tempObjDistance");
            distanceGameObject.transform.SetParent(baseGameObject.transform);
            distanceGameObject.transform.localPosition = Vector3.zero;
            Vector3 position = distanceGameObject.transform.localPosition;
            position.z = distance - ((SceneView.lastActiveSceneView.cameraDistance <= distance) ? SceneView.lastActiveSceneView.cameraDistance : 0);
            distanceGameObject.transform.localPosition = position;
            baseGameObject.transform.LookAt(SceneView.lastActiveSceneView.camera.transform.position);
            SceneView.lastActiveSceneView.LookAt(distanceGameObject.transform.position);
            distanceGameObject.transform.LookAt(baseGameObject.transform.position);
            SceneView.lastActiveSceneView.rotation = distanceGameObject.transform.rotation;
            SceneView.lastActiveSceneView.Repaint();
            DestroyImmediate(baseGameObject, false);
        }
#endif

        //Core methods of scan

        private void ValidateAllParameters(bool isGoingToScan)
        {
            //This method validate the current parameters of scan

            //Calculate the min distance value, of last desired level of LOD
            float minDistanceValueFromLastLevel = 0.0f;
            for (int i = 0; i <= levelsOfDetailToGenerate; i++)
                minDistanceValueFromLastLevel = minDistanceOfViewForEachLod[i];

            //Disable prevent artifacts if force is not normal
            if (forceOfSimplification != ForceOfSimplification.Normal)
                preventArtifacts = false;

            //Validate min level of each
            float[] minDistancesValidForEachLod = new float[] { 0.0f, 1.0f, 5.0f, 10.0f, 15.0f, 20.0f, 25.0f, 30.0f, 35.0f };
            for (int i = 0; i < 9; i++)
                if (minDistanceOfViewForEachLod[i] < minDistancesValidForEachLod[i])
                    minDistanceOfViewForEachLod[i] = minDistancesValidForEachLod[i];
            if (minDistanceOfViewForCull <= minDistanceValueFromLastLevel)
                minDistanceOfViewForCull = (minDistanceValueFromLastLevel + 10.0f);

            //Validate percent of vertices for each level (only if is going to scan)
            if (isGoingToScan == true)
            {
                //Validate percent of vertices
                for (int i = 0; i < 9; i++)
                {
                    if (i == 0)
                        continue;

                    if (levelsOfDetailToGenerate >= i)
                        if (percentOfVerticesForEachLod[i] > percentOfVerticesForEachLod[i - 1])
                        {
                            percentOfVerticesForEachLod[i] = (percentOfVerticesForEachLod[i - 1] - 1.0f);
                            if (percentOfVerticesForEachLod[i] <= 0)
                                percentOfVerticesForEachLod[i] = percentOfVerticesForEachLod[i - 1];
                        }
                }

                //Validate the distances
                for (int i = 8; i >= 0; i--)
                {
                    if (i == 0)
                        continue;

                    if (levelsOfDetailToGenerate >= i)
                        if (minDistanceOfViewForEachLod[i - 1] >= minDistanceOfViewForEachLod[i])
                        {
                            minDistanceOfViewForEachLod[i - 1] = (minDistanceOfViewForEachLod[i] - 1.0f);
                            if (minDistanceOfViewForEachLod[i - 1] <= 0)
                                minDistanceOfViewForEachLod[i - 1] = 1.0f;
                        }
                }

                if (minDistanceOfViewForCull <= minDistanceValueFromLastLevel)
                    minDistanceOfViewForCull = (minDistanceValueFromLastLevel + 10.0f);
            }
        }

        private void CreateHierarchyOfFoldersIfNotExists()
        {
#if UNITY_EDITOR
            //Create the directory in project
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets"))
                AssetDatabase.CreateFolder("Assets", "MT Assets");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData"))
                AssetDatabase.CreateFolder("Assets/MT Assets", "_AssetsData");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData", "Meshes");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes/LODs_1"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData/Meshes", "LODs_1");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes/LODs_2"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData/Meshes", "LODs_2");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes/LODs_3"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData/Meshes", "LODs_3");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes/LODs_4"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData/Meshes", "LODs_4");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes/LODs_5"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData/Meshes", "LODs_5");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes/LODs_6"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData/Meshes", "LODs_6");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes/LODs_7"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData/Meshes", "LODs_7");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Meshes/LODs_8"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData/Meshes", "LODs_8");
#endif
        }

        private string SaveGeneratedLodInAssets(string lodNumber, long ticks, Mesh generatedLodMesh)
        {
#if UNITY_EDITOR
            //Save generated LOD in assets, if desired
            if (saveGeneratedLodsInAssets == true && Application.isPlaying == false)
            {
                string path = "Assets/MT Assets/_AssetsData/Meshes/LODs_" + lodNumber + "/LOD" + lodNumber + "_" + ticks + ".asset";
                AssetDatabase.CreateAsset(generatedLodMesh, path);
                return path;
            }
            return "";
#endif
#if !UNITY_EDITOR
            return "";
#endif
        }

        private Mesh GetGeneratedLodForThisMesh(Mesh originalMesh, float percentOfVertices, bool isSkinnedMesh)
        {
            //Simplification multiplier
            float multiplier = 0.00001f;

            //Return the mesh converted to LOD
            MeshSimplifier.MeshSimplifier meshSimplifier = new MeshSimplifier.MeshSimplifier();
            MeshSimplifier.SimplificationOptions meshSimplificationSettings = new MeshSimplifier.SimplificationOptions();
            switch (forceOfSimplification)
            {
                case ForceOfSimplification.Normal:
                    meshSimplificationSettings.Agressiveness = 7.0f;
                    multiplier = 1.0f;
                    break;
                case ForceOfSimplification.Strong:
                    meshSimplificationSettings.Agressiveness = 8.5f;
                    multiplier = 0.8f;
                    break;
                case ForceOfSimplification.VeryStrong:
                    meshSimplificationSettings.Agressiveness = 10.0f;
                    multiplier = 0.6f;
                    break;
                case ForceOfSimplification.ExtremelyStrong:
                    meshSimplificationSettings.Agressiveness = 12.0f;
                    multiplier = 0.4f;
                    break;
                case ForceOfSimplification.Destroyer:
                    meshSimplificationSettings.Agressiveness = 14.0f;
                    multiplier = 0.2f;
                    break;
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
            meshSimplifier.Initialize(originalMesh);
            meshSimplifier.SimplifyMesh((percentOfVertices / 100.0f) * multiplier);
            Mesh resultMesh = meshSimplifier.ToMesh();
            if (optimizeResultingMeshes == true)
                resultMesh.Optimize();
            if (isSkinnedMesh == true && skinnedAnimsCompatibilityMode == true)
                resultMesh.bindposes = originalMesh.bindposes;
#if UNITY_EDITOR
            if (isSkinnedMesh == false && enableLightmapsSupport == true)
                Unwrapping.GenerateSecondaryUVSet(resultMesh);
#endif
            return resultMesh;
        }

        private Material[] GetCopyOfExistentArrayOfMaterials(Material[] sourceArray)
        {
            //Get a copy of existent array of materials
            Material[] copy = new Material[sourceArray.Length];
            for (int i = 0; i < sourceArray.Length; i++)
                copy[i] = sourceArray[i];
            return copy;
        }

        private void ScanForMeshesAndGenerateAllLodGroups_StartProcessing(bool showProgressBar)
        {
            //Start coroutine to scan and process meshes (if is called on runtime, procces along of various frames)
            StartCoroutine(ScanForMeshesAndGenerateAllLodGroups_AsyncProcessing(showProgressBar));
        }

        private IEnumerator ScanForMeshesAndGenerateAllLodGroups_AsyncProcessing(bool showProgressBar)
        {
            //This methods generate meshes LODs along of various frames and done the scan process

            //Validate all parameters
            ValidateAllParameters(true);

#if UNITY_EDITOR
            //Show progressbar
            if (showProgressBar == true)
                EditorUtility.DisplayProgressBar("Scanning...", "Please wait...", 0.0f);
#endif

            //Storage all valid meshes found
            List<SkinnedMeshRenderer> skinnedMeshRenderersFound = new List<SkinnedMeshRenderer>();
            List<MeshFilter> meshFiltersFound = new List<MeshFilter>();
            bool hasMeshesFound = true;

            //Get all meshes
            if (modeOfMeshesScanning == ScanMeshesMode.ScanInChildrenGameObjectsOnly)
            {
                //If scan in children is desired
                SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(scanInactiveGameObjects);
                foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
                    //Check all conditions before add this mesh as valid
                    if (renderer != null && gameObjectsToIgnore.Contains(renderer.gameObject) == false)
                        if (renderer.gameObject.GetComponent<UltimateLevelOfDetail>() == null)
                            if (renderer.sharedMesh != null)
                                skinnedMeshRenderersFound.Add(renderer);

                MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(scanInactiveGameObjects);
                foreach (MeshFilter filter in meshFilters)
                    //Check all conditions before add this mesh as valid
                    if (filter != null && gameObjectsToIgnore.Contains(filter.gameObject) == false)
                        if (filter.gameObject.GetComponent<UltimateLevelOfDetail>() == null)
                            if (filter.sharedMesh != null)
                                meshFiltersFound.Add(filter);
            }
            if (modeOfMeshesScanning == ScanMeshesMode.ScanInThisGameObjectOnly)
            {
                //If scan in childer is not desire
                SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                    skinnedMeshRenderersFound.Add(skinnedMeshRenderer);
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                if (meshFilter != null)
                    meshFiltersFound.Add(meshFilter);
            }

            //Cancel if not found meshes
            if (skinnedMeshRenderersFound.Count == 0 && meshFiltersFound.Count == 0)
            {
#if UNITY_EDITOR
                EditorUtility.ClearProgressBar();
#endif
                Debug.Log("No mesh was found in GameObject \"" + this.gameObject.name + "\" or its child GameObjects. Please check the GameObjects hierarchy and the settings for this component and try again.");
                hasMeshesFound = false;
            }

            //If found meshes, continue to process and generate all lods
            if (hasMeshesFound == true)
            {
                //Create list of ScannedMeshItems
                List<ScannedMeshItem> scannedMeshItems = new List<ScannedMeshItem>();

                //Create folder to store LODs in project
                CreateHierarchyOfFoldersIfNotExists();

                //Create progress stats
                float totalMeshes = skinnedMeshRenderersFound.Count + meshFiltersFound.Count;
                float totalLodsProgress = ((skinnedMeshRenderersFound.Count * GetNumberOfLodsGenerated()) + (meshFiltersFound.Count * GetNumberOfLodsGenerated()));
                float currentMesh = 0;
                float currentLod = 0;
                string lightmapSuffix = ((enableLightmapsSupport == true) ? " with lightmaps" : "");

                //Add first, skinned mesh renderers
                foreach (SkinnedMeshRenderer smr in skinnedMeshRenderersFound)
                {
                    //Get current date
                    DateTime date = DateTime.UtcNow;
                    long ticks = date.Ticks + (long)currentMesh;
                    currentMesh += 1;

                    //Create a new ScannedMeshItem object to store all data about this mesh, and scan info, and fill it
                    ScannedMeshItem thisScannedMeshItem = new ScannedMeshItem();
                    thisScannedMeshItem.originalGameObject = smr.gameObject;
                    thisScannedMeshItem.originalSkinnedMeshRenderer = smr;
                    thisScannedMeshItem.allMeshLods[0] = smr.sharedMesh;
                    thisScannedMeshItem.InitializeAllMeshLodsMaterialsArray();
                    thisScannedMeshItem.canChangeMaterialsOnThisMeshLods = enableMaterialsChanges;
                    thisScannedMeshItem.allMeshLodsMaterials[0].materialArray = GetCopyOfExistentArrayOfMaterials(smr.sharedMaterials);
#if UNITY_EDITOR
                    thisScannedMeshItem.allMeshLodsPaths[0] = AssetDatabase.GetAssetPath(smr.sharedMesh);
#endif

                    //Create the ULODMeshes for this Mesh GameObject
                    thisScannedMeshItem.originalMeshLodsManager = thisScannedMeshItem.originalGameObject.AddComponent<UltimateLevelOfDetailMeshes>();
                    thisScannedMeshItem.originalMeshLodsManager.responsibleUlod = this;
                    thisScannedMeshItem.originalMeshLodsManager.idOfOriginalMeshItemOfThisInResponsibleUlod = (int)currentMesh - 1;

                    //Process each mesh according to quantity of LODs desired
                    for (int i = 1; i <= levelsOfDetailToGenerate; i++)
                        if (levelsOfDetailToGenerate >= i)
                        {
                            //Wait for end of 2 frames before proccess this mesh (if this is called in runtime)
                            if (Application.isPlaying == true)
                            {
                                yield return WAIT_FOR_END_OF_FRAME;
                                yield return WAIT_FOR_END_OF_FRAME;
                            }

#if UNITY_EDITOR
                            //Show/update progress bar
                            if (showProgressBar == true)
                                EditorUtility.DisplayProgressBar("Please wait...", "Generating LOD Group (For Mesh " + currentMesh + "/" + totalMeshes + lightmapSuffix + ")", currentLod / totalLodsProgress);
#endif

                            //Proccess this mesh
                            thisScannedMeshItem.allMeshLods[i] = GetGeneratedLodForThisMesh(smr.sharedMesh, percentOfVerticesForEachLod[i], true);
                            thisScannedMeshItem.allMeshLodsPaths[i] = SaveGeneratedLodInAssets(i.ToString(), ticks, thisScannedMeshItem.allMeshLods[i]);
                            thisScannedMeshItem.allMeshLodsMaterials[i].materialArray = GetCopyOfExistentArrayOfMaterials(smr.sharedMaterials);

                            //Increase the current lod counter by one
                            currentLod += 1;
                        }

                    scannedMeshItems.Add(thisScannedMeshItem);
                }

                //Add, mesh filters
                foreach (MeshFilter mf in meshFiltersFound)
                {
                    //Get current date
                    DateTime date = DateTime.UtcNow;
                    long ticks = date.Ticks + (long)currentMesh;
                    currentMesh += 1;

                    //Create a new ScannedMeshItem object to store all data about this mesh, and scan info, and fill it
                    ScannedMeshItem thisScannedMeshItem = new ScannedMeshItem();
                    thisScannedMeshItem.originalGameObject = mf.gameObject;
                    thisScannedMeshItem.originalMeshFilter = mf;
                    thisScannedMeshItem.originalMeshRenderer = mf.GetComponent<MeshRenderer>();
                    thisScannedMeshItem.allMeshLods[0] = mf.sharedMesh;
                    thisScannedMeshItem.InitializeAllMeshLodsMaterialsArray();
                    thisScannedMeshItem.canChangeMaterialsOnThisMeshLods = enableMaterialsChanges;
                    thisScannedMeshItem.allMeshLodsMaterials[0].materialArray = GetCopyOfExistentArrayOfMaterials(thisScannedMeshItem.originalMeshRenderer.sharedMaterials);
#if UNITY_EDITOR
                    thisScannedMeshItem.allMeshLodsPaths[0] = AssetDatabase.GetAssetPath(mf.sharedMesh);
#endif

                    //Create the ULODMeshes for this Mesh GameObject
                    thisScannedMeshItem.originalMeshLodsManager = thisScannedMeshItem.originalGameObject.AddComponent<UltimateLevelOfDetailMeshes>();
                    thisScannedMeshItem.originalMeshLodsManager.responsibleUlod = this;
                    thisScannedMeshItem.originalMeshLodsManager.idOfOriginalMeshItemOfThisInResponsibleUlod = (int)currentMesh - 1;

                    //Process each loop according to quantity of LODs desired
                    for (int i = 1; i <= levelsOfDetailToGenerate; i++)
                        if (levelsOfDetailToGenerate >= i)
                        {
                            //Wait two frames before proccess this mesh
                            if (Application.isPlaying == true)
                            {
                                yield return WAIT_FOR_END_OF_FRAME;
                                yield return WAIT_FOR_END_OF_FRAME;
                            }

#if UNITY_EDITOR
                            //Show/update progress bar
                            if (showProgressBar == true)
                                EditorUtility.DisplayProgressBar("Please wait...", "Generating LOD Group (For Mesh " + currentMesh + "/" + totalMeshes + lightmapSuffix + ")", currentLod / totalLodsProgress);
#endif

                            //Proccess this mesh
                            thisScannedMeshItem.allMeshLods[i] = GetGeneratedLodForThisMesh(mf.sharedMesh, percentOfVerticesForEachLod[i], true);
                            thisScannedMeshItem.allMeshLodsPaths[i] = SaveGeneratedLodInAssets(i.ToString(), ticks, thisScannedMeshItem.allMeshLods[i]);
                            thisScannedMeshItem.allMeshLodsMaterials[i].materialArray = GetCopyOfExistentArrayOfMaterials(thisScannedMeshItem.originalMeshRenderer.sharedMaterials);

                            //Increase the current lod counter by one
                            currentLod += 1;
                        }

                    scannedMeshItems.Add(thisScannedMeshItem);
                }

                //Save the list
                currentScannedMeshesList = scannedMeshItems;
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif

                //Call event of start scan
                if (Application.isPlaying == true && onDoneScan != null)
                    onDoneScan.Invoke();

                //Delete progressbar and finalize
#if UNITY_EDITOR
                if (showProgressBar == true)
                    EditorUtility.ClearProgressBar();
#endif
                Debug.Log("All meshes present in GameObject \"" + this.gameObject.name + "\" or its child GameObjects have been scanned and have groups of LODs created automatically based on the parameters you have defined. Now just move the camera away and watch the magic of optimization happen!");

                //Return null

                if (Application.isPlaying == false)
                    yield return null;
            }
        }

        private void UndoAllMeshesScannedAndAllLodGroups(bool showProgressBar, bool deleteAllGeneratedMeshes, bool runMonoIl2CppGc, bool runUnityGc)
        {
#if UNITY_EDITOR
            //Show progressbar
            if (showProgressBar == true)
                EditorUtility.DisplayProgressBar("Deleting...", "Please wait...", 1.0f);
#endif

            //Delete all data and reset currentScannedMeshesList
            foreach (ScannedMeshItem meshItem in currentScannedMeshesList)
            {
                //If is desired to save, delete all assets
                if (saveGeneratedLodsInAssets == true && deleteAllGeneratedMeshes == true)
                {
#if UNITY_EDITOR
                    for (int i = 0; i < meshItem.allMeshLodsPaths.Length; i++)
                    {
                        if (i == 0)
                            continue;

                        if (string.IsNullOrEmpty(meshItem.allMeshLodsPaths[i]) == false)
                            AssetDatabase.DeleteAsset(meshItem.allMeshLodsPaths[i]);
                    }
#endif
                }

                //Restore original mesh and materials
                if (meshItem.originalSkinnedMeshRenderer != null)
                {
                    meshItem.originalSkinnedMeshRenderer.sharedMesh = meshItem.allMeshLods[0];
                    if (meshItem.canChangeMaterialsOnThisMeshLods == true)
                        meshItem.originalSkinnedMeshRenderer.sharedMaterials = meshItem.allMeshLodsMaterials[0].materialArray;
                }
                if (meshItem.originalMeshFilter != null && meshItem.originalMeshRenderer != null)
                {
                    meshItem.originalMeshFilter.sharedMesh = meshItem.allMeshLods[0];
                    if (meshItem.canChangeMaterialsOnThisMeshLods == true)
                        meshItem.originalMeshRenderer.sharedMaterials = meshItem.allMeshLodsMaterials[0].materialArray;
                }

                //Delete all ULODMeshes
                if (meshItem.originalMeshLodsManager != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(meshItem.originalMeshLodsManager);
#endif
#if !UNITY_EDITOR
                    Destroy(meshItem.originalMeshLodsManager);
#endif
                }
            }

            //Delete all
            currentScannedMeshesList.Clear();
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            //Run the GC if is activated
            if (runMonoIl2CppGc == true)
            {
                System.GC.Collect();
            }
            if (runUnityGc == true)
            {
                Resources.UnloadUnusedAssets();
            }

            //Restore important variables values
            lastDistanceFromMainCamera = -1f;
            currentLodAccordingToDistance = -1;

            //Call event of undo scan
            if (Application.isPlaying == true && onUndoScan != null)
                onUndoScan.Invoke();

            //Delete progressbar and finalize
#if UNITY_EDITOR
            if (showProgressBar == true)
                EditorUtility.ClearProgressBar();
#endif
            Debug.Log("All scanned meshes in GameObject \"" + this.gameObject.name + "\" were restored to the original meshes. The scan was undone.");
        }

        //Tools methods for OnRenderObject

        private bool isLodsSimulationEnabledInThisSceneForEditorSceneViewMode()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return isLodSimulationEnabledInThisSceneForEditorSceneViewMode;
            if (Application.isPlaying == true)
                return true;
            return true;
#endif
#if !UNITY_EDITOR
            return true;
#endif
        }

        private Camera GetLastActiveSceneViewCamera()
        {
#if UNITY_EDITOR
            //Return the scene camera, if is editor
            if (cacheOfLastActiveSceneViewCamera == null && SceneView.lastActiveSceneView != null)
                cacheOfLastActiveSceneViewCamera = SceneView.lastActiveSceneView.camera;
            return cacheOfLastActiveSceneViewCamera;
#endif
#if !UNITY_EDITOR
            //Return null, if is the build
            return null;
#endif
        }

        //Universal Methods of lod simulation

        private void CullThisLodMeshOfRenderer(ScannedMeshItem meshItem)
        {
            //If culling is disabled, return
            if (cullingMode == CullingMode.Disabled)
                return;

            //If culling method is "CullingMeshes"
            if (cullingMode == CullingMode.CullingMeshes)
            {
                if (meshItem.originalSkinnedMeshRenderer != null)
                    if (meshItem.originalSkinnedMeshRenderer.sharedMesh != null)
                    {
                        meshItem.beforeCullingData_lastMeshOfThis = meshItem.originalSkinnedMeshRenderer.sharedMesh;
                        meshItem.originalSkinnedMeshRenderer.sharedMesh = null;
                    }
                if (meshItem.originalMeshFilter != null)
                    if (meshItem.originalMeshFilter.sharedMesh != null)
                    {
                        meshItem.beforeCullingData_lastMeshOfThis = meshItem.originalMeshFilter.sharedMesh;
                        meshItem.originalMeshFilter.sharedMesh = null;
                    }
            }

            //If culling method is "CullingRenderer"
            if (cullingMode == CullingMode.CullingRenderer)
            {
                if (meshItem.originalSkinnedMeshRenderer != null)
                {
                    meshItem.beforeCullingData_isForcedToRenderizationOff = meshItem.originalSkinnedMeshRenderer.forceRenderingOff;
                    meshItem.originalSkinnedMeshRenderer.forceRenderingOff = true;
                }
                if (meshItem.originalMeshRenderer != null)
                {
                    meshItem.beforeCullingData_isForcedToRenderizationOff = meshItem.originalMeshRenderer.forceRenderingOff;
                    meshItem.originalMeshRenderer.forceRenderingOff = true;
                }
            }
        }

        private void UncullThisLodMeshOfRenderer(ScannedMeshItem meshItem)
        {
            //If culling is disabled, return, if is playing
            if (cullingMode == CullingMode.Disabled)
                return;

            //If culling method is "CullingMeshes"
            if (cullingMode == CullingMode.CullingMeshes)
            {
                if (meshItem.originalSkinnedMeshRenderer != null)
                    if (meshItem.originalSkinnedMeshRenderer.sharedMesh == null)
                        meshItem.originalSkinnedMeshRenderer.sharedMesh = meshItem.beforeCullingData_lastMeshOfThis;
                if (meshItem.originalMeshFilter != null)
                    if (meshItem.originalMeshFilter.sharedMesh == null)
                        meshItem.originalMeshFilter.sharedMesh = meshItem.beforeCullingData_lastMeshOfThis;
            }

            //If culling method is "CullingRenderer"
            if (cullingMode == CullingMode.CullingRenderer)
            {
                if (meshItem.originalSkinnedMeshRenderer != null)
                    meshItem.originalSkinnedMeshRenderer.forceRenderingOff = meshItem.beforeCullingData_isForcedToRenderizationOff;

                if (meshItem.originalMeshRenderer != null)
                    meshItem.originalMeshRenderer.forceRenderingOff = meshItem.beforeCullingData_isForcedToRenderizationOff;
            }
        }

        private void ChangeLodMeshAndMaterialsOfRenderer(ScannedMeshItem meshItem, int lodLevel)
        {
            //If is a Skinned Mesh Renderer
            if (meshItem.originalSkinnedMeshRenderer != null)
            {
                //Prepare the change of lods skinned parameter in editor
                bool canChangeLods = false;
                if (Application.isEditor == false)
                    canChangeLods = true;
                if (Application.isEditor == true && forceChangeLodsOfSkinnedInEditor == true)
                    canChangeLods = true;

                //Change the mesh, according to the current required lod level
                if (canChangeLods == true)
                {
                    for (int i = 0; i < 9; i++)
                        if (i == lodLevel)
                        {
                            meshItem.originalSkinnedMeshRenderer.sharedMesh = meshItem.allMeshLods[i];
                            if (meshItem.canChangeMaterialsOnThisMeshLods == true)
                                meshItem.originalSkinnedMeshRenderer.sharedMaterials = meshItem.allMeshLodsMaterials[i].materialArray;
                        }

                    //Enable and disable mesh to avoid log of errors and artifacts on update mesh bones data
                    meshItem.originalSkinnedMeshRenderer.enabled = false;
                    meshItem.originalSkinnedMeshRenderer.enabled = true;
                }
            }

            //If is a Mesh Renderer and Mesh Filter
            if (meshItem.originalMeshFilter != null && meshItem.originalMeshRenderer != null)
            {
                //Change the mesh, according to the current required lod level
                for (int i = 0; i < 9; i++)
                    if (i == lodLevel)
                    {
                        meshItem.originalMeshFilter.sharedMesh = meshItem.allMeshLods[i];
                        if (meshItem.canChangeMaterialsOnThisMeshLods == true)
                            meshItem.originalMeshRenderer.sharedMaterials = meshItem.allMeshLodsMaterials[i].materialArray;
                    }
            }
        }

        private void CalculateCorrectLodForDistanceBeforeChange(float distance)
        {
            //If not have a scan, cancel
            if (currentScannedMeshesList.Count == 0)
                return;

            //Change the meshs according to distance of current main camera
            if (lastDistanceFromMainCamera != distance)
            {
                //Pre calculate the lod for apply for meshes
                int lodLevelForApplyInThisMeshes = -1;

                //Original LOD
                if (distance < minDistanceOfViewForEachLod[1])
                    lodLevelForApplyInThisMeshes = 0;
                //LOD 1
                if (distance >= minDistanceOfViewForEachLod[1])
                    lodLevelForApplyInThisMeshes = 1;
                //LOD 2
                if (levelsOfDetailToGenerate >= 2)
                    if (distance >= minDistanceOfViewForEachLod[2])
                        lodLevelForApplyInThisMeshes = 2;
                //LOD 3
                if (levelsOfDetailToGenerate >= 3)
                    if (distance >= minDistanceOfViewForEachLod[3])
                        lodLevelForApplyInThisMeshes = 3;
                //LOD 4
                if (levelsOfDetailToGenerate >= 4)
                    if (distance >= minDistanceOfViewForEachLod[4])
                        lodLevelForApplyInThisMeshes = 4;
                //LOD 5
                if (levelsOfDetailToGenerate >= 5)
                    if (distance >= minDistanceOfViewForEachLod[5])
                        lodLevelForApplyInThisMeshes = 5;
                //LOD 6
                if (levelsOfDetailToGenerate >= 6)
                    if (distance >= minDistanceOfViewForEachLod[6])
                        lodLevelForApplyInThisMeshes = 6;
                //LOD 7
                if (levelsOfDetailToGenerate >= 7)
                    if (distance >= minDistanceOfViewForEachLod[7])
                        lodLevelForApplyInThisMeshes = 7;
                //LOD 8
                if (levelsOfDetailToGenerate >= 8)
                    if (distance >= minDistanceOfViewForEachLod[8])
                        lodLevelForApplyInThisMeshes = 8;
                //Cull (Only occurs if Culling Mode not is disabled)
                if (cullingMode != CullingMode.Disabled)
                    if (distance >= minDistanceOfViewForCull)
                        lodLevelForApplyInThisMeshes = 9;

                //Apply the lod level pre calculated, if is not culling
                if (lodLevelForApplyInThisMeshes >= 0 && lodLevelForApplyInThisMeshes < 9)
                    if (currentLodAccordingToDistance != lodLevelForApplyInThisMeshes)
                        foreach (ScannedMeshItem meshItem in currentScannedMeshesList)
                        {
                            UncullThisLodMeshOfRenderer(meshItem);
                            ChangeLodMeshAndMaterialsOfRenderer(meshItem, lodLevelForApplyInThisMeshes);
                            currentLodAccordingToDistance = lodLevelForApplyInThisMeshes;
                        }
                //If the distance is needed, and culling enabled, cull meshes
                if (lodLevelForApplyInThisMeshes == 9)
                    if (currentLodAccordingToDistance != 9)
                        foreach (ScannedMeshItem meshItem in currentScannedMeshesList)
                        {
                            CullThisLodMeshOfRenderer(meshItem);
                            currentLodAccordingToDistance = 9;
                        }

                //Set the new last distance from camera
                lastDistanceFromMainCamera = distance;
            }
        }

        public void OnRenderObject()
        {
            //If ulod system is disabled, cancel auto management and restore original meshes
            if (UltimateLevelOfDetailGlobal.isGlobalULodSystemEnabled() == false)
            {
                CalculateCorrectLodForDistanceBeforeChange(0);
                return;
            }
            //If this ulod component is forced to be disabled, restore original meshes and stop
            if (forcedToDisableLodsOfThisComponent == true)
            {
                CalculateCorrectLodForDistanceBeforeChange(0);
                return;
            }
            //If LODs simulation is disabled in this scene, restore original meshes [This only works in editor, in scene edit mode]
            if (isLodsSimulationEnabledInThisSceneForEditorSceneViewMode() == false)
            {
                CalculateCorrectLodForDistanceBeforeChange(0);
                return;
            }

            //Get correct camera, according to parameter defined
            Camera lastActiveSceneViewCamera = GetLastActiveSceneViewCamera();
            Camera cameraToCalcDistance = null;

            //If is mode "MainCamera"
            if (cameraDetectionMode == CameraDetectionMode.MainCamera && Application.isPlaying == true)
            {
                //If cache is disabled, get the main camera in each renderization
                if (useCacheForMainCameraInDetection == false)
                    cameraToCalcDistance = Camera.main;
                //If cache is enabled, get the main camera of cache, if is possible
                if (useCacheForMainCameraInDetection == true)
                {
                    //Invalid the cache if camera of cache is disabled or in a gameobject disabled
                    if (cacheOfMainCamera != null)
                        if (cacheOfMainCamera.enabled == false || cacheOfMainCamera.gameObject.activeSelf == false || cacheOfMainCamera.gameObject.activeInHierarchy == false)
                            cacheOfMainCamera = null;
                    //If cache is null or invalid, try to find another main camera to fill the ache
                    if (cacheOfMainCamera == null)
                        cacheOfMainCamera = Camera.main;
                    //If cache is valid or not null, get the camera of cache
                    if (cacheOfMainCamera != null)
                        cameraToCalcDistance = cacheOfMainCamera;
                }
                //If is not possible to get the main camera
                if (cameraToCalcDistance == null)
                    Debug.LogError("It was not possible to find a main camera to calculate LODs. Please make sure that the main camera in your scene has the \"MainCamera\" tag defined in the GameObject in which it is located.");
            }

            //If is mode "CurrentCamera"
            if (cameraDetectionMode == CameraDetectionMode.CurrentCamera && Application.isPlaying == true)
            {
                //Get the current camera determined by the componente "Ultimate LOD Data" in this scene
                cameraToCalcDistance = UltimateLevelOfDetailGlobal.currentCameraThatIsOnTopOfScreenInThisScene;
                //If the current camera on top of screen is null, or component "Ultimate LOD Data" impossible to find a camera
                if (cameraToCalcDistance == null)
                    Debug.LogError("It was not possible to find a current camera at the moment, it seems that there are no cameras in the scene, or Unity was unable to make references. Please try to switch to \"Main Camera\" mode.");
            }

            //If is mode "CustomCamera"
            if (cameraDetectionMode == CameraDetectionMode.CustomCamera && Application.isPlaying == true)
            {
                //Get the custom camera determined by user
                cameraToCalcDistance = customCameraForSimulationOfLods;
                //If is not possible to get custom camera
                if (cameraToCalcDistance == null)
                    Debug.LogError("No custom camera for calculating distance and simulating LODs has been provided in \"" + this.gameObject.name + "\".");
            }

            //If is not playing, and in editor, get the camera of scene view
            if (Application.isEditor == true && Application.isPlaying == false)
            {
                cameraToCalcDistance = lastActiveSceneViewCamera;
                if (cameraToCalcDistance == null)
                    Debug.LogError("It was not possible to find a camera that is currently viewing a scene. Make sure the scene view window is active and in focus.");
            }
            //Start the calc of distance to change LODs according to the camera detected
            if (cameraToCalcDistance != null)
            {
                //If not have a custom pivot, use this gameobject position to calculate distance
                if (_customPivotToSimulateLods == null)
                {
                    currentDistanceFromMainCamera = (Vector3.Distance(this.gameObject.transform.position, cameraToCalcDistance.transform.position) * UltimateLevelOfDetailGlobal.GetGlobalLodDistanceMultiplier());
                    currentRealDistanceFromMainCamera = Vector3.Distance(this.gameObject.transform.position, cameraToCalcDistance.transform.position);
                }
                //If have a custom pivot, use pivot position to calculate distance
                if (_customPivotToSimulateLods != null)
                {
                    currentDistanceFromMainCamera = (Vector3.Distance(_customPivotToSimulateLods.position, cameraToCalcDistance.transform.position) * UltimateLevelOfDetailGlobal.GetGlobalLodDistanceMultiplier());
                    currentRealDistanceFromMainCamera = Vector3.Distance(_customPivotToSimulateLods.position, cameraToCalcDistance.transform.position);
                }
            }
            if (cameraToCalcDistance == null)
            {
                currentDistanceFromMainCamera = 0;
                currentRealDistanceFromMainCamera = 0;
            }

            //Start lod calculation with the current distance
            CalculateCorrectLodForDistanceBeforeChange(currentDistanceFromMainCamera);
        }

        public void Awake()
        {
            //Start the game setting original mesh lod 0
            CalculateCorrectLodForDistanceBeforeChange(0);

            //Try to find Ultimate LOD Data
            GameObject ulodData = GameObject.Find("Ultimate LOD Data");

            //If Ultimate LOD Data exists in this scene, register this ulod instance
            if (ulodData != null && Application.isPlaying == true)
            {
                ulodData.GetComponent<RuntimeInstancesDetector>().instancesOfUlodInThisScene.Add(this);
                cacheOfUlodData = ulodData;
                cacheOfUlodDataRuntimeInstancesDetector = ulodData.GetComponent<RuntimeInstancesDetector>();
            }
            //Create the Ultimate LOD Data GameObject in this scene, if not exists
            if (ulodData == null && Application.isPlaying == true)
            {
                ulodData = new GameObject("Ultimate LOD Data");
                ulodData.transform.position = new Vector3(0, 0, 0);
                RuntimeCameraDetector runtimeCameraDetector = ulodData.AddComponent<RuntimeCameraDetector>();
                RuntimeInstancesDetector runtimeInstancesDetector = ulodData.AddComponent<RuntimeInstancesDetector>();
                runtimeInstancesDetector.instancesOfUlodInThisScene.Add(this);
                cacheOfUlodData = ulodData;
                cacheOfUlodDataRuntimeInstancesDetector = ulodData.GetComponent<RuntimeInstancesDetector>();
            }
        }

        //API methods

        public int GetCurrentLodLevel()
        {
            //Return the current lod level of this
            if (currentLodAccordingToDistance != 9)
                return currentLodAccordingToDistance;
            if (currentLodAccordingToDistance == 9)
                return (GetNumberOfLodsGenerated() - 1);
            return 0;
        }

        public float GetCurrentCameraDistance()
        {
            //return the current camera distance from this object
            return currentDistanceFromMainCamera;
        }

        public float GetCurrentRealCameraDistance()
        {
            //return the current camera distance from this object, without multiplier
            return currentRealDistanceFromMainCamera;
        }

        public int GetNumberOfLodsGenerated()
        {
            //Count and return the number of LODs generated in this component
            int count = 1;
            if (levelsOfDetailToGenerate >= 2)
                count += 1;
            if (levelsOfDetailToGenerate >= 3)
                count += 1;
            if (levelsOfDetailToGenerate >= 4)
                count += 1;
            if (levelsOfDetailToGenerate >= 5)
                count += 1;
            if (levelsOfDetailToGenerate >= 6)
                count += 1;
            if (levelsOfDetailToGenerate >= 7)
                count += 1;
            if (levelsOfDetailToGenerate >= 8)
                count += 1;
            return count;
        }

        public bool isScannedMeshesCurrentCulled()
        {
            //Return true if all meshes scanned by this ULOD, is culled
            if (currentLodAccordingToDistance == 9)
                return true;
            if (currentLodAccordingToDistance != 9)
                return false;
            return false;
        }

        public UltimateLevelOfDetailMeshes[] GetListOfAllMeshesScanned()
        {
            //Create the list of all meshes scanned
            List<UltimateLevelOfDetailMeshes> list = new List<UltimateLevelOfDetailMeshes>();
            foreach (ScannedMeshItem meshItem in currentScannedMeshesList)
                list.Add(meshItem.originalMeshLodsManager);
            return list.ToArray();
        }

        public void ForceDisableLodChangesInThisComponent(bool force)
        {
            //Force or not the disable of lod changes
            forcedToDisableLodsOfThisComponent = force;
        }

        public bool isThisComponentForcedToDisableLodChanges()
        {
            //Return true if is forced to disable lod changes
            return forcedToDisableLodsOfThisComponent;
        }

        public void ForceThisComponentToUpdateLodsRender()
        {
            //Force this component to update and change meshes, does not matter current distance, current lod and etc
            lastDistanceFromMainCamera = lastDistanceFromMainCamera + UnityEngine.Random.Range(0.1f, 1.0f);
            currentLodAccordingToDistance = -1;
            OnRenderObject();
        }

        public bool isMeshesCurrentScannedAndLodsWorkingInThisComponent()
        {
            //Return true if this component already scanned meshes and lods is working

            if (currentScannedMeshesList.Count == 0)
                return false;
            if (currentScannedMeshesList.Count > 0)
                return true;
            return false;
        }

        public void ScanAllMeshesAndGenerateLodsGroups()
        {
            //If already scanned
            if (isMeshesCurrentScannedAndLodsWorkingInThisComponent() == true)
            {
                Debug.LogError("It was not possible to start scanning meshes to generate LODs. Component in " + this.gameObject.name + " already has an active scan. It is necessary to undo the current scan before starting a new one.");
                return;
            }
            //If not scanned yet
            if (isMeshesCurrentScannedAndLodsWorkingInThisComponent() == false)
            {
                ScanForMeshesAndGenerateAllLodGroups_StartProcessing(false);
            }
        }

        public void UndoCurrentScanWorkingAndDeleteGeneratedMeshes(bool runMonoIl2CppGc, bool runUnityGc)
        {
            //If scan not exists
            if (isMeshesCurrentScannedAndLodsWorkingInThisComponent() == false)
            {
                Debug.LogError("It was not possible to undo the LODs scan existing in the " + this.gameObject.name + " component. There is no scan done yet, it is necessary to perform one before.");
                return;
            }
            //If not scanned yet
            if (isMeshesCurrentScannedAndLodsWorkingInThisComponent() == true)
            {
                UndoAllMeshesScannedAndAllLodGroups(false, true, runMonoIl2CppGc, runUnityGc);
            }
        }

        public UltimateLevelOfDetail[] GetListOfAllUlodsInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of ULODs in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all ulods in this scene
            return cacheOfUlodDataRuntimeInstancesDetector.instancesOfUlodInThisScene.ToArray();
        }

        public UltimateLevelOfDetailOptimizer[] GetListOfAllUlodsOptimizerInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of ULODs Optimizers in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all ulods optimizer in this scene
            return cacheOfUlodData.GetComponent<RuntimeInstancesDetector>().instancesOfUlodOptimizerInThisScene.ToArray();
        }

        public void SetNewCustomCameraForThisAndAllUlodsInThisScene(Camera newCustomCamera)
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is not possible to define a custom camera for all ULODs components in this scene. This method is only usable at run time.");
                return;
            }

            //Set a new custom câmera for this ulod and all ulods in this scene
            this.customCameraForSimulationOfLods = newCustomCamera;
            foreach (UltimateLevelOfDetail ulod in cacheOfUlodDataRuntimeInstancesDetector.instancesOfUlodInThisScene)
                ulod.customCameraForSimulationOfLods = newCustomCamera;
        }

        public void ConvertThisToDefaultUnityLods()
        {
            //This method will convert this Ultimate Level Of Detail component to default Unity LOD Group components

            //Loop in all current scanned meshes to run process, Create a new GameObject and put as child of this scanned item, for each LOD of this item. Process the LOD too
            foreach (ScannedMeshItem scannedItem in currentScannedMeshesList)
            {
                //Create the LOD Group component, for skinned mesh renderer and mesh renderer
                LODGroup lodGroup = null;
                List<LOD> generatedLods = new List<LOD>();
                Renderer[] renderersForLod0 = new Renderer[1];
                if (scannedItem.originalSkinnedMeshRenderer != null)
                {
                    lodGroup = scannedItem.originalSkinnedMeshRenderer.gameObject.AddComponent<LODGroup>();
                    renderersForLod0[0] = scannedItem.originalSkinnedMeshRenderer;
                }
                if (scannedItem.originalMeshRenderer != null && scannedItem.originalMeshFilter != null)
                {
                    lodGroup = scannedItem.originalMeshRenderer.gameObject.AddComponent<LODGroup>();
                    renderersForLod0[0] = scannedItem.originalMeshRenderer;
                }
                lodGroup.fadeMode = LODFadeMode.CrossFade;
                lodGroup.animateCrossFading = true;
                generatedLods.Add(new LOD(0.70f, renderersForLod0));

                //Process each generated LOD
                for (int i = 0; i <= levelsOfDetailToGenerate; i++)
                {
                    //If this is the LOD 0 (Original Mesh)
                    if (i == 0)
                        continue;

                    //If this is a LOD not original
                    //Create the new GameObject
                    GameObject thisLodObj = new GameObject("LOD " + i + " (Generated By ULOD)");

                    //Process the renderers
                    Renderer[] renderersForThisLod = new Renderer[1];
                    if (scannedItem.originalSkinnedMeshRenderer != null)
                    {
                        //Position the new GameObject
                        thisLodObj.transform.SetParent(scannedItem.originalSkinnedMeshRenderer.transform);
                        thisLodObj.transform.localPosition = Vector3.zero;
                        thisLodObj.transform.localEulerAngles = Vector3.zero;
                        thisLodObj.transform.localScale = Vector3.one;
                        //Add the skinned mesh renderer, copy the values from original skinned mesh renderer
                        SkinnedMeshRenderer meshRenderer = thisLodObj.AddComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = scannedItem.allMeshLods[i];
                        meshRenderer.bones = scannedItem.originalSkinnedMeshRenderer.bones;
                        meshRenderer.rootBone = scannedItem.originalSkinnedMeshRenderer.rootBone;
                        meshRenderer.updateWhenOffscreen = scannedItem.originalSkinnedMeshRenderer.updateWhenOffscreen;
                        meshRenderer.receiveShadows = scannedItem.originalSkinnedMeshRenderer.receiveShadows;
                        meshRenderer.shadowCastingMode = scannedItem.originalSkinnedMeshRenderer.shadowCastingMode;
                        meshRenderer.skinnedMotionVectors = scannedItem.originalSkinnedMeshRenderer.skinnedMotionVectors;
                        meshRenderer.allowOcclusionWhenDynamic = scannedItem.originalSkinnedMeshRenderer.allowOcclusionWhenDynamic;
                        meshRenderer.materials = scannedItem.allMeshLodsMaterials[i].materialArray;
                        //Add this renderer
                        renderersForThisLod[0] = meshRenderer;
                    }
                    if (scannedItem.originalMeshRenderer != null && scannedItem.originalMeshFilter != null)
                    {
                        //Position the new GameObject
                        thisLodObj.transform.SetParent(scannedItem.originalMeshRenderer.transform);
                        thisLodObj.transform.localPosition = Vector3.zero;
                        thisLodObj.transform.localEulerAngles = Vector3.zero;
                        thisLodObj.transform.localScale = Vector3.one;
                        //Add the mesh filter
                        MeshFilter meshFilter = thisLodObj.AddComponent<MeshFilter>();
                        meshFilter.mesh = scannedItem.allMeshLods[i];
                        //Add the mesh renderer, copy the values from original mesh renderer
                        MeshRenderer meshRenderer = thisLodObj.AddComponent<MeshRenderer>();
                        meshRenderer.materials = scannedItem.allMeshLodsMaterials[i].materialArray;
                        meshRenderer.shadowCastingMode = scannedItem.originalMeshRenderer.shadowCastingMode;
                        meshRenderer.receiveShadows = scannedItem.originalMeshRenderer.receiveShadows;
                        meshRenderer.motionVectorGenerationMode = scannedItem.originalMeshRenderer.motionVectorGenerationMode;
                        meshRenderer.allowOcclusionWhenDynamic = scannedItem.originalMeshRenderer.allowOcclusionWhenDynamic;
                        //Add this renderer
                        renderersForThisLod[0] = meshRenderer;
                    }

                    //Add this lod in LOD group
                    float screenRelativeTransitionHeight = 0.30f;
                    float dividedByLodsGenerated = 0.30f / levelsOfDetailToGenerate;
                    float finalSRTH = screenRelativeTransitionHeight - ((dividedByLodsGenerated * 0.97f) * i);
                    generatedLods.Add(new LOD(finalSRTH, renderersForThisLod));
                }

                //Save the generated GameObjects LODs in LOD Group
                lodGroup.SetLODs(generatedLods.ToArray());
                lodGroup.RecalculateBounds();
            }

            //Get name of this component
            string nameOfThisComponent = this.gameObject.name;
            //Undo the scan done
            UndoAllMeshesScannedAndAllLodGroups(false, false, true, true);
            //Remove this component
            if (Application.isPlaying == false)
                DestroyImmediate(this);
            if (Application.isPlaying == true)
                Destroy(this);

            //Send notification
            Debug.Log("The Ultimate Level Of Detail component in \"" + nameOfThisComponent + "\" has been removed and all scanned meshes are now managed by Unity's standard \"LOD Group\" components.");
        }
    }
}