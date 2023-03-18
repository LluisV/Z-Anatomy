using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System;

public class Shortcuts : MonoBehaviour
{
    public static Shortcuts Instance;

    [Header("Selection shortcuts")]
    public InputAction changeSelectionToolShortcut;
    public InputAction invertShortcut;
    public InputAction multipleSelectionShortcut;
    public InputAction selectAll;
   // public Key[] brushSizeShortcut; // + mouse scroll

    [Space]
    [Header("Visibility shortcuts")]
    public InputAction isolateShortcut;
    public InputAction isolateAndLabelsShortcut;
    public InputAction partialIsolateShortcut;
    public InputAction deleteShortcut;
    public InputAction keyColors;

    [Space]
    [Header("Cross sections shortcuts")]
    public InputAction setPlaneGizmo;

    [Space]
    [Header("Camera shortcuts")]
    public InputAction center;

    [Space]
    [Header("Panels shortcuts")]
    public InputAction showLexicon;
    public InputAction showDescription;
    public InputAction showHelp;
    public InputAction showSettings;
    public InputAction overlay;


    [Space]
    [Header("Gizmo shortcuts")]
    public InputAction left;
    public InputAction rigth;
    public InputAction up;
    public InputAction down;
    public InputAction _X;
    public InputAction _Y;
    public InputAction _Z;

    [Space]
    [Header("General shortcuts")]
    public InputAction search;
    public InputAction reset;
    public InputAction undo;
    public InputAction redo;
    public InputAction copy;

    [Header("Hidden shortcuts")]
    public InputAction clearPlayerPrefs;

    private void Awake()
    {
        Instance = this;

        changeSelectionToolShortcut.Enable();
        changeSelectionToolShortcut.performed += ChangeSelectionTool;
        invertShortcut.Enable();
        invertShortcut.performed += Invert;
        multipleSelectionShortcut.Enable();
        multipleSelectionShortcut.started += MultipleSelectionStarted;
        multipleSelectionShortcut.canceled += MultipleSelectionCanceled;
        selectAll.Enable();
        selectAll.performed += SelectAll;

        isolateShortcut.Enable();
        isolateShortcut.performed += Isolate;
        isolateAndLabelsShortcut.Enable();
        isolateAndLabelsShortcut.performed += IsolateAndLabels;
        partialIsolateShortcut.Enable();
        partialIsolateShortcut.performed += PartialIsolate;
        deleteShortcut.Enable();
        deleteShortcut.performed += Delete;
        keyColors.Enable();
        keyColors.performed += KeyColors;

        setPlaneGizmo.Enable();

        center.Enable();
        center.performed += Center;

        showLexicon.Enable();
        showLexicon.performed += ShowLexicon;
        showDescription.Enable();
        showDescription.performed += ShowDescription;
        showHelp.Enable();
        showHelp.performed += ShowHelp;
        showSettings.Enable();
        showSettings.performed += ShowSettings;
        overlay.Enable();
        overlay.started += ShowTutorialStarted;
        overlay.canceled += ShowTutorialCanceled;

        left.Enable();
        left.performed += Left;
        rigth.Enable();
        rigth.performed += Right;
        up.Enable();
        up.performed += Up;
        down.Enable();
        down.performed += Down;
        _X.Enable();
        _X.performed += X;
        _Y.Enable();
        _Y.performed += Y;
        _Z.Enable();
        _Z.performed += Z;

        search.Enable();
        search.performed += Search;
        reset.Enable();
        reset.performed += Reset;
        copy.Enable();
        copy.performed += Copy;

        undo.Enable();
        undo.performed += Undo;
        redo.Enable();
        redo.performed += Redo;

        clearPlayerPrefs.Enable();
        clearPlayerPrefs.performed += ClearPlayerPrefs;

    }

    private void ChangeSelectionTool(InputAction.CallbackContext context)
    {
        if (ActionControl.boxSelection)
            ActionControl.Instance.BrushSelectionClick();
        else if (ActionControl.brushSelection)
            ActionControl.Instance.LassoSelectionClick();
        else if (ActionControl.lassoSelection)
            ActionControl.Instance.BoxSelectionClick();
    }

    private void Invert(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        SelectedObjectsManagement.Instance.InvertSelection();
    }

    private void MultipleSelectionStarted(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        ActionControl.multipleSelection = true;
    }

    private void MultipleSelectionCanceled(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        ActionControl.multipleSelection = false;
    }

    private void SelectAll(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        ActionControl.Instance.SelectAll();

    }

    private void Isolate(InputAction.CallbackContext context)
    {
        if (UserIsWriting() || Keyboard.current.leftCtrlKey.isPressed)
            return;
        MeshManagement.Instance.IsolationClick(false);
    }

    private void IsolateAndLabels(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        MeshManagement.Instance.IsolationClick();
    }

    private void PartialIsolate(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        MeshManagement.Instance.PartialIsolationClick();

    }

    private void Delete(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        if(SelectedObjectsManagement.Instance.selectedObjects.Count > 0)
            SelectedObjectsManagement.Instance.DeleteSelected();

    }

    private void KeyColors(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        if(ContextualMenu.Instance.ShowKeyColorsOn() || ContextualMenu.Instance.HideKeyColorsOn())
            ContextualMenu.Instance.ShowKeyColorsClick();
    }

