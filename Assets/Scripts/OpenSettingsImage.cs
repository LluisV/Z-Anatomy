using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenSettingsImage : MonoBehaviour
{
    public ContentSizeFitter contentFitter;
    public GameObject container;
    public Image imageToChange;
    public Sprite openSprite;
    public Sprite closeSprite;
    public float expandY;
    private RectTransform rect;
    private float originalY;
    private bool open;
    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnOpenCloseClick()
    {
        open = !open;
        contentFitter.enabled = false;
        container.SetActive(open);
        if (open)
        {
            originalY = rect.sizeDelta.y;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, expandY);
            imageToChange.sprite = openSprite;
        }
        else
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, originalY);
            imageToChange.sprite = closeSprite;
        }
        contentFitter.enabled = true;
    }

}
