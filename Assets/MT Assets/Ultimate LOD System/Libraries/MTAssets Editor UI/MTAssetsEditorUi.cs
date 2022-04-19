#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace MTAssets.UltimateLODSystem
{
    /*
     * This is a script that is part of the essential library for "MT Assets" assets.
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class MTAssetsEditorUi : MonoBehaviour
    {
#if UNITY_EDITOR

        //Public methods

        //This method disable gizmo of a script that have a icon
        public static bool DisableGizmosInSceneView(string scriptClassNameToDisable, bool isGizmosDisabled)
        {
            /*
            *  This method disables Gizmos in scene view, for this component
            */

            if (isGizmosDisabled == true)
                return true;

            //Try to disable
            try
            {
                //Get all data of Unity Gizmos manager window
                var Annotation = System.Type.GetType("UnityEditor.Annotation, UnityEditor");
                var ClassId = Annotation.GetField("classID");
                var ScriptClass = Annotation.GetField("scriptClass");
                var Flags = Annotation.GetField("flags");
                var IconEnabled = Annotation.GetField("iconEnabled");

                System.Type AnnotationUtility = System.Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");
                var GetAnnotations = AnnotationUtility.GetMethod("GetAnnotations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var SetIconEnabled = AnnotationUtility.GetMethod("SetIconEnabled", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                //Scann all Gizmos of Unity, of this project
                System.Array annotations = (System.Array)GetAnnotations.Invoke(null, null);
                foreach (var a in annotations)
                {
                    int classId = (int)ClassId.GetValue(a);
                    string scriptClass = (string)ScriptClass.GetValue(a);
                    int flags = (int)Flags.GetValue(a);
                    int iconEnabled = (int)IconEnabled.GetValue(a);

                    // this is done to ignore any built in types
                    if (string.IsNullOrEmpty(scriptClass))
                    {
                        continue;
                    }

                    const int HasIcon = 1;
                    bool hasIconFlag = (flags & HasIcon) == HasIcon;

                    //If the current gizmo is of the class desired, disable the gizmo in scene
                    if (scriptClass == scriptClassNameToDisable)
                    {
                        if (hasIconFlag && (iconEnabled != 0))
                        {
                            /*UnityEngine.Debug.LogWarning(string.Format("Script:'{0}' is not ment to show its icon in the scene view and will auto hide now. " +
                                "Icon auto hide is checked on script recompile, if you'd like to change this please remove it from the config", scriptClass));*/
                            SetIconEnabled.Invoke(null, new object[] { classId, scriptClass, 0 });
                        }
                    }
                }

                return true;
            }
            //Catch any error
            catch (System.Exception exception)
            {
                string exceptionOcurred = "";
                exceptionOcurred = exception.Message;
                if (exceptionOcurred != null)
                    exceptionOcurred = "";
                return false;
            }
        }

        //This method returns the current width of inspector window
        public static Rect GetInspectorWindowSize()
        {
            //Returns the current size of inspector window
            return EditorGUILayout.GetControlRect(true, 0f);
        }
#endif
    }
}