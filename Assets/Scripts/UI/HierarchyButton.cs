using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Assets.Scripts.Commands;

public class HierarchyButton : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public GameObject obj;
    [HideInInspector]
    public bool isRoot;
    [HideInInspector]
    public bool isFirst;
    private TextMeshProUGUI tmpro;
    public Image image;
    private RectTransform trans;

    private void Awake()
    {
        tmpro = GetComponentInChildren<TextMeshProUGUI>();
        trans = GetComponent<RectTransform>();
    }

    private IEnumerator Start()
    {
        image.enabled = !isRoot;
        tmpro.text = obj.transform.name.RemoveSuffix();
        if (isFirst)
        {
            tmpro.color = GlobalVariables.HighligthColor;
            tmpro.fontSize += 7;
            tmpro.fontStyle = FontStyles.Bold;
        }
        yield return new WaitForEndOfFrame();
        trans.sizeDelta = new Vector2(tmpro.textBounds.size.x + 10, trans.sizeDelta.y);
    }

    public void OnClick()
    {
        List<GameObject> shown = new List<GameObject>();

        SelectedObjectsManagement.Instance.lastParentSelected = obj.transform;
        SelectedObjectsManagement.Instance.SelectAllChildren(obj.transform, true, shown);
        SelectedObjectsManagement.Instance.CheckLabelsVisibility();
        NameAndDescription nameScript = obj.GetComponent<NameAndDescription>();
        nameScript.SetDescription();
        HierarchyBar.Instance.Set(obj.transform);
        //Expand in lexicon
        Lexicon.Instance.ExpandRecursively();
        if(ActionControl.zoomSelected)
            CameraController.instance.CenterView(true);
        TranslateObject.Instance.UpdateSelected();

        if (shown.Count > 0)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);
        ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);

    }

    public void OnPointerClick(PointerEventData eventData)
    {    
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ContextualMenu.Instance.contextObject = obj;
            ContextualMenu.Instance.Show();
        }
    }
}
