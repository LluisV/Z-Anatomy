using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public TMP_InputField tmpro_input;
    public TextMeshProUGUI tmpro;
    public NoteGizmo gizmo;
    public Line3D line;
    public TangibleBodyPart bodyPart;

    private RectTransform rt;

    private bool isVisible = true;

    public Button boldBtn;
    public Button italicBtn;
    public Button underlineBtn;

    private Camera cam;

    private void Awake()
    {

       // tmpro_input.onTextSelection.AddListener(GetSelectedText);
        rt = GetComponent<RectTransform>();
        tmpro_input.richText = false;
        cam = Camera.main;
    }

 /*   private void GetSelectedText(string str, int start, int end)
    {
        selectionStart = Mathf.Min(start, end);
        selectionEnd = Mathf.Max(start, end);
        Debug.Log(str.Substring(Mathf.Min(start, end), Mathf.Abs(end - start)));
    }*/

    private void LateUpdate()
    {
        line.lineRenderer.SetPosition(1, cam.ScreenToWorldPoint(rt.position));
    }

    public void GizmoClick()
    {
        if (isVisible)
            Collapse();
        else
            Expand();
    }

    public void Expand()
    {
        gameObject.SetActive(true);
        line.gameObject.SetActive(true);
        isVisible = true;
    }
    
    public void Collapse()
    {
        gameObject.SetActive(false);
        line.gameObject.SetActive(false);
        isVisible = false;
    }

    public void Highlight()
    {

        tmpro_input.ForceLabelUpdate();
        tmpro_input.text = tmpro.text.RemoveBodyPartLinks();
      //  StartCoroutine(HighlightText.Hightlight(tmpro, "", tmpro_input));
        SetCaretPosition(tmpro_input.text.Length - 1);
    }

    void SetCaretPosition(int caretIndex)
    {

        StartCoroutine(SetPosition());

        IEnumerator SetPosition()
        {
            int width = tmpro_input.caretWidth;
            tmpro_input.caretWidth = 0;

            yield return new WaitForEndOfFrame();

            tmpro_input.caretWidth = width;
            tmpro_input.caretPosition = caretIndex;
        }
    }

    public void EditModeClick()
    {
        if (!tmpro_input.richText)
            Highlight();
        tmpro_input.richText = !tmpro_input.richText;

    }
}