    private void Center(InputAction.CallbackContext context)
    {
        if (UserIsWriting() || Keyboard.current.leftCtrlKey.isPressed)
            return;
        CameraController.instance.CenterView(true);
    }

    private void ShowLexicon(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        PanelsManagement.instance.ShowLexicon();

    }

    private void ShowDescription(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        PanelsManagement.instance.ShowDescription();
    }

    private void ShowHelp(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        PanelsManagement.instance.ShowHelp();

    }

    private void ShowSettings(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        PanelsManagement.instance.ShowSettings();

    }

    private void ShowTutorialStarted(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        ActionControl.Instance.OpenHelpOverlay();

    }

    private void ShowTutorialCanceled(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        ActionControl.Instance.CloseHelpOverlay();
    }

    private void Left(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        GizmoBehaviour.instance.SetFaceShortcut(GizmoFace.Left);

    }

    private void Right(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        GizmoBehaviour.instance.SetFaceShortcut(GizmoFace.Right);

    }

    private void Up(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        GizmoBehaviour.instance.SetFaceShortcut(GizmoFace.Up);

    }
    private void Down(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        GizmoBehaviour.instance.SetFaceShortcut(GizmoFace.Down);

    }
    private void X(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        if (!CrossSections.Instance.xPlane)
        {
            CrossSections.Instance.inverted = false;

            if (CrossSections.Instance.ixPlane)
                CrossPlanesGizmo.Instance.InvertClick();
            else
            {
                if (CrossSections.Instance.invertedSliders)
                    CrossSections.Instance.InvertSliders();
                CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Left);
            }
        }
        else
            CrossPlanesGizmo.Instance.InvertClick();

        if (!CrossPlanesGizmo.Instance.opened)
            CrossPlanesGizmo.Instance.OpenClosePlanesClick();
    }
    private void Y(InputAction.CallbackContext context)
    {
        if (UserIsWriting() || Keyboard.current.leftCtrlKey.isPressed)
            return;
        if (!CrossSections.Instance.yPlane)
        {
            CrossSections.Instance.inverted = false;

            if (CrossSections.Instance.iyPlane)
                CrossPlanesGizmo.Instance.InvertClick();
            else
            {
                if (CrossSections.Instance.invertedSliders)
                    CrossSections.Instance.InvertSliders();
                CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Front);
            }
        }
        else
            CrossPlanesGizmo.Instance.InvertClick();

        if (!CrossPlanesGizmo.Instance.opened)
            CrossPlanesGizmo.Instance.OpenClosePlanesClick();
    }
    private void Z(InputAction.CallbackContext context)
    {
        if (UserIsWriting() || Keyboard.current.leftCtrlKey.isPressed)
            return;
        if (!CrossSections.Instance.zPlane)
        {
            CrossSections.Instance.inverted = false;

            if (CrossSections.Instance.izPlane)
                CrossPlanesGizmo.Instance.InvertClick();
            else
            {
                if (CrossSections.Instance.invertedSliders)
                    CrossSections.Instance.InvertSliders();
                CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Up);
            }
        }
        else
            CrossPlanesGizmo.Instance.InvertClick();

        if (!CrossPlanesGizmo.Instance.opened)
            CrossPlanesGizmo.Instance.OpenClosePlanesClick();
    }
    private void Search(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        SearchEngine.Instance.mainInputField.Select();
        SearchEngine.Instance.mainInputField.ActivateInputField();
    }

    private void Reset(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        ActionControl.Instance.ResetAll();

    }

    private void Undo(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        ActionControl.Instance.Undo();
    }

    private void Redo(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        ActionControl.Instance.Redo();
    }
    private void Copy(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        GUIUtility.systemCopyBuffer = GUIUtility.systemCopyBuffer.RemoveRichTextTags();
        PopUpManagement.Instance.Show("Text copied!");
    }

    private void ClearPlayerPrefs(InputAction.CallbackContext context)
    {
        if (UserIsWriting())
            return;
        PlayerPrefs.DeleteAll();
        PopUpManagement.Instance.Show("Clear PlayerPrefs");
    }


    private void LayersShortcuts()
    {
        int layer = -2;

        if (Input.GetKeyDown(KeyCode.Alpha1) && !UserIsWriting())
            layer = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !UserIsWriting())
            layer = 2;
        else if(Input.GetKeyDown(KeyCode.Alpha3) && !UserIsWriting())
            layer = 4;
        else if(Input.GetKeyDown(KeyCode.Alpha4) && !UserIsWriting())
            layer = 6;
        else if(Input.GetKeyDown(KeyCode.Alpha5) && !UserIsWriting())
            layer = 8;
        else if(Input.GetKeyDown(KeyCode.Alpha6) && !UserIsWriting())
            layer = 10;
        else if(Input.GetKeyDown(KeyCode.Alpha7) && !UserIsWriting())
            layer = 12;
        else if(Input.GetKeyDown(KeyCode.Alpha8) && !UserIsWriting())
            layer = 14;
        else if(Input.GetKeyDown(KeyCode.Alpha9) && !UserIsWriting())
            layer = 16;
        else if(Input.GetKeyDown(KeyCode.Alpha0) && !UserIsWriting())
            layer = 18;

        if(Input.GetKey(KeyCode.LeftControl))
            layer++;
    }

    private bool UserIsWriting()
    {
        return EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null && !ActionControl.blockedInput;
    }
}
