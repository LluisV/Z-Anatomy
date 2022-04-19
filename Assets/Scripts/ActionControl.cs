using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionControl : MonoBehaviour
{
    public static bool boxSelection;
    public static bool brushSelection;
    public static bool multipleSelection;
    public static bool someObjectSelected = false;
    public static bool canErase;
    public static bool twoFingerOnScreen = false;
    public static bool crossSectionsEnabled;

    private CameraController cam;

    public ExpandCollapseUI[] selectionToolsAnimations;

    public ToggleChangeColor selectionButton;
    public ToggleChangeColor multipleSelectionButton;
    public ToggleChangeColor brushSelectionButton;
    public ToggleChangeColor boxSelectionButton;
    public ToggleChangeColor deleteButton;
    public Toggle[] togglesToReset;

    public GameObject brushDiameterSlider;
    public BoxSelection boxSelectionScript;

    public BrushSelection brushSelectionScript;
    public RectTransform orientationCube;
    public RectTransform crossPlanesCube;
    public static ActionControl instance;

    public GameObject settingsPanel;
    public GameObject helpPanel;
    public GameObject collabPanel;
    public GameObject emailPanel;
    public GameObject aboutPanel;
    public GameObject brushSelectionCanvas;
    public GameObject boxSelectionCanvas;

    private bool isolationToolsOpened;
    private bool selectionToolsOpened;

    private RectTransform cubeCanvasRT;
    private Vector2 cubeCanvasOriginalSize;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = Camera.main.GetComponent<CameraController>();
        cubeCanvasRT = Settings.instance.cubeCanvas.GetComponent<RectTransform>();
        cubeCanvasOriginalSize = cubeCanvasRT.sizeDelta;
        if (GlobalVariables.instance.mobile)
            StartCoroutine(CheckForOrientationChange());
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            //Close the tree view
            if (TreeViewCanvas.instance.expandScript.isExpanded)
            {
                TreeViewCanvas.instance.expandScript.ExpandButtonClick();
                UIManager.draggingUI = false;
            }
            if (NamesManagement.instance.expandScript.isExpanded)
            {
                NamesManagement.instance.expandScript.ExpandButtonClick();
                UIManager.draggingUI = false;
            }
            if (settingsPanel.activeInHierarchy)
            {
                settingsPanel.SetActive(false);
                MeshManagement.instance.globalParent.SetActive(true);
                MeshManagement.instance.RefreshReflections();
            }
            if (helpPanel.activeInHierarchy)
            {
                helpPanel.SetActive(false);
                MeshManagement.instance.globalParent.SetActive(true);
            }
            if (collabPanel.activeInHierarchy)
            {
                collabPanel.SetActive(false);
                MeshManagement.instance.globalParent.SetActive(true);
            }
            if (emailPanel.activeInHierarchy)
            {
                emailPanel.SetActive(false);
                collabPanel.SetActive(true);
            }
            if (aboutPanel.activeInHierarchy)
            {
                aboutPanel.SetActive(false);
                settingsPanel.SetActive(true);
            }
        }
    }


    IEnumerator CheckForOrientationChange()
    {
        yield return new WaitForSeconds(1);
        if (GlobalVariables.actualScreenSize != new Vector2(Screen.width, Screen.height))
        {
            GlobalVariables.actualScreenSize = new Vector2(Screen.width, Screen.height);
            OrientationChanged();
        }
        StartCoroutine(CheckForOrientationChange());
    }

    private void OrientationChanged()
    {
        //Portrait to landscape
        //Is mobile and last orientation was portrait and actual orientation is not portrait
        if (GlobalVariables.instance.mobile && (GlobalVariables.screenOrientation == ScreenOrientation.Portrait || GlobalVariables.screenOrientation == ScreenOrientation.PortraitUpsideDown) && !(Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown))
        {
            GlobalVariables.screenOrientation = Screen.orientation;
            CubeBehaviour orientationCube = ActionControl.instance.orientationCube.GetComponent<CubeBehaviour>();
            CubeBehaviour crossPlanesCube = ActionControl.instance.crossPlanesCube.GetComponent<CubeBehaviour>();
            orientationCube.cubePosition.y = orientationCube.originalCubePosition.y - 0.3f;
            crossPlanesCube.cubePosition.y = crossPlanesCube.originalCubePosition.y - 0.3f;
        }
        //Landscape to portrait
        //Is mobile and last orientation was not portrait and actual orientation is portrait
        else if (GlobalVariables.instance.mobile && !(GlobalVariables.screenOrientation == ScreenOrientation.Portrait || GlobalVariables.screenOrientation == ScreenOrientation.PortraitUpsideDown) && (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown))
        {
            GlobalVariables.screenOrientation = Screen.orientation;
            CubeBehaviour orientationCube = ActionControl.instance.orientationCube.GetComponent<CubeBehaviour>();
            CubeBehaviour crossPlanesCube = ActionControl.instance.crossPlanesCube.GetComponent<CubeBehaviour>();
            orientationCube.cubePosition.y = orientationCube.originalCubePosition.y;
            crossPlanesCube.cubePosition.y = crossPlanesCube.originalCubePosition.y;
        }

        if(!NamesManagement.instance.expandScript.isExpanded)
        {
            NamesManagement.instance.SetPosToRight();
            NamesManagement.instance.SetCollapsedToRight();
        }
        if(!TreeViewCanvas.instance.expandScript.isExpanded)
        {
            TreeViewCanvas.instance.SetPosToLeft();
            TreeViewCanvas.instance.SetCollapsedToLeft();
        }
    }


    public void BoxSelectionClick()
    {
        boxSelection = !boxSelection;
        if (boxSelection)
        {
            boxSelectionCanvas.SetActive(true);
            brushSelection = false;
            brushSelectionButton.SetDisabledColor();
            brushDiameterSlider.SetActive(false);
            brushSelectionScript.HideImage();

        }
        else
        {
            boxSelectionCanvas.SetActive(false);
        }
        if(multipleSelection)
        {
            multipleSelection = false;
            multipleSelectionButton.SetDisabledColor();
        }
        if(canErase)
        {
            canErase = false;
            deleteButton.SetDisabledColor();
        }
        SomeSelectionToolIsEnabled();
    }

    public void BrushSelectionClick()
    {
        brushSelection = !brushSelection;
        brushDiameterSlider.SetActive(brushSelection);
        if (brushSelection)
        {
            brushSelectionCanvas.SetActive(true);
            boxSelection = false;
            boxSelectionButton.SetDisabledColor();
        }
        else
        {
            brushSelectionCanvas.SetActive(false);
        }
        if(multipleSelection)
        {
            multipleSelection = false;
            multipleSelectionButton.SetDisabledColor();
        }
        if(canErase)
        {
            canErase = false;
            deleteButton.SetDisabledColor();
        }
        if (!brushSelection)
            brushSelectionScript.HideImage();
        SomeSelectionToolIsEnabled();
    }

    public void MultipleSelectionClick()
    {
        if(boxSelection)
        {
            boxSelection = false;
            boxSelectionButton.SetDisabledColor();
        }
        if (brushSelection)
        {
            brushSelection = false;
            brushSelectionButton.SetDisabledColor();
            brushDiameterSlider.SetActive(false);
            brushSelectionScript.HideImage();
        }
        if (canErase)
        {
            canErase = false;
            deleteButton.SetDisabledColor();
        }
        multipleSelection = !multipleSelection;
        SomeSelectionToolIsEnabled();
    }

    private void SomeSelectionToolIsEnabled()
    {
        if (multipleSelection || boxSelection || brushSelection)
            selectionButton.SetEnabledColor();
        else
            selectionButton.SetDisabledColor();
    }

    public void DisableSelection()
    {
        selectionToolsOpened = !selectionToolsOpened;
        if (multipleSelection || boxSelection || brushSelection)
        {
            selectionButton.SetDisabledColor();
            multipleSelectionButton.SetDisabledColor();
            boxSelectionButton.SetDisabledColor();
            brushSelectionButton.SetDisabledColor();
            multipleSelection = false;
            boxSelection = false;
            brushSelection = false;
            brushDiameterSlider.SetActive(false);
            brushSelectionScript.HideImage();
            foreach (var item in selectionToolsAnimations)
            {
                item.Collapse();
            }
        }
    }

    public void EraseClick()
    {
        canErase = !canErase;
        DisableSelection();
    }

    public void DeleteSelection()
    {
        SelectedObjectsManagement.instance.DeselectAllObjects();
        boxSelectionScript.ResetSelection();
    }

    public void Isolate()
    {
        MeshManagement.instance.IsolationClick();
        SomeIsolationToolIsEnabled();
    }

    public void PartiallyIsolate()
    {
        MeshManagement.instance.PartialIsolationClick();
        SomeIsolationToolIsEnabled();
    }

    private void SomeIsolationToolIsEnabled()
    {
      /*  if (isolationEnabled || partialIsolationEnabled)
            seeButton.SetEnabledColor();
        else
            seeButton.SetDisabledColor();*/
    }

    public void DisableTransparency()
    {
        isolationToolsOpened = !isolationToolsOpened;
      /*  if (isolationEnabled || partialIsolationEnabled)
        {
            seeButton.SetDisabledColor();

            isolationButton.SetDisabledColor();
            partialIsolationButton.SetDisabledColor();

            meshManagement.TransparencyClick();
        }*/
        if(isolationToolsOpened && selectionToolsOpened)
        {
            selectionButton.GetComponent<Button>().onClick.Invoke();
        }
       
    }

    public void Reset()
    {

        foreach (var section in MeshManagement.instance.bodySections)
        {
            if (section.CompareTag("Skeleton"))
                StaticMethods.SetActiveRecursively(section.transform, true);
            else
                StaticMethods.SetActiveRecursively(section.transform, false);
        }

        foreach (var toggle in togglesToReset)
        {
            toggle.isOn = false;
        }


        SelectedObjectsManagement.instance.UndoAll();
        SelectedObjectsManagement.instance.DeselectAllObjects();
        cam.ResetCamera();
        orientationCube.anchoredPosition = orientationCube.GetComponent<CubeBehaviour>().originalCubePosition;
        crossPlanesCube.anchoredPosition = crossPlanesCube.GetComponent<CubeBehaviour>().originalCubePosition;
        orientationCube.transform.localScale = Vector3.one * 30;
        crossPlanesCube.transform.localScale = Vector3.one * 30;
        cubeCanvasRT.sizeDelta = cubeCanvasOriginalSize;
        ActionControl.someObjectSelected = false;
    }
}
