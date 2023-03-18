using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class CollectionElement : MonoBehaviour
{
    public TextMeshProUGUI nameTMP;
    public Image backgroundImage;
    public RawImage image;
    [HideInInspector]
    public CollectionElementData collectionData;
    public float longClickDuration;
    private float ellapsedClickTime = 0;
    private bool mouseDown;
    public Image checkImage;
    public Sprite uncheckedSprite;
    public Sprite checkedSprite;
    [HideInInspector]
    public bool selected;

    private void Update()
    {
        if (mouseDown)
        {
            ellapsedClickTime += Time.deltaTime;
            if(ellapsedClickTime > longClickDuration)
            {
                mouseDown = false;
                LongClick();
            }
        }
    }

    public CollectionElement(CollectionElementData elementData)
    {
        nameTMP.text = elementData.name;
        Color col = Color.white;
        ColorUtility.TryParseHtmlString(elementData.backgroundColorHex, out col);
        backgroundImage.color = col;
    }

    public void CollectionMouseDown()
    {
        ellapsedClickTime = 0;
        mouseDown = true;
    }

    public void CollectionMouseUp()
    {
        mouseDown = false;
        if (ellapsedClickTime < longClickDuration)
            ShortClick();
    }

    private void ShortClick()
    {
        if(Collections.instance.selecting)
        {
            if (!selected)
                Select();
            else
                Deselect();
        }
        else
        {
            //Disable everything
            GlobalVariables.Instance.globalParent.transform.SetActiveRecursively(false);

            //Get all objects
            List<NameAndDescription> gameObjects = GlobalVariables.Instance.allNameScripts
            .Where(it => !it.gameObject.transform.parent.name.Contains(".labels"))
            .Select(it => it.GetComponent<NameAndDescription>())
            .ToList();

            //Enable each collection element
            foreach (string item in collectionData.elements)
            {
                var found = gameObjects.Where(it => it != null && it.originalName + it.leftRight == item).FirstOrDefault();
                if (found != null)
                {
                    found.transform.SetActiveParentsRecursively(true);
                    gameObjects.Remove(found);
                }
            }
            Collections.instance.collectionsCanvas.SetActive(false);
            Lexicon.Instance.UpdateTreeViewCheckboxes();
            Camera.main.GetComponent<CameraController>().CenterImmediate();
        }
    }

    private void LongClick()
    {
        if(!Collections.instance.selecting)
        {
            Collections.instance.SelectStart();
            Select();
        }
    }

    public void EnableSelection()
    {
        checkImage.enabled = true;
        checkImage.sprite = uncheckedSprite;
    }

    public void DisableSelection()
    {
        checkImage.enabled = false;
    }

    private void Select()
    {
        selected = true;
        checkImage.sprite = checkedSprite;
        Collections.instance.selectedCollections.Add(this);
    }

    public void Deselect()
    {
        selected = false;
        checkImage.sprite = uncheckedSprite;
        Collections.instance.selectedCollections.Remove(this);
        if (Collections.instance.selectedCollections.Count == 0)
        {
            Collections.instance.SelectEnd();
            Collections.instance.selecting = false;
        }
    }
}
