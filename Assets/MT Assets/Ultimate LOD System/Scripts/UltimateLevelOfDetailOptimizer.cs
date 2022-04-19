#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.UltimateLODSystem
{
    /*
     This class is responsible for the functioning of the "Ultimate Level Of Detail Optimizer" component, and all its functions.
    */
    /*
     * The Ultimate LOD System was developed by Marcos Tomaz in 2020.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Ultimate LOD System/Ultimate Level Of Detail Optimizer")] //Add this component in a category of addComponent menu
    public class UltimateLevelOfDetailOptimizer : MonoBehaviour
    {
        //Private constantes
        private WaitForSecondsRealtime DELAY_BETWEEN_OPTIMIZATION_UPDATES = new WaitForSecondsRealtime(0.20f);
        private WaitForSecondsRealtime DELAY_BETWEEN_GAMEOBJECTS_STATE_CHANGE = new WaitForSecondsRealtime(0.05f);
        private float ADITIONAL_CULLING_DISTANCE_OFFSET = 10;

        //Private variables
        private RuntimeInstancesDetector runtimeInstancesDetector;
        private int[] instructionsToMakeOnUlods = new int[0]; //-1 = do nothing, 0 = disable ulod, 1 = enable ulod, 2 = ignore and skip ulod

        //Public variables
        [HideInInspector]
        public bool enableOptimizationTasks = true;
        [HideInInspector]
        public List<UltimateLevelOfDetail> ulodsToBeIgnored = new List<UltimateLevelOfDetail>();

#if UNITY_EDITOR
        //Public variables of Interface
        private bool gizmosOfThisComponentIsDisabled = false;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(UltimateLevelOfDetailOptimizer))]
        public class CustomInspector : UnityEditor.Editor
        {
            //Private variables of Editor Only
            private Vector2 gameObjectsToIgnoreScrollpos = Vector2.zero;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                UltimateLevelOfDetailOptimizer script = (UltimateLevelOfDetailOptimizer)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");
                script.gizmosOfThisComponentIsDisabled = MTAssetsEditorUi.DisableGizmosInSceneView("UltimateLevelOfDetailOptimizer", script.gizmosOfThisComponentIsDisabled);

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Ultimate LOD System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Optimization Settings", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.enableOptimizationTasks = (bool)EditorGUILayout.Toggle(new GUIContent("Enable Optimization Tasks",
                                            "If this option is enabled, this component will optimize all ULODs in this scene, disabling GameObjects that contain ULODs that are too far away. Only ULODs with the Culling option enabled and currently with the meshes down will be disabled, but if this component approaches again at a distance where its meshes should be displayed again, the ULOD components will be activated again. Consult the documentation for more details on this optimization component."),
                                            script.enableOptimizationTasks);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Optimization Rules", EditorStyles.boldLabel);
                GUILayout.Space(10);

                Texture2D removeItemIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Ultimate LOD System/Editor/Images/Remove.png", typeof(Texture2D));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ULODs To Ignore On Optimization", GUILayout.Width(230));
                GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 230);
                EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                EditorGUILayout.IntField(script.ulodsToBeIgnored.Count, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                GUILayout.BeginVertical("box");
                gameObjectsToIgnoreScrollpos = EditorGUILayout.BeginScrollView(gameObjectsToIgnoreScrollpos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                if (script.ulodsToBeIgnored.Count == 0)
                    EditorGUILayout.HelpBox("Oops! No ULODs to be ignored has been registered! If you want to subscribe any, click the button below!", MessageType.Info);
                if (script.ulodsToBeIgnored.Count > 0)
                    for (int i = 0; i < script.ulodsToBeIgnored.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(removeItemIcon, GUILayout.Width(25), GUILayout.Height(16)))
                            script.ulodsToBeIgnored.RemoveAt(i);
                        script.ulodsToBeIgnored[i] = (UltimateLevelOfDetail)EditorGUILayout.ObjectField(new GUIContent("ULOD " + i.ToString(), "This ULOD will be ignored during the scan, if it has any mesh, it will not be scanned to have LODs.\n\nClick the button to the left if you want to remove this GameObject from the list."), script.ulodsToBeIgnored[i], typeof(UltimateLevelOfDetail), true, GUILayout.Height(16));
                        GUILayout.EndHorizontal();
                    }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add New Slot"))
                {
                    script.ulodsToBeIgnored.Add(null);
                    gameObjectsToIgnoreScrollpos.y += 999999;
                }
                if (script.ulodsToBeIgnored.Count > 0)
                    if (GUILayout.Button("Remove Empty Slots", GUILayout.Width(Screen.width * 0.48f)))
                        for (int i = script.ulodsToBeIgnored.Count - 1; i >= 0; i--)
                            if (script.ulodsToBeIgnored[i] == null)
                                script.ulodsToBeIgnored.RemoveAt(i);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

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
        }
        #endregion
#endif

        //Core methods

        public void Awake()
        {
            //Start the optimation loop
            StartCoroutine(UlodOptimizationLoop());
        }

        private IEnumerator UlodOptimizationLoop()
        {
            //Wait X milliseconds before starts the optimization loop
            yield return new WaitForSecondsRealtime(0.10f);

            //Find the "Ultimate LOD Data" and get the Runtime Instances Detector
            GameObject ulodData = GameObject.Find("Ultimate LOD Data");
            runtimeInstancesDetector = ulodData.GetComponent<RuntimeInstancesDetector>();

            //First, register this optimizator in Runtime Instaces Detector
            runtimeInstancesDetector.RegisterNewUlodOptimizerInThisScene(this);

            //Starts the optimization loop
            while (true == true)
            {
                yield return DELAY_BETWEEN_OPTIMIZATION_UPDATES;

                //If optimization of this component is enabled
                if (enableOptimizationTasks == true)
                {
                    //Check if instructionsToMakeOnUlods array is not equal to quantity of ulods in scene, re create the array
                    if (instructionsToMakeOnUlods.Length != runtimeInstancesDetector.instancesOfUlodInThisScene.Count)
                    {
                        instructionsToMakeOnUlods = new int[runtimeInstancesDetector.instancesOfUlodInThisScene.Count];
                        for (int i = 0; i < instructionsToMakeOnUlods.Length; i++)
                        {
                            if (ulodsToBeIgnored.Contains(runtimeInstancesDetector.instancesOfUlodInThisScene[i]) == true)
                                instructionsToMakeOnUlods[i] = 2;
                            if (ulodsToBeIgnored.Contains(runtimeInstancesDetector.instancesOfUlodInThisScene[i]) == false)
                                instructionsToMakeOnUlods[i] = -1;
                        }
                    }

                    //Check each ulod in this scene to enable, or disable each one
                    for (int i = 0; i < runtimeInstancesDetector.instancesOfUlodInThisScene.Count; i++)
                    {
                        if (runtimeInstancesDetector.instancesOfUlodInThisScene[i].cullingMode == UltimateLevelOfDetail.CullingMode.Disabled)
                            continue;
                        if (runtimeInstancesDetector.instancesOfUlodInThisScene[i].cullingMode != UltimateLevelOfDetail.CullingMode.Disabled)
                        {
                            //If this ULOD not have a scan
                            if (runtimeInstancesDetector.instancesOfUlodInThisScene[i].isMeshesCurrentScannedAndLodsWorkingInThisComponent() == false)
                                continue;
                            //If this ulod is to be ignored
                            if (instructionsToMakeOnUlods[i] == 2)
                                continue;

                            //If distance between this optimizer and this ulod, is major than the culling distance required by this ulod, disable the ulod
                            if (Vector3.Distance(this.gameObject.transform.position,
                                 runtimeInstancesDetector.instancesOfUlodInThisScene[i].transform.position) > runtimeInstancesDetector.instancesOfUlodInThisScene[i].minDistanceOfViewForCull + ADITIONAL_CULLING_DISTANCE_OFFSET)
                                if (runtimeInstancesDetector.instancesOfUlodInThisScene[i].gameObject.activeSelf == true)
                                    instructionsToMakeOnUlods[i] = 0;
                            //If distance between this optimizer and this ulod, is minor or equal than the culling distance required by this ulod, enable the ulod
                            if (Vector3.Distance(this.gameObject.transform.position,
                                 runtimeInstancesDetector.instancesOfUlodInThisScene[i].transform.position) <= runtimeInstancesDetector.instancesOfUlodInThisScene[i].minDistanceOfViewForCull + ADITIONAL_CULLING_DISTANCE_OFFSET)
                                if (runtimeInstancesDetector.instancesOfUlodInThisScene[i].gameObject.activeSelf == false)
                                    instructionsToMakeOnUlods[i] = 1;
                        }
                    }

                    //Finnally, follow instructions in the array of instructions, where disable or enable each ulod
                    for (int i = 0; i < runtimeInstancesDetector.instancesOfUlodInThisScene.Count; i++)
                    {
                        //If instruction is -1, do nothing
                        if (instructionsToMakeOnUlods[i] == -1)
                            continue;
                        //If instruction is 0, disable this ulod and reset instruction
                        if (instructionsToMakeOnUlods[i] == 0)
                        {
                            runtimeInstancesDetector.instancesOfUlodInThisScene[i].gameObject.SetActive(false);
                            instructionsToMakeOnUlods[i] = -1;
                        }
                        //If instruction is 1, enable this ulod and reset instruction
                        if (instructionsToMakeOnUlods[i] == 1)
                        {
                            runtimeInstancesDetector.instancesOfUlodInThisScene[i].gameObject.SetActive(true);
                            instructionsToMakeOnUlods[i] = -1;
                        }

                        yield return DELAY_BETWEEN_GAMEOBJECTS_STATE_CHANGE;
                    }
                }

                //If optimization of this component is disabled
                if (enableOptimizationTasks == false)
                    foreach (UltimateLevelOfDetail ulod in runtimeInstancesDetector.instancesOfUlodInThisScene)
                    {
                        ulod.gameObject.SetActive(true);
                        yield return DELAY_BETWEEN_GAMEOBJECTS_STATE_CHANGE;
                    }
            }
        }
    }
}