using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MTAssets.UltimateLODSystem
{
    public class RuntimeScanDemo : MonoBehaviour
    {
        public UltimateLevelOfDetail ulodOfScene;
        public Text buttonText;
        public GameObject buttonObj;
        public Text scanStatus;
        public Animator cameraAnimator;

        void Start()
        {
            ulodOfScene.onDoneScan.AddListener(() =>
            {
                scanStatus.text = "Scan Done! Showing LOD Demo";
                cameraAnimator.SetBool("runLoop", true);
                buttonObj.SetActive(true);
            });
            ulodOfScene.onUndoScan.AddListener(() =>
            {
                scanStatus.text = "No Scan Performed Yet";
                cameraAnimator.SetBool("runLoop", false);
                buttonObj.SetActive(true);
            });
        }

        void Update()
        {
            if (ulodOfScene.isMeshesCurrentScannedAndLodsWorkingInThisComponent() == true)
                buttonText.text = "Undo Current Scan And Delete Generated LODs";
            if (ulodOfScene.isMeshesCurrentScannedAndLodsWorkingInThisComponent() == false)
                buttonText.text = "Do Scan And Generete LOD Groups";
        }

        public void StartUndoScan()
        {
            if (ulodOfScene.isMeshesCurrentScannedAndLodsWorkingInThisComponent() == true)
            {
                scanStatus.text = "Undoing Scan...";
                buttonObj.SetActive(false);
                ulodOfScene.UndoCurrentScanWorkingAndDeleteGeneratedMeshes(true, true);
                return;
            }
            if (ulodOfScene.isMeshesCurrentScannedAndLodsWorkingInThisComponent() == false)
            {
                scanStatus.text = "Scanning...";
                buttonObj.SetActive(false);
                ulodOfScene.ScanAllMeshesAndGenerateLodsGroups();
                return;
            }
        }
    }
}