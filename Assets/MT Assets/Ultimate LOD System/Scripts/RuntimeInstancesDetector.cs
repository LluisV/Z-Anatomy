#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.UltimateLODSystem
{
    /*
     This class is responsible for the functioning of the "Runtime Instances Detector" component, and all its functions.
    */
    /*
     * The Ultimate LOD System was developed by Marcos Tomaz in 2020.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class RuntimeInstancesDetector : MonoBehaviour
    {
        //Public variables
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<UltimateLevelOfDetail> instancesOfUlodInThisScene = new List<UltimateLevelOfDetail>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<UltimateLevelOfDetailOptimizer> instancesOfUlodOptimizerInThisScene = new List<UltimateLevelOfDetailOptimizer>();

#if UNITY_EDITOR
        //Public variables of Interface
        private bool gizmosOfThisComponentIsDisabled = false;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(RuntimeInstancesDetector))]
        public class CustomInspector : UnityEditor.Editor
        {
            //Private variables of Editor Only
            private Vector2 ulodInstancesOnThisSceneScrollpos = Vector2.zero;
            private Vector2 ulodOptimizersOnThisSceneScrollpos = Vector2.zero;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                RuntimeInstancesDetector script = (RuntimeInstancesDetector)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");
                script.gizmosOfThisComponentIsDisabled = MTAssetsEditorUi.DisableGizmosInSceneView("RuntimeInstancesDetector", script.gizmosOfThisComponentIsDisabled);

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Ultimate LOD System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);
                GUILayout.Space(10);

                //Ulod instances

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ULOD Instances On This Scene", GUILayout.Width(230));
                GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 230);
                EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                EditorGUILayout.IntField(script.instancesOfUlodInThisScene.Count, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                GUILayout.BeginVertical("box");
                ulodInstancesOnThisSceneScrollpos = EditorGUILayout.BeginScrollView(ulodInstancesOnThisSceneScrollpos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                //Original meshes list
                int i = 0;
                foreach (UltimateLevelOfDetail ulodItem in script.instancesOfUlodInThisScene)
                {
                    GUILayout.Space(2);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    if (ulodItem != null)
                        EditorGUILayout.LabelField(ulodItem.gameObject.name, EditorStyles.boldLabel);
                    if (ulodItem == null)
                        EditorGUILayout.LabelField("Instance Not Found");
                    GUILayout.Space(-3);
                    if (ulodItem == null)
                        EditorGUILayout.LabelField("Instance Not Found");
                    if (ulodItem != null)
                    {
                        if (ulodItem.gameObject.activeInHierarchy == false)
                            EditorGUILayout.LabelField("Instance " + i.ToString() + ". Disabled.");
                        if (ulodItem.gameObject.activeInHierarchy == true)
                            EditorGUILayout.LabelField("Instance " + i.ToString() + ". Enabled.");
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    GUILayout.Space(8);
                    if (ulodItem != null)
                        if (GUILayout.Button("Game Object", GUILayout.Height(20)))
                            EditorGUIUtility.PingObject(ulodItem.gameObject);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    i += 1;
                }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();

                //Ulod optimizers

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ULOD Optimizers On This Scene", GUILayout.Width(230));
                GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 230);
                EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                EditorGUILayout.IntField(script.instancesOfUlodOptimizerInThisScene.Count, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                GUILayout.BeginVertical("box");
                ulodOptimizersOnThisSceneScrollpos = EditorGUILayout.BeginScrollView(ulodOptimizersOnThisSceneScrollpos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                //Original meshes list
                foreach (UltimateLevelOfDetailOptimizer optimizerItem in script.instancesOfUlodOptimizerInThisScene)
                {
                    GUILayout.Space(2);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    if (optimizerItem != null)
                        EditorGUILayout.LabelField(optimizerItem.gameObject.name, EditorStyles.boldLabel);
                    if (optimizerItem == null)
                        EditorGUILayout.LabelField("Optimizer Not Found");
                    GUILayout.Space(-3);
                    if (optimizerItem == null)
                        EditorGUILayout.LabelField("Optimizer Not Found");
                    if (optimizerItem != null)
                    {
                        if (optimizerItem.enableOptimizationTasks == false)
                            EditorGUILayout.LabelField("Disabled");
                        if (optimizerItem.enableOptimizationTasks == true)
                            EditorGUILayout.LabelField("Working");
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    GUILayout.Space(8);
                    if (optimizerItem != null)
                        if (GUILayout.Button("Game Object", GUILayout.Height(20)))
                            EditorGUIUtility.PingObject(optimizerItem.gameObject);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();

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

        //Component public methods

        public void RegisterNewUlodOptimizerInThisScene(UltimateLevelOfDetailOptimizer optimizer)
        {
            //Register a new optimizer in this scene, and if have more than one, notify
            instancesOfUlodOptimizerInThisScene.Add(optimizer);
            if (instancesOfUlodOptimizerInThisScene.Count > 1)
                Debug.LogWarning("It has been identified that there is more than one \"Ultimate Level Of Detail Optimizer\" component in this scene. It is highly recommended that there is only one active component in the scene to avoid optimization problems and conflicts.");
        }
    }
}