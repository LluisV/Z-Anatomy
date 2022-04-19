#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.UltimateLODSystem
{
    /*
     This class is responsible for the functioning of the "Ultimate Level Of Detail Pivot" component, and all its functions.
    */
    /*
     * The Ultimate LOD System was developed by Marcos Tomaz in 2020.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Ultimate LOD System/Ultimate Level Of Detail Pivot")] //Add this component in a category of addComponent menu
    public class UltimateLevelOfDetailPivot : MonoBehaviour
    {
        //Public variables
        [HideInInspector]
        public UltimateLevelOfDetail targetUlodToChangePivot = null;

#if UNITY_EDITOR
        //Public variables of Interface
        private bool gizmosOfThisComponentIsDisabled = false;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(UltimateLevelOfDetailPivot))]
        public class CustomInspector : UnityEditor.Editor
        {
            //Private variables of Editor Only
            private Vector2 gameObjectsToIgnoreScrollpos = Vector2.zero;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                UltimateLevelOfDetailPivot script = (UltimateLevelOfDetailPivot)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");
                script.gizmosOfThisComponentIsDisabled = MTAssetsEditorUi.DisableGizmosInSceneView("UltimateLevelOfDetailPivot", script.gizmosOfThisComponentIsDisabled);

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Ultimate LOD System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);
                EditorGUILayout.HelpBox("You can supply a ULOD component here, the ULOD component provided here, will use this GameObject as a pivot to calculate distances and render the LODs correctly. For everything to work, it is necessary that this GameObject is a child of the ULOD that you want to modify.", MessageType.Info);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Custom Pivot Settings", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.targetUlodToChangePivot = (UltimateLevelOfDetail)EditorGUILayout.ObjectField(new GUIContent("ULOD To Set This Pivot",
                                                                        "Provide a ULOD component to use this GameObject as a pivot."),
                                                                        script.targetUlodToChangePivot, typeof(UltimateLevelOfDetail), true, GUILayout.Height(16));
                if (script.targetUlodToChangePivot != null && script.gameObject.transform.IsChildOf(script.targetUlodToChangePivot.transform) == false)
                {
                    Debug.LogError("It was not possible to assign this GameObject as a new pivot for the desired ULOD. This GameObject must be a child of the ULOD to which you want to define a new pivot.");
                    script.targetUlodToChangePivot = null;
                }
                if (script.targetUlodToChangePivot != null && script.gameObject.transform.IsChildOf(script.targetUlodToChangePivot.transform) == true)
                {
                    script.targetUlodToChangePivot.customPivotToSimulateLods = script.gameObject.transform;
                }

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
    }
}