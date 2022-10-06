using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ActionControl : MonoBehaviour
{
    public static bool boxSelection = false;
    public static bool lassoSelection = false;
    public static bool brushSelection;
    public static bool multipleSelection;
    public static bool crossSectionsEnabled = false;
    public static bool canZoom = true;
    public static bool scalpel = false;
    public static bool creatingLocalNote = false;
    public static bool creatingGlobalNote = false;
    public static bool pencil = false;
    public static bool zoomToMouse = true; 
    public static bool zoomSelected = true;
    public static bool draggingGizmo = false;
    public static bool nameOnMouse = true;
    public static bool limitRotation = true;
    public static bool draggingMoveIcon = false;
    public static bool draggingRotateIcon = false;
    public static bool blockedInput = false;

    private CameraController cam;

    public ExpandCollapseUI[] allExpandableTools;
    public NeumorphismUI.Neumorphism[] allNeumorpishmTools;

    public ToggleChangeColor boxSelectionButton;
    public ToggleChangeColor brushSelectionButton;
    public ToggleChangeColor lassoSelectionButton;
    public ToggleChangeColor scalpelButton;

    public BoxSelection boxSelectionScript;

    public BrushSelection brushSelectionScript;
    public static ActionControl Instance;

    public Button undoBtn;
    public Button redoBtn;

    public GameObject brushSelectionCanvas;
    public GameObject boxSelectionCanvas;

    public Image selectionOptionsImg;
    public Sprite boxSelectionSprite;
    public Sprite brushSelectionSprite;
    public Sprite lassoSelectionSprite;

    public RectTransform visibilityOptions;

    public Button upHierarchyBtn;
    public Button partialIsonationBtn;
    public Button[] buttonsEnabledOnObjectSelected;

    public GameObject ENOverlay;
    public GameObject ESOverlay;
    public GameObject FROverlay;
    public GameObject PTOverlay;

    public GameObject welcomeMessage;
    public Button enableTutorial;

    private void Awake()
    {
        Instance = this;
        creatingLocalNote = false;

        if(!PlayerPrefs.HasKey("Welcome"))
        {
            welcomeMessage.SetActive(true);
            blockedInput = true;
        }
        else
            enableTutorial.interactable = true;

    }

    private void Start()
    {
        cam = Camera.main.GetComponent<CameraController>();
        boxSelectionCanvas.SetActive(true);
        BoxSelectionClick();
    }

    public void ScalpelClick()
    {
        scalpel = true;
        if (boxSelection)
            BoxSelectionClick();
        else if (brushSelection)
            BrushSelectionClick();
        else if (lassoSelection)
            LassoSelectionClick();
        scalpelButton.SetEnabledColor();
    }

    public void BrushSelectionClick()
    {
        if(!brushSelection)
        {
            brushSelection = true;
            boxSelection = false;
            lassoSelection = false;

            selectionOptionsImg.sprite = brushSelectionSprite;

            brushSelectionButton.SetEnabledColor();
            boxSelectionButton.SetDisabledColor();
            lassoSelectionButton.SetDisabledColor();

            brushSelectionCanvas.SetActive(true);
            LassoSelection.instance.Clear();
        }
    }

    public void BoxSelectionClick()
    {
        if(!boxSelection)
        {
            boxSelection = true;
            brushSelection = false;
            lassoSelection = false;

            selectionOptionsImg.sprite = boxSelectionSprite;

            boxSelectionButton.SetEnabledColor();
            brushSelectionButton.SetDisabledColor();
            lassoSelectionButton.SetDisabledColor();

            boxSelectionCanvas.SetActive(true);
            LassoSelection.instance.Clear();
        }
    }

    public void LassoSelectionClick()
    {
        if(!lassoSelection)
        {
            lassoSelection = true;
            brushSelection = false;
            boxSelection = false;

            selectionOptionsImg.sprite = lassoSelectionSprite;

            lassoSelectionButton.SetEnabledColor();
            brushSelectionButton.SetDisabledColor();
            boxSelectionButton.SetDisabledColor();
        }
    }

    public void SelectAll()
    {
        SelectedObjectsManagement.Instance.GetActiveObjects();
        foreach (var item in SelectedObjectsManagement.Instance.activeObjects)
            SelectedObjectsManagement.Instance.SelectObject(item);

        AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
    }

    public void DeleteSelection()
    {
        SelectedObjectsManagement.Instance.DeselectAllObjects();
        boxSelectionScript.ResetSelection();
    }

    public void DisableBrushSelection()
    {
        brushSelectionButton.SetDisabledColor();
        brushSelection = false;
        brushSelectionScript.HideImage();
    }

    public void Isolate()
    {
        MeshManagement.Instance.IsolationClick();
    }

    public void PartiallyIsolate()
    {
        MeshManagement.Instance.PartialIsolationClick();
    }

    public void CollapseAll()
    {
        foreach (var item in allExpandableTools)
        {
            item.Collapse();
        }
        foreach (var item in allNeumorpishmTools)
        {
            item.Collapse();
        }
    }

    public void ResetAll()
    {
        foreach (var section in GlobalVariables.Instance.bodySections)
        {
            if (section.CompareTag("Skeleton"))
                section.transform.SetActiveRecursively(true);
            else
                section.transform.SetActiveRecursively(false);
        }

        foreach (var bodypart in GlobalVariables.Instance.allBodyParts.Where(it => it.displaced))
            bodypart.ResetPosition();

        BoxSelectionClick();
        SelectedObjectsManagement.Instance.DeselectAllObjects();
        SelectedObjectsManagement.Instance.lastIsolatedObjects.Clear();
        MeshManagement.Instance.DisableInsertions();
        Lexicon.Instance.ResetAll();
        cam.ResetCamera();
        SelectedObjectsManagement.Instance.GetActiveObjects();
        CrossPlanesGizmo.Instance.ResetAll();
        CrossSections.Instance.ResetAll();
        CommandController.Reset();
        KeyColors.Instance.ResetAll();
        TranslateObject.Instance.Disable();
        PanelsManagement.instance.Reset();
        MeshManagement.Instance.HideAllLabels();
    }

    public void AddCommand(ICommand command, bool execute)
    {
        if(CommandController.IsLastDifferent(command) && !command.IsEmpty())
        {
            CommandController.AddCommand(command, execute);
            undoBtn.interactable = CommandController.UndoEnabled();
            redoBtn.interactable = CommandController.RedoEnabled();
            //Debug.Log(command);
        }
    }

    public void Undo()
    {
        CommandController.UndoCommand();
        undoBtn.interactable = CommandController.UndoEnabled();
        redoBtn.interactable = CommandController.RedoEnabled();
        Lexicon.Instance.UpdateTreeViewCheckboxes();

    }

    public void Redo()
    {
        CommandController.RedoCommand();
        undoBtn.interactable = CommandController.UndoEnabled();
        redoBtn.interactable = CommandController.RedoEnabled();
        Lexicon.Instance.UpdateTreeViewCheckboxes();

    }

    public void UpdateButtons()
    {
        //Optimize this (Check which tags are active)
        bool isParentDisabled = !GlobalVariables.Instance.globalParent.activeInHierarchy;
        if (isParentDisabled)
            GlobalVariables.Instance.globalParent.SetActive(true);

        partialIsonationBtn.gameObject.SetActive(IsPartialIsolationEnabled());
        foreach (Button btn in buttonsEnabledOnObjectSelected)
            btn.gameObject.SetActive(SelectedObjectsManagement.Instance.selectedObjects.Count > 0);

        if (SelectedObjectsManagement.Instance.lastParentSelected == null)
            upHierarchyBtn.interactable = SelectedObjectsManagement.Instance.selectedObjects.Count == 1;
        else
            upHierarchyBtn.interactable = !SelectedObjectsManagement.Instance.lastParentSelected.parent.CompareTag("GlobalParent");

        if (isParentDisabled)
            GlobalVariables.Instance.globalParent.SetActive(false);

        visibilityOptions.gameObject.SetActive(SelectedObjectsManagement.Instance.selectedObjects.Count > 0);

        if (visibilityOptions.gameObject.activeInHierarchy)
            PanelsManagement.instance.ExpandOptionsPanel();
        else
            PanelsManagement.instance.CollapseOptionsPanel();
    }

    public bool IsPartialIsolationEnabled()
    {
        List<string> selectedTags = new List<string>();
        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
        {
            if (!selectedTags.Contains(item.tag))
            {
                selectedTags.Add(item.tag);
            }
        }
        List<string> actualTags = new List<string>();
        foreach (var item in SelectedObjectsManagement.Instance.activeObjects)
        {
            if (!actualTags.Contains(item.tag))
            {
                actualTags.Add(item.tag);
            }
        }
        return selectedTags.Count == 1 && actualTags.Count > 1 && SelectedObjectsManagement.Instance.selectedObjects.Count > 0;
    }

    public void AddLocalNoteClick()
    {
        creatingLocalNote = true;
        CameraController.instance.raycaster.enabled = false;

        /*if (!ContextualMenu.Instance.contextObject.IsSelected())
            ContextualMenu.Instance.contextObject.ObjectClicked();*/
    }

    public void AddGlobalNoteClick()
    {
        creatingGlobalNote = true;
        CameraController.instance.raycaster.enabled = false;

    }

    public void DrawClick()
    {
        pencil = !pencil;

        boxSelection = false;
        brushSelection = false;
        lassoSelection = false;

        PanelsManagement.instance.ShowPencilOptions();
        if (!pencil)
            BoxSelectionClick();
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void MinimizeApp()
    {
        BorderlessWindow.MinimizeWindow();
    }

    public void SetMoveDragOn() => draggingMoveIcon = true;
    public void SetRotateDragOn() => draggingRotateIcon = true;
    public void SetMoveDragOff() => draggingMoveIcon = false;
    public void SetRotateDragOff() => draggingRotateIcon = false;

    public void OpenHelpOverlay()
    {
        switch (Settings.language)
        {
            case SystemLanguage.English:
                ENOverlay.SetActive(true);
                break;
            case SystemLanguage.Spanish:
                ESOverlay.SetActive(true);
                break;
            case SystemLanguage.Portuguese:
                PTOverlay.SetActive(true);
                break;
            case SystemLanguage.French:
                FROverlay.SetActive(true);
                break;
            default:
                ENOverlay.SetActive(true);
                break;
        }
        blockedInput = true;

    }

    public void CloseHelpOverlay()
    {
        ENOverlay.SetActive(false);
        ESOverlay.SetActive(false);
        FROverlay.SetActive(false);
        PTOverlay.SetActive(false);

        blockedInput = false;
    }


    public void WelcomeCompleted()
    {
        PlayerPrefs.SetInt("Welcome", 1);
        blockedInput = false;
        enableTutorial.interactable = true;
    }

}
