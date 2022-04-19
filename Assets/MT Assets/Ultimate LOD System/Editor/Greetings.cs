using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

namespace MTAssets.UltimateLODSystem.Editor
{
    /*
     * This class is responsible for displaying the welcome message when installing this asset.
     */

    [InitializeOnLoad]
    class Greetings
    {
        //This asset parameters 

        public static string assetName = "Ultimate LOD System";
        public static string pathForThisAsset = "Assets/MT Assets/Ultimate LOD System";
        public static string pathForThisAssetDocumentation = "/_Documentation/Documentation (Open With Browser).html";
        public static string optionalObservation = "";
        public static string pathToGreetingsFile = "Assets/MT Assets/_AssetsData/Greetings/GreetingsData.Uls.ini";
        public static string linkForAssetStorePage = "https://assetstore.unity.com/publishers/40306";

        //Greetings script methods

        static Greetings()
        {
            //Run the script after Unity compiles
            EditorApplication.delayCall += Run;
        }

        static void Run()
        {
            //Create base directory "_AssetsData" and "Greetings" if not exists yet
            CreateBaseDirectoriesIfNotExists();

            //Verify if the greetings message already showed, if not yet, show the message
            VerifyAndShowAssetGreentingsMessageIfNeverShowedYet();
        }

        public static void CreateBaseDirectoriesIfNotExists()
        {
            //Create the directory to feedbacks folder, of this asset
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets"))
                AssetDatabase.CreateFolder("Assets", "MT Assets");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData"))
                AssetDatabase.CreateFolder("Assets/MT Assets", "_AssetsData");
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Greetings"))
                AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData", "Greetings");
        }

        public static void VerifyAndShowAssetGreentingsMessageIfNeverShowedYet()
        {
            //If the greetings file not exists
            if (AssetDatabase.LoadAssetAtPath(pathToGreetingsFile, typeof(object)) == null)
            {
                //Create a new greetings file
                File.WriteAllText(pathToGreetingsFile, "Done");

                //Show greetings and save 
                Regex regexFilter = new Regex("Assets/");
                bool optionClicked = EditorUtility.DisplayDialog(assetName + " was imported!",
                    "The " + assetName + " was imported for your project. Please do not change the directory of the files for this asset. You should be able to locate it in the folder \"" + regexFilter.Replace(pathForThisAsset, "", 1) + "\"" +
                    "\n\n" +
                    ((string.IsNullOrEmpty(optionalObservation) == false) ? optionalObservation + "\n\n" : "") +
                    "Remember to read the documentation to understand how to use this asset and get the most out of it!" +
                    "\n\n" +
                    "You can get support at email (mtassets@windsoft.xyz)" +
                    "\n\n" +
                    "- Thank you for purchasing the asset! :)",
                    "Ok, Cool!", "Open Documentation");

                //If clicked on "Ok, Cool!"
                if (optionClicked == true)
                {
                    //Select the folder of project
                    UnityEngine.Object assetFolder = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(pathForThisAsset, typeof(UnityEngine.Object));
                    Selection.activeObject = assetFolder;
                    EditorGUIUtility.PingObject(assetFolder);
                }
                //If clicked on "Open Documentation"
                if (optionClicked == false)
                {
                    //Select the folder of project
                    UnityEngine.Object docItem = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(pathForThisAsset + pathForThisAssetDocumentation, typeof(UnityEngine.Object));
                    Selection.activeObject = docItem;
                    EditorGUIUtility.PingObject(docItem);
                    AssetDatabase.OpenAsset(docItem);
                }

                //Update files
                AssetDatabase.Refresh();
            }
        }
    }
}